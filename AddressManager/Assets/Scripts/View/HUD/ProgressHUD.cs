using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressHUD : MonoBehaviour {

	[SerializeField] private LoginLoading _loadingHUD;
	[SerializeField] private Image _background;

	[SerializeField] private Color _originBgColor;

    public void setMsgText(string text)
    {
        if (_loadingHUD != null)
        {
            _loadingHUD.msgText = text;
        }
    }

    private bool _isBackgroundHidden = false;
	public bool isBackgroundHidden {
		set { 
			if (_isBackgroundHidden != value) {
				_isBackgroundHidden = value;
				setBackgroundHidden (_isBackgroundHidden);
			}
		}
		get { 
			return _isBackgroundHidden;
		}
	}

	private void setBackgroundHidden (bool isHidden) {
		if (isHidden) {
			_background.color = Color.clear;
		} else {
			_background.color = _originBgColor;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
