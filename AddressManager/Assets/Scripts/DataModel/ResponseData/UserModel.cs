using UnityEngine;
using System.Collections;
using SimpleJson;

public class UserModel : BaseResponse {

	private static string KeyUserId = "userId";
    private static string KeyNickName = "nickName";
    private static string KetPassword = "password";
	private static string KeyHost = "host";
	private static string KeyPort = "port";

	public UserModel () {
		
	}

	public UserModel(JsonObject obj) : base (obj) {
		
	}

	/// <summary>
	/// 用户Id
	/// </summary>
	/// <value>The user identifier.</value>
	public string userId {
		get { 
			return getString (KeyUserId);
		}
	}
    /// <summary>
	/// 用户名称
	/// </summary>
	/// <value>The user nick name.</value>
	public string nickName
    {
        get
        {
            return getString(KeyNickName);
        }
    }
    /// <summary>
	/// 用户密码
	/// </summary>
	/// <value>The user password.</value>
	public string password
    {
        get
        {
            return getString(KetPassword);
        }
    }

    /// <summary>
    /// host
    /// </summary>
    /// <value>The host.</value>
    public string host {
		get { 
			return getString (KeyHost);
		}
	}

	/// <summary>
	/// port
	/// </summary>
	/// <value>The port.</value>
	public int port {
		get { 
			return getInt32 (KeyPort);
		}
	}
}
