using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TipMsgHUD : MonoBehaviour {

	[SerializeField] private Image bgImage;
	[SerializeField] private Text tipText;

	private Action dismissEndDelegate;
	private Animator animator;
	private Coroutine hideCoroutine;

	public string tipMsg {
		get { return tipText.text; }
		set { 
			tipText.text = value; 
			updateBgImageWidth ();
		}
	}

	private void updateBgImageWidth () {
		int strLength = tipMsg.Length;
		float width = 40.0f * (strLength) + 50.0f;
		Vector2 newSize = bgImage.rectTransform.sizeDelta;
		newSize.x = width;
		bgImage.rectTransform.sizeDelta = newSize;
	}

	public void hideAfterDelay (float delay, Action callback) {
		dismissEndDelegate += callback;
		hideCoroutine = StartCoroutine (_startHide (delay));
	}

	private IEnumerator _startHide (float delay) {
		yield return new WaitForSeconds (delay);
		animator.SetBool ("dismiss", true);
	}

	public void tipMsgDidDismiss () {
		if (dismissEndDelegate != null) {
			dismissEndDelegate ();
		}
	}

	void Awake () {
		animator = GetComponent<Animator> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDisable () {
		if (hideCoroutine != null) {
			StopCoroutine (hideCoroutine);
		}
	}
}
