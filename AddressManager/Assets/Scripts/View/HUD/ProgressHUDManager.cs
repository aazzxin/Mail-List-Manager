using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ProgressHUDManager : MonoBehaviour {

	private GameObject _canvas;
	private GameObject _currentWaitingHUD;
	private GameObject _currentComfirmHUD;
	private GameObject _currentTipMsgHUD;
	private GameObject _currentYesOrNoHUD;
	public GameObject WaittingHUDPrefab;
	public GameObject ComfirmHUDPrefab;
	public GameObject TipMsgHUDPrefab;
	public GameObject YesOrNoHUDPrefab;

	private static ProgressHUDManager s_instance;
	private static ProgressHUDManager getInstance () {
		return s_instance;
	}

	public static ProgressHUDManager Instance {
		get { return getInstance(); }
	}

	/// <summary>
	/// 隐藏所有HUD
	/// </summary>
	public static void hideCurrentHUD () {
		hideCurrentHUD (0.0f);
	}
	public static void hideCurrentHUD (float delay) {
		Instance._hideCurrentHUD (delay);
		Instance._hideCurrentYesOrNoHUD (delay);
		Instance._hideCurrentComfirmHUD (delay);
        Instance._hideTipMsgHUD(delay);
    }

	#region Waiting HUD
	public static void showHUD (string text) {
		Debug.Log ("show HUD ：" + text);
		Instance._showHUD (text, false);
	}

	public static void showHUD (string text, bool isHidden) {
		Debug.Log ("show HUD : " + text);
		Instance._showHUD (text, isHidden);
	}

	private void _showHUD (string text, bool isBgHidden) {
		checkCanvas ();
        if (_canvas != null)
        {
            if (_currentWaitingHUD == null)
            {
                _currentWaitingHUD = GameObject.Instantiate(WaittingHUDPrefab);
                _currentWaitingHUD.transform.SetParent(_canvas.transform, false);
            }
            ProgressHUD hud = _currentWaitingHUD.GetComponent<ProgressHUD>();
            hud.isBackgroundHidden = isBgHidden;
            hud.setMsgText (text);
        }
        _canvas = null;
	}

	private void _hideCurrentHUD () {
		_hideCurrentHUD (0.0f);
	}
	private void _hideCurrentHUD (float delay) {
		if (_currentWaitingHUD != null) {
			Destroy (_currentWaitingHUD.gameObject, delay);
			_currentWaitingHUD = null;
		}
	}

    #endregion

    #region Comfirm HUD
    public static void showComfirmHUD(string text)
    {
        Instance._showComfirmHUD(text, null);
    }
    public static void showComfirmHUD(string text, Action callback)
    {
        Instance._showComfirmHUD(text, callback);
    }
    private void _showComfirmHUD(string text, Action callback)
    {
        checkCanvas();
        if (_canvas != null)
        {
            _hideCurrentComfirmHUD();
            _currentComfirmHUD = GameObject.Instantiate(ComfirmHUDPrefab);
            _currentComfirmHUD.transform.SetParent(_canvas.transform, false);
            ComfirmHUD hud = _currentComfirmHUD.GetComponent<ComfirmHUD>();
            hud.msg = text;
            hud.addListender(() =>
            {
                _hideCurrentComfirmHUD();
                if (callback != null)
                {
                    callback();
                }
            });
        }
        _canvas = null;
    }

    private void _hideCurrentComfirmHUD()
    {
        _hideCurrentComfirmHUD(0.0f);
    }
    private void _hideCurrentComfirmHUD(float delay)
    {
        if (_currentComfirmHUD != null)
        {
            Destroy(_currentComfirmHUD.gameObject, delay);
            _currentComfirmHUD = null;
        }
    }

    #endregion

    #region TipMsg HUD
    public static void showTipMsgHUD(string tipMsg)
    {
        Instance._showTipMsgHUD(tipMsg, 2.0f);
    }
    public static void showTipMsgHUD(string tipMsg, float delay)
    {
        Instance._showTipMsgHUD(tipMsg, delay);
    }
    public void _showTipMsgHUD(string tipMsg, float delay)
    {
        checkCanvas();
        if (_canvas != null)
        {
            _hideTipMsgHUD();
            _currentTipMsgHUD = GameObject.Instantiate(TipMsgHUDPrefab);
            _currentTipMsgHUD.transform.SetParent(_canvas.transform, false);
            TipMsgHUD tipMsgHud = _currentTipMsgHUD.GetComponent<TipMsgHUD>();
            tipMsgHud.tipMsg = tipMsg;
            tipMsgHud.hideAfterDelay(delay, () =>
            {
                _hideTipMsgHUD();
            });
        }
        _canvas = null;
    }

    private void _hideTipMsgHUD()
    {
        _hideTipMsgHUD(0.0f);
    }
    private void _hideTipMsgHUD(float delay)
    {
        if (_currentTipMsgHUD != null)
        {
            Destroy(_currentTipMsgHUD.gameObject, delay);
            _currentTipMsgHUD = null;
        }
    }

    #endregion

    #region YesOrNo
    public static void showYesOrNoHUD(string message, Action<bool> callback)
    {
        Instance._showYesOrNoHUD(message, true, callback);
    }

    public static void showYesOrNoHUD(string message, bool canCancel, Action<bool> callback)
    {
        Instance._showYesOrNoHUD(message, canCancel, callback);
    }

    public void _showYesOrNoHUD(string message, bool canCancel, Action<bool> callback)
    {
        checkCanvas();
        if (_canvas != null)
        {
            _hideCurrentYesOrNoHUD();
            _currentYesOrNoHUD = GameObject.Instantiate(YesOrNoHUDPrefab);
            _currentYesOrNoHUD.transform.SetParent(_canvas.transform, false);
            YesOrNoHUD hud = _currentYesOrNoHUD.GetComponent<YesOrNoHUD>();
            hud.canCancel = canCancel;
            hud.message = message;
            hud.yesOrNoCallback = (bool yesOrNo) =>
            {
                _hideCurrentYesOrNoHUD();
                if (callback != null)
                {
                    callback(yesOrNo);
                }
            };
        }
        _canvas = null;
    }

    private void _hideCurrentYesOrNoHUD()
    {
        _hideCurrentYesOrNoHUD(0.0f);
    }
    private void _hideCurrentYesOrNoHUD(float delay)
    {
        if (_currentYesOrNoHUD != null)
        {
            Destroy(_currentYesOrNoHUD.gameObject, delay);
            _currentYesOrNoHUD = null;
        }
    }

    #endregion
    

    private void checkCanvas () {
		GameObject[] canvasArray = GameObject.FindGameObjectsWithTag ("Canvas");
		List<GameObject> canvasList = new List<GameObject> (canvasArray);
		if (canvasList != null && canvasList.Count > 0) {
//			_canvas = GameObject.Find ("Canvas");
			_canvas = canvasList[canvasList.Count - 1];
		}
	}

	void Awake () {
		s_instance = this;
		DontDestroyOnLoad (s_instance.gameObject);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
