using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;

public class NetworkTimmer : MonoBehaviour {

	private static NetworkTimmer s_instance;
	public static NetworkTimmer Instance {
		get { 
			if (s_instance == null) {
				GameObject go = new GameObject ("NetworkTimmer");
				DontDestroyOnLoad (go);
				s_instance = go.AddComponent<NetworkTimmer> ();
				s_instance.reqId = 1;
				s_instance.timeoutDict = new Dictionary<int, Coroutine> ();
			}
			return s_instance;
		}
	}

	private Dictionary<int, Coroutine> timeoutDict;
	private int reqId = 1;
	private static readonly float timeoutTime = 25.0f;

	public static int startTimeoutCounter () {
		return startTimeoutCounter (null);
	}

	public static int startTimeoutCounter (Action<JsonObject> callback) {
		return Instance._startTimeoutCounter (callback);
	} 

	public static void stopTimeoutCounter (int reqId) {
		Instance._stopTimeoutCounter (reqId);
	}

	private int _startTimeoutCounter (Action<JsonObject> callback) {
		int currentId = Instance.reqId++;
		Coroutine counter = StartCoroutine (timeOutCounter (callback));
		Instance.timeoutDict.Add (currentId, counter);	
		return currentId;
	}

	private void _stopTimeoutCounter (int reqId) {
		Coroutine counter = timeoutDict [reqId];
		timeoutDict.Remove (reqId);
		StopCoroutine (counter);
	}

	private IEnumerator timeOutCounter (Action<JsonObject> callback) {
		yield return new WaitForSeconds (timeoutTime);
		ProgressHUDManager.showComfirmHUD ("连接网络超时，请检查网络设置", () => {
			ProgressHUDManager.hideCurrentHUD();
			if (callback != null) {
				JsonObject obj = new JsonObject ();
				obj.Add ("timeout", true);
				callback (obj);
			}
		});
	} 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
