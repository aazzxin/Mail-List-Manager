using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using SimpleJson;
using NetworkModel;

public class ReadySceneController : MonoBehaviour {

    #region 服务器无域名时，输入host
    public void SetGateHost(string host)
    {
        ServiceGate.Host = host;
    }
    public void Link()
    {
        StartCoroutine(setupNetworkConnect());
    }
    #endregion

    // Use this for initialization
    void Start () {
        // 先跑一次，让Loom初始化
        Loom.RunAsync(null);

        StartCoroutine(setupNetworkConnect());
    }
    private void Awake()
    {
        if (debugList != null)
        {
            debugList.debugListDelegate += debugListDo;
        }
    }

    IEnumerator setupNetworkConnect()
    {
        NetworkBase.Reset();

        yield return null;

        Debug.Log("开始连接");
        ServiceGate.connect((JsonObject obj) => {
            ServiceGate.queryEntry(((EntryResponse result) => {
                if (result.code == 200)
                {
                    ProgressHUDManager.showHUD("正在连接服务器");
                    int localPort = LocalDataModel.currentLoginPort;
                    LocalDataModel.host = result.host;
                    LocalDataModel.port = (localPort != 0) ? localPort : result.port;

                    ServiceUser.connect((JsonObject connectResult) => {
                        Debug.Log("连接成功");
                        ProgressHUDManager.hideCurrentHUD();

                        SceneManager.LoadScene("Login");
                    });
                }
            }));
        });
    }


    #region use To debug

    [SerializeField]
    private DebugList debugList;
    [SerializeField]
    private Button testerLoginBtn;

    private float _lastClickTime = 0.0f;
    private int _clickCount = 0;
    

    private void showDebugWin()
    {
        if (debugList != null)
        {
            debugList.gameObject.SetActive(true);
            testerLoginBtn.gameObject.SetActive(true);
        }
    }

    private void debugListDo(HostType type, string host, string port)
    {
        ServiceGate.switchServer(type, host, port);
        testerLoginBtn.gameObject.SetActive(false);
        StartCoroutine(setupNetworkConnect());
    }

    private string timeStempStr
    {
        get
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            string timeStemp = ((int)(DateTime.Now - startTime).TotalSeconds).ToString();
            return timeStemp;
        }
    }

    #endregion
}
