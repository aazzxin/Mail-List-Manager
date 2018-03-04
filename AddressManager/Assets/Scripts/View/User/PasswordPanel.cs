using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordPanel : MonoBehaviour {

    [SerializeField]
    private InputField _passwordText, _comfirmPwText;
    [SerializeField]
    private Button _confirmBtn;

    public string password
    {
        get { return _passwordText.text; }
        set { _passwordText.text = value; }
    }
    public string confirmPw
    {
        get { return _comfirmPwText.text; }
        set { _comfirmPwText.text = value; }
    }

    public System.Action<string> modifyPasswordDelegate;

	// Use this for initialization
	void Start () {
        _confirmBtn.onClick.AddListener(onClickConfirm);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void onClickConfirm()
    {
        if (password == "" || confirmPw == "")
        {
            ProgressHUDManager.showTipMsgHUD("请输入密码");
        }
        else if (password != confirmPw)
        {
            ProgressHUDManager.showTipMsgHUD("确认密码不一致");
        }
        else if (modifyPasswordDelegate != null) 
        {
            modifyPasswordDelegate(password);
        }
    }
}
