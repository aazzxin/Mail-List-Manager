using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ComfirmHUD : MonoBehaviour {

	private Action comfirmDelegate;

	[SerializeField] private Text textMsg;

	private string _msg;
	public string msg {
		get { return _msg; }
		set { 
			_msg = value;
			textMsg.text = _msg;
		}
	} 

	public void comfirmButtonDidPush () {
		if (comfirmDelegate != null) {
			comfirmDelegate ();
		}
	}

	public void addListender (Action callback) {
		comfirmDelegate += callback;
	}

	public void removeListender (Action callback) {
		comfirmDelegate -= callback;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
