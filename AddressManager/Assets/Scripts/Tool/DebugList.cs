using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugList : MonoBehaviour {

	public System.Action<NetworkModel.HostType, string, string> debugListDelegate; 
	public InputField inputField;
	public InputField portField;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void productBtnDo () {
		if (debugListDelegate != null) {
			debugListDelegate (NetworkModel.HostType.Product, null, portField.text);
		}
		gameObject.SetActive (false);
	}

	public void developmentBtnDo () {
		if (debugListDelegate != null) {
			debugListDelegate (NetworkModel.HostType.Development, null, portField.text);
		}
		gameObject.SetActive (false);
	}

	public void customBtnDo () {
		if (debugListDelegate != null) {
			string host = inputField.text;
			string loginPort = portField.text;
			debugListDelegate (NetworkModel.HostType.Local, host, loginPort);
		}
		gameObject.SetActive (false);
	}


}
