using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkModel;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    private static MainMenuController s_instance;
    public static MainMenuController Instance
    {
        get {
            return s_instance;
        }
    }

    public Transform canvasTrans;

    [SerializeField]
    private InputField _userNickName;
    [SerializeField]
    private Button _modifyNameBtn, _logOffBtn, _modifyPwBtn, _deleteUserBtn;
    [SerializeField]
    private PasswordPanel _passwordPanel;

    public Sprite defaultCell, activeCell;

    [SerializeField]
    private Button _typesBtn,_searchBtn;
    [SerializeField]
    private ScrollRect _contactCellRect, _typesSloganRect;
    private float _cellRectHeight, _sloganRectHeight;
    [SerializeField]
    private GameObject _contactCellPfb;

    public GameObject typeSloganPfb;

    public AddressMessage addressMessage;

    private bool isOpenType = false, isOpenPwPanel = false;
    public InputField searchTypeName, searchNickName;
    public Dropdown searchSex;

    public int selectId = -1;//当前选中的联系人的ID


    private void Awake()
    {
        s_instance = this;
    }

    // Use this for initialization
    void Start () {
        _userNickName.text = LocalDataModel.userName;
        _userNickName.onEndEdit.AddListener(modifyNickName);
        _modifyNameBtn.onClick.AddListener(onClickEditorName);
        _logOffBtn.onClick.AddListener(logOff);
        _modifyPwBtn.onClick.AddListener(openPasswordPanel);
        _deleteUserBtn.onClick.AddListener(deleteUser);
        _typesBtn.onClick.AddListener(openTypeRect);
        _searchBtn.onClick.AddListener(onClickSearch);
        _passwordPanel.modifyPasswordDelegate = modifyPassword;

        _cellRectHeight = _contactCellRect.content.GetComponent<RectTransform>().sizeDelta.y / 3;
        _sloganRectHeight = _typesSloganRect.content.GetComponent<RectTransform>().sizeDelta.y / 3;

        addressMessage.SetDefault();
        addressMessage.updateOrCreateDelegate = addContact;
        addressMessage.newType = new List<int>();
        addressMessage.newType = new List<int>();

        showType();
        ShowContactCell();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    #region 用户管理
    private bool _editorName;
    private bool editorName
    {
        get { return _editorName; }
        set
        {
            if(value)
            {
                _userNickName.interactable = true;
                _userNickName.targetGraphic.enabled = true;
            }
            else
            {
                _userNickName.interactable = false;
                _userNickName.targetGraphic.enabled = false;
            }
            _editorName = value;
        }
    }
    private void onClickEditorName()
    {
        modifyNickName(_userNickName.text);
    }
    private void modifyNickName(string nickName)
    {
        if(!_editorName)
        {
            editorName = true;
            _userNickName.Select();
        }
        else
        {
            ServiceUser.modifyNickName(nickName, (responseObj) => {
                if (responseObj.code == 200)
                {
                    ProgressHUDManager.showTipMsgHUD(responseObj.msg);
                    editorName = false;
                }
                else
                {
                    ProgressHUDManager.showComfirmHUD(responseObj.msg);
                }
            });
        }
    }
    private void logOff()
    {
        ProgressHUDManager.showYesOrNoHUD("确认注销账户吗？", (yes) =>
        {
            if (yes)
            {
                LocalDataModel.userName = null;
                LocalDataModel.password = null;
                LocalDataModel.isLogin = false;
                SceneManager.LoadScene("Login");
            }
        });
    }
    private void openPasswordPanel()
    {
        if (!isOpenPwPanel)
            _passwordPanel.gameObject.SetActive(true);
        else
            _passwordPanel.gameObject.SetActive(false);
        isOpenPwPanel = !isOpenPwPanel;
    }
    private void modifyPassword(string password)
    {
        ServiceUser.modifyPassword(password, (responseObj) => {
            if (responseObj.code == 200)
            {
                ProgressHUDManager.showTipMsgHUD(responseObj.msg);
                _passwordPanel.gameObject.SetActive(false);
                isOpenPwPanel = false;
            }
            else
            {
                ProgressHUDManager.showComfirmHUD(responseObj.msg);
            }
        });
    }
    private void deleteUser()
    {
        ProgressHUDManager.showYesOrNoHUD("确认删除账户吗？", (yes) =>
        {
            if (yes)
            {
                ServiceUser.deleteUser((responseObj) => {
                    if (responseObj.code == 200)
                    {
                        ProgressHUDManager.showComfirmHUD("删除成功，即将登出界面",()=> {
                            SceneManager.LoadScene("Login");
                        });
                    }
                    else
                    {
                        ProgressHUDManager.showComfirmHUD(responseObj.msg);
                    }
                });
            }
        });
    }
    #endregion

    #region 标签管理
    private void openTypeRect()
    {
        if(isOpenType)
            _typesSloganRect.gameObject.SetActive(false);
        else
            _typesSloganRect.gameObject.SetActive(true);
        isOpenType = !isOpenType;
    }
    private void showType()
    {
        ServiceUser.showType((typeObj)=>
        {
            if (typeObj.code == 200)
            {
                for (int i = 0; i < _typesSloganRect.content.childCount; i++)
                {
                    GameObject go = _typesSloganRect.content.GetChild(i).gameObject;
                    Destroy(go);
                }
                int row = typeObj.types.Count / 5;
                if (row >= 3)
                {
                    _typesSloganRect.content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, _sloganRectHeight * row);
                }
                else
                    _typesSloganRect.content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, _sloganRectHeight);
                foreach (ContactTypeModel typeModel in typeObj.types)
                {
                    GameObject sloganObj = Instantiate(typeSloganPfb);
                    TypeSlogan slogan = sloganObj.GetComponent<TypeSlogan>();
                    slogan.typeName = typeModel.typeName;
                    slogan.typeId = typeModel.typeId;
                    slogan.onClickSloganDelegate = setSearchTypeName;
                    slogan.onEndEditDelegate = updateSlogan;
                    slogan.onSlotEndDelegate = newType;
                    slogan.onDropEndDelegate = dropType;
                    sloganObj.transform.SetParent(_typesSloganRect.content);
                }
                GameObject defaultSloganObj = Instantiate(typeSloganPfb);
                TypeSlogan defaultSlogan = defaultSloganObj.GetComponent<TypeSlogan>();
                defaultSlogan.onClickSloganDelegate = CreateSlogan;
                defaultSlogan.onEndEditDelegate = addType;
                defaultSlogan.SetDefault();
                defaultSloganObj.transform.SetParent(_typesSloganRect.content);
            }
            else
            {
                ProgressHUDManager.showComfirmHUD(typeObj.msg);
            }
        });
    }
    public void CreateSlogan(TypeSlogan slogan)
    {
        if (_typesSloganRect.content.childCount > 101) 
        {
            ProgressHUDManager.showComfirmHUD("类别已满上限，请清除无用类别");
            return;
        }
        slogan.typeName = "";
        slogan.isEditor = true;
    }
    private void setSearchTypeName(TypeSlogan slogan)
    {
        searchTypeName.text = slogan.typeName;
    }
    private void dropType(int typeId)//删除类别表中的某类别
    {
        ProgressHUDManager.showYesOrNoHUD("确认删除标签吗？", (yes) =>
        {
            if(yes)
            {
                ServiceUser.dropType(typeId, (responseObj) => {
                    if (responseObj.code == 200)
                    {
                        ProgressHUDManager.showTipMsgHUD(responseObj.msg);
                        showType();
                    }
                    else
                    {
                        ProgressHUDManager.showComfirmHUD(responseObj.msg);
                    }
                });
            }
        });
    }
    public void deleteType(int typeId)//删除联系人的某类别
    {
        foreach (int i in addressMessage.newType)
        {
            if (typeId == i)//若标签之前添加过
            {
                addressMessage.newType.Remove(i);
                return;
            }
        }
        addressMessage.deleteType.Add(typeId);
    }
    private void newType(TypeSlogan slogan)//为左侧信息栏标签添加标签
    {
        foreach(ContactTypeModel model in addressMessage.types)
        {
            int i = model.typeId;
            if (slogan.typeId == i)//若已经存在在信息栏中
            {
                Destroy(slogan.gameObject);
                return;
            }
        }
        foreach (int i in addressMessage.deleteType)
        {
            if (slogan.typeId == i)//若同样的标签之前被删除过
            {
                addressMessage.deleteType.Remove(i);
                break;
            }
        }
        slogan.SetOutMgr();
        addressMessage.newType.Add(slogan.typeId);
    }
    private void updateSlogan(TypeSlogan slogan)
    {
        ServiceUser.updateType(slogan.typeId, slogan.typeName, (responseObj) => {
            if (responseObj.code == 200)
            {
                ProgressHUDManager.showTipMsgHUD(responseObj.msg);
                if (selectId != -1)
                    readContact(selectId);
            }
            else
            {
                ProgressHUDManager.showComfirmHUD(responseObj.msg);
                slogan.ReturnName();
            }
        });
    }
    private void addType(TypeSlogan slogan)//为标签管理添加新的标签
    {
        ServiceUser.addType(slogan.typeName, (responseObj) => {
            if (responseObj.code == 200)
            {
                ProgressHUDManager.showTipMsgHUD(responseObj.msg);
                showType();
            }
            else
            {
                ProgressHUDManager.showComfirmHUD(responseObj.msg);
                slogan.SetDefault();
                slogan.isEditor = false;
            }
        });
    }
    #endregion

    #region 联系人数据管理
    //点击查询
    private void onClickSearch()
    {
        string sex = "";
        if (searchSex.captionText.text != "全部")
            sex = searchSex.captionText.text;
        SearchContact(searchNickName.text, sex, searchTypeName.text);
    }
    //查询联系人列表
    public void SearchContact(string nickName, string sex, string typeName)
    {
        ServiceUser.searchContact(nickName,sex,typeName,(contactObj) => {
            if (contactObj.code == 200)
            {
                for (int i = 0; i < _contactCellRect.content.childCount; i++)
                {
                    GameObject go = _contactCellRect.content.GetChild(i).gameObject;
                    Destroy(go);
                }
                foreach (NickNameResponseModel nickNameModel in contactObj.contacts)
                {
                    GameObject cell = Instantiate(_contactCellPfb);
                    ContactCell contactCell = cell.GetComponent<ContactCell>();
                    contactCell.Init(nickNameModel.contactId, nickNameModel.nickName, readContact, deleteContact);
                    cell.transform.SetParent(_contactCellRect.content);
                }
                GameObject defaultCell = Instantiate(_contactCellPfb);
                ContactCell defaultContactCell = defaultCell.GetComponent<ContactCell>();
                defaultContactCell.onClickCellDelegate = (id) => { CreateCell(); };
                defaultContactCell.SetDefault();
                defaultCell.transform.SetParent(_contactCellRect.content);
            }
            else
            {
                ProgressHUDManager.showComfirmHUD(contactObj.msg);
            }
        });
    }
    //显示联系人列表
    public void ShowContactCell()
    {
        ServiceUser.showContact((contactObj)=> {
            if (contactObj.code == 200)
            {
                for (int i = 0; i < _contactCellRect.content.childCount; i++)
                {
                    GameObject go = _contactCellRect.content.GetChild(i).gameObject;
                    Destroy(go);
                }
                int row = contactObj.contacts.Count / 5;
                if (row >= 3) 
                {
                    _contactCellRect.content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, _cellRectHeight * row);
                }
                else
                    _contactCellRect.content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, _cellRectHeight);
                foreach (NickNameResponseModel nickNameModel in contactObj.contacts)
                {
                    GameObject cell = Instantiate(_contactCellPfb);
                    ContactCell contactCell = cell.GetComponent<ContactCell>();
                    contactCell.Init(nickNameModel.contactId, nickNameModel.nickName, readContact, deleteContact);
                    cell.transform.SetParent(_contactCellRect.content);
                }
                GameObject defaultCell = Instantiate(_contactCellPfb);
                ContactCell defaultContactCell = defaultCell.GetComponent<ContactCell>();
                defaultContactCell.onClickCellDelegate = (id) => { CreateCell(); };
                defaultContactCell.SetDefault();
                defaultCell.transform.SetParent(_contactCellRect.content);
            }
            else
            {
                ProgressHUDManager.showComfirmHUD(contactObj.msg);
            }
        });
    }
    public void CreateCell()
    {
        if (_contactCellRect.content.childCount > 1001) 
        {
            ProgressHUDManager.showComfirmHUD("联系人已满上限，请清除无用联系人");
            return;
        }
        addressMessage.SetDefault();
        addressMessage.isEditor = true;
        addressMessage.updateOrCreateDelegate = addContact;
        selectId = 0;
    }
    private void deleteContact(ContactCell cell)
    {
        ProgressHUDManager.showYesOrNoHUD("确认删除该联系人吗？", (yes) =>
        {
            if (yes)
            {
                ServiceUser.dropContact(cell.contactId, (responseObj) => {
                    if (responseObj.code == 200)
                    {
                        ProgressHUDManager.showTipMsgHUD(responseObj.msg);
                        Destroy(cell.gameObject);
                    }
                    else
                    {
                        ProgressHUDManager.showComfirmHUD(responseObj.msg);
                    }
                });
            }
        });
    }
    private void readContact(int contactId)
    {
        ServiceUser.readContact(contactId,(addressObj) => {
            if (addressObj.code == 200)
            {
                addressMessage.isEditor = false;

                addressMessage.nickName = addressObj.nickName;
                addressMessage.sex = addressObj.sex;
                addressMessage.tel = addressObj.tel;
                addressMessage.email = addressObj.email;
                addressMessage.types = addressObj.types;
                addressMessage.remarks = addressObj.remarks;
                addressMessage.updateOrCreateDelegate = updateContact;
                addressMessage.newType = new List<int>();
                addressMessage.deleteType = new List<int>();
                selectId = contactId;
            }
            else
            {
                ProgressHUDManager.showComfirmHUD(addressObj.msg);
            }
        });
    }
    private void updateContact()
    {
        ServiceUser.updateContact(addressMessage, (responseObj) => {
            if (responseObj.code == 200)
            {
                addressMessage.isEditor = false;

                ShowContactCell();
                ProgressHUDManager.showTipMsgHUD(responseObj.msg);
                
                addressMessage.newType = new List<int>();
                addressMessage.deleteType = new List<int>();

            }
            else
            {
                ProgressHUDManager.showComfirmHUD(responseObj.msg);
            }
        });
    }
    private void addContact()
    {
        ServiceUser.addContact(addressMessage, (contactObj) => {
            if (contactObj.code == 200)
            {
                addressMessage.isEditor = false;
                selectId = contactObj.contacts[0].contactId;

                ProgressHUDManager.showTipMsgHUD(contactObj.msg);
                ShowContactCell();

                addressMessage.updateOrCreateDelegate = updateContact;
            }
            else
            {
                ProgressHUDManager.showComfirmHUD(contactObj.msg);
            }
        });
    }
    #endregion
}
