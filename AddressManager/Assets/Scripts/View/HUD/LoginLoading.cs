using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LoginLoading : MonoBehaviour {

	[SerializeField] private Image icon;
	[SerializeField] private Text MessageText;
	[SerializeField] private List<Sprite>iconImage;

	private string _msgText = "请稍后";
	private int amount;
	private int current;

	void Awake () {
		amount = iconImage.Count;
		current = 0;
		//setIcon (current);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void changeIamge () {
		current++;
		if (current >= amount) {
			current = 0;
		}
		setIcon (current);
		updateMsgTextDot (current);
	}

	private void setIcon (int index) {
		Sprite currentIcon = null;
		try {
			currentIcon = iconImage[index];
		} catch {
			currentIcon = null;
		} finally {
			icon.sprite = currentIcon;
		} 
	}

	public string msgText {
		get { return _msgText; }
		set { 
			_msgText = value; 
			setText (_msgText);
		}
	}

	private void updateMsgTextDot (int dotCount) {
		string msg = msgText;
		for (int i = 0; i <= dotCount; i++) {
			msg += ".";
		}
		setText (msg);
	}

	private void setText(string text) {
		MessageText.text = text;
	}
}
