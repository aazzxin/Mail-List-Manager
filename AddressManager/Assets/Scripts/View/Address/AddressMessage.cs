using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJson;

public class AddressMessage : MonoBehaviour {

    [SerializeField]
    private Button _editorBtn;
    private bool _isEditor;
    [SerializeField]
    private Sprite _toEditor, _toSave;//编辑按钮图片、保存信息按钮图片
    [SerializeField]
    public Sprite _sexSprite_boy, _sexSprite_girl;

    [SerializeField]
    private Image _avatorImage;
    [SerializeField]
    private Button _sex;
    [SerializeField]
    private InputField _nickName, _tel, _email, _remarks;

    [SerializeField]
    private Transform _typeSloganParent;

    public bool isEditor
    {
        get { return _isEditor; }
        set
        {
            if (value)
            {
                _sex.enabled = true;

                _nickName.interactable = true;
                _nickName.targetGraphic.enabled = true;
                _tel.interactable = true;
                _tel.targetGraphic.enabled = true;
                _email.interactable = true;
                _email.targetGraphic.enabled = true;
                _remarks.interactable = true;
                _remarks.targetGraphic.enabled = true;

                _typeSloganParent.GetComponent<Image>().enabled = true;
                _typeSloganParent.GetComponent<CanvasGroup>().blocksRaycasts = true;
                _typeSloganParent.GetComponent<TypeSlot>().enabled = true;
            }
            else
            {
                _sex.enabled = false;

                _nickName.interactable = false;
                _nickName.targetGraphic.enabled = false;
                _tel.interactable = false;
                _tel.targetGraphic.enabled = false;
                _email.interactable = false;
                _email.targetGraphic.enabled = false;
                _remarks.interactable = false;
                _remarks.targetGraphic.enabled = false;

                _typeSloganParent.GetComponent<Image>().enabled = false;
                _typeSloganParent.GetComponent<CanvasGroup>().blocksRaycasts = false;
                _typeSloganParent.GetComponent<TypeSlot>().enabled = false;
            }
            _isEditor = value;
            _editorBtn.image.sprite = value == true ? _toSave : _toEditor;
        }
    }

    private void editorOnClick()
    {
        if (!isEditor)
            isEditor = true;
        else if (updateOrCreateDelegate != null)
            updateOrCreateDelegate();
    }
    private void sexOnClick()
    {
        if(isEditor)
        {
            if (sex == "男")
                sex = "女";
            else if (sex == "女")
                sex = "男";
        }
    }
    public void SetDefault()//设置默认信息
    {
        nickName = "";
        tel = "";
        email = "";
        remarks = "";
        types = new List<ContactTypeModel>();
    }

    public string sex
    {
        get {
            if (_sex.image.sprite == _sexSprite_boy)
            {
                return "男";
            }
            else
                return "女";
        }
        set
        {
            if(value=="男")
            {
                _sex.image.sprite = _sexSprite_boy;
            }
            else if(value=="女")
            {
                _sex.image.sprite = _sexSprite_girl;
            }
        }
    }

    public string nickName
    {
        get { return _nickName.text; }
        set { _nickName.text = value; }
    }
    public string tel
    {
        get { return _tel.text; }
        set { _tel.text = value; }
    }
    public string email
    {
        get { return _email.text; }
        set { _email.text = value; }
    }
    public string remarks
    {
        get { return _remarks.text; }
        set { _remarks.text = value; }
    }
    public List<ContactTypeModel> types
    {
        set {
            for (int i = 0; i < _typeSloganParent.childCount; i++)
            {
                GameObject go = _typeSloganParent.GetChild(i).gameObject;
                Destroy(go);
            }
            foreach(ContactTypeModel type in value)
            {
                GameObject sloganObj = Instantiate(MainMenuController.Instance.typeSloganPfb);
                TypeSlogan slogan = sloganObj.GetComponent<TypeSlogan>();
                slogan.typeName = type.typeName;
                slogan.typeId = type.typeId;
                slogan.SetOutMgr();
                slogan.transform.SetParent(_typeSloganParent);
            }
        }
        get
        {
            List<ContactTypeModel> typeList = new List<ContactTypeModel>();
            for(int i=0;i<_typeSloganParent.childCount;i++)
            {
                JsonObject obj = new JsonObject();
                TypeSlogan slogan = _typeSloganParent.GetChild(i).GetComponent<TypeSlogan>();
                obj.Add("typeName", slogan.typeName);
                obj.Add("typeId", slogan.typeId);
                typeList.Add(new ContactTypeModel(obj));
            }
            return typeList;
        }
    }
    public List<int> newType, deleteType;//信息栏中，新增的标签、移除的标签

    public System.Action updateOrCreateDelegate;

	// Use this for initialization
	void Start () {
        isEditor = false;
        _editorBtn.onClick.AddListener(editorOnClick);
        _sex.onClick.AddListener(sexOnClick);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
