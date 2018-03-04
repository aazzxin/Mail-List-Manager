using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

public enum GameStatus {
	UnLogin,
	Login,
	GamePlaying
}

public class LocalDataModel {

    private static LocalDataModel s_instance;

    private static LocalDataModel getInstance() {
        if (s_instance == null) {
            s_instance = new LocalDataModel();
        }
        return s_instance;
    }

    #region cache data
    #endregion

    #region local data
    public static void Logout() {
        isLogin = false;
        userId = null;
        userName = null;
        sex = null;
        userIconUrl = null;
        host = null;
        port = 0;
    }

    public static bool isLogin {
        set {
            getInstance().setBool("isLogin", value);
        }
        get {
            return getInstance().getBool("isLogin");
        }
    }

    public static string userName {
        set {
            getInstance().setData("userName", value);
        }
        get {
            return getInstance().getData("userName");
        }
    }

    public static string userIconUrl {
        set {
            getInstance().setData("userIconUrl", value);
        }
        get {
            return getInstance().getData("userIconUrl");
        }
    }

    public static string userId {
        set {
            getInstance().setData("userId", value);
        }
        get {
            return getInstance().getData("userId");
        }
    }
    public static string password
    {
        set
        {
            getInstance().setData("password", value);
        }
        get
        {
            return getInstance().getData("password");
        }
    }

    /// <summary>
    /// 1:man 2:woman
    /// </summary>
    /// <value>The user no.</value>
    public static string sex {
        set {
            getInstance().setData("sex", value);
        }
        get {
            return getInstance().getData("sex");
        }
    }

    public static string host {
        set {
            getInstance().setData("host", value);
        }
        get {
            return getInstance().getData("host");
        }
    }

    public static int port {
        set {
            getInstance().setData("port", value.ToString());
        }
        get {
            return System.Convert.ToInt32(getInstance().getData("port"));
        }
    }

    public static string currentHost {
        set {
            getInstance().setData("currentHost", value);
        }
        get {
            return getInstance().getData("currentHost");
        }
    }

    public static int currentLoginPort {
        set {
            getInstance().setInt("currentLoginPort", value);
        }
        get {
            return getInstance().getInt("currentLoginPort");
        }
    }

    #endregion

    #region base set & get

	public static void deleteAllInfo () {
		PlayerPrefs.DeleteAll ();
	}

	public static void setUserInfo (UserModel model) {
		userId = model.userId;
        password = model.password;
        userName = model.nickName;
	}

	private void setFloat (string key, float value) {
		PlayerPrefs.SetFloat (key, value);
	}

	private float getFloat (string key) {
		float value = PlayerPrefs.GetFloat (key);
		return value;
	}

	private void setData(string key, string value) {
		PlayerPrefs.SetString (key, value);
	}

	private string getData(string key) {
		string str = PlayerPrefs.GetString(key);
		return (str == "" || str == null || str.Length <= 0) ? null : str;
	}

	private void setBool (string key, bool value) {
		PlayerPrefs.SetInt (key, value ? 1 : 0);
	}

	private bool getBool (string key) {
		return (PlayerPrefs.GetInt(key) == 1) ? true : false;
	}

	private void setInt (string key, int value) {
		PlayerPrefs.SetInt (key, value);
	}

	private int getInt (string key) {
		return PlayerPrefs.GetInt (key);
	}
		
	#endregion
}
