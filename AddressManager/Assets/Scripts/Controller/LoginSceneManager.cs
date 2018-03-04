using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
//using cn.sharesdk.unity3d;
using UnityEngine.SceneManagement;
using SimpleJson;
using NetworkModel;

public class LoginSceneManager : MonoBehaviour {

    [SerializeField] private Text logOrRegText, helpText;
    [SerializeField]
    private GameObject helpPanel;
    [SerializeField]
    private InputField userIdText, passwordText, comfirmPwText;

    private bool _isReg;
    private bool isReg
    {
        get { return _isReg;}
        set { _isReg = value;
            logOrRegText.text = value == true ? "返回登录" : "注册";
            _canvasAni.SetBool("Reg", value);
        }
    }
    private bool _isHelp;
    private bool isHelp
    {
        get { return _isHelp; }
        set { _isHelp = value;
            helpText.text = value == true ? "返回" : "帮助";
            helpPanel.SetActive(value);
        }
    }
    [SerializeField]
    private Animator _canvasAni;
    
	// Use this for initialization
	void Start () {
        //LocalDataModel.userName = null;

        setupTipsTextTrigger();

        autoSendLoginRequest();

    }

	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
            ProgressHUDManager.showYesOrNoHUD("确定退出吗？", (bool yesOrNo) =>
            {
                if (yesOrNo)
                {
                    Application.Quit();
                }
            });
        }
	}
    
    
	private void autoSendLoginRequest () {
        if (LocalDataModel.userName != null)
            sendLoginRequest();
        
	}

	public void sendLoginRequest () {

        //ProgressHUDManager.showHUD ("正在登陆游戏");

        string userId = null;
        string password = null;
        if (userIdText.text != "" && passwordText.text != "")
        {
            userId = userIdText.text;
            password = passwordText.text;
        }
        else
        {
            if(LocalDataModel.userId!=null)
                userId = LocalDataModel.userId;
            if (LocalDataModel.password != null)
                password = LocalDataModel.password;
        }

		ServiceUser.login (userId, password, (UserModel userObj) => {
			Debug.Log ("登陆返回");
			//ProgressHUDManager.hideCurrentHUD();
			if(userObj.code == 200) {
				LocalDataModel.isLogin = true;
				SceneManager.LoadScene("MainMenuScene");
			} else {
				ProgressHUDManager.showTipMsgHUD(userObj.msg);
			}
		});
	}
    public void sendRegisterRequest()
    {
        if(passwordText.text!=comfirmPwText.text)
        {
            ProgressHUDManager.showTipMsgHUD("密码确认不一致，请确认输入密码");
            return;
        }
        string userId = userIdText.text;
        string password = passwordText.text;
        ServiceUser.register(userId,password, (UserModel userObj) =>
         {
             Debug.Log("注册返回");
             if(userObj.code==200)
             {
                 LocalDataModel.setUserInfo(userObj);
                 ProgressHUDManager.showTipMsgHUD(userObj.msg);
             }
             else
             {
                 ProgressHUDManager.showTipMsgHUD(userObj.msg);
             }
         });
    }

    
	private void setupTipsTextTrigger () {
		EventTrigger trigger = logOrRegText.GetComponent<EventTrigger> ();
		EventTrigger.Entry entry = new EventTrigger.Entry ();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener ((data) => {
            OnClickRegister((PointerEventData)data);
		});
		trigger.triggers.Add (entry);

        trigger=helpText.GetComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => {
            OnClickHelp((PointerEventData)data);
        });
        trigger.triggers.Add(entry);
    } 

	private void OnClickRegister (PointerEventData data) {
        isReg = !isReg;
	}
    private void OnClickHelp(PointerEventData data)
    {
        isHelp = !isHelp;
    }
}
