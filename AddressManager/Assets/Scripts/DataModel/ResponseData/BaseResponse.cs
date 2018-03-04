using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;

public class BaseResponse : BaseModel {

	public static string KeyCode = "code";
	public static string KeyMsg = "msg";
	public static string KeyMessage = "message";

	public BaseResponse () {
		
	}

	public BaseResponse (JsonObject obj) : base (obj){
		
	}

	public int code {
		get {
			System.Object data = new System.Object ();
			this.TryGetValue (KeyCode, out data);
			return System.Convert.ToInt32 (data);
		}
	}

	public string msg {
		get { 
			System.Object data = new System.Object ();
			this.TryGetValue (KeyMessage, out data);
			if (data == null) {
				this.TryGetValue (KeyMsg, out data);
			}
			return (data == null) ? null : data.ToString ();
		}
	}

//	public string getString (string key) {
//		System.Object data = new System.Object ();
//		this.TryGetValue (key, out data);
//		return data != null ? data.ToString () : "";
//	}
//
//	public int getInt32 (string key) {
//		System.Object data = new System.Object ();
//		this.TryGetValue (key, out data);
//		return data != null ? System.Convert.ToInt32 (data) : 0;
//	}
}
