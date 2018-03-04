using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class TypeSlogan : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {

    [SerializeField]
    private InputField _typeName;
    private string _protoName;//编辑前的类别名,失败时还原
    public string typeName
    {
        get { return _typeName.text; }
        set { _typeName.text = value; _protoName = value; }
    }
    public int typeId = -1;
    [SerializeField]
    private Button _updateBtn;
    private bool _isEditor = false;
    public bool isEditor
    {
        get { return _isEditor; }
        set {
            _isEditor = value;
            if (value)
            {
                _updateBtn.image.enabled = (false);
                _typeName.GetComponent<CanvasGroup>().blocksRaycasts = true;
                //_typeName.interactable = true;
                _typeName.targetGraphic.enabled = true;
            }
            else
            {
                _updateBtn.image.enabled = (true);
                _typeName.GetComponent<CanvasGroup>().blocksRaycasts = false;
                //_typeName.interactable = false;
                _typeName.targetGraphic.enabled = false;
            }
        }
    }

    public bool isInMgr = true;//是否在标签管理中，否则即在左侧联系人信息栏中。
    public void SetDefault()
    {
        Destroy(_updateBtn);
        typeName = "+";
        this.GetComponent<EventTrigger>().triggers.Clear();
    }
    public void SetOutMgr()//设置为非标签管理器中的标签
    {
        Destroy(_updateBtn);
        isInMgr = false;
        onClickSloganDelegate = null;
        onEndEditDelegate = null;
        onSlotEndDelegate = null;
        onDropEndDelegate = MainMenuController.Instance.deleteType;
    }
    
    private void onClickUpdate()//点击编辑按钮
    {
        isEditor = true;
        _typeName.Select();
    }
    #region 委托
    public System.Action<TypeSlogan> onClickSloganDelegate;
    private void onClickSlogan()//点击标签
    {
        if (onClickSloganDelegate != null)
            onClickSloganDelegate(this);
    }

    public System.Action<TypeSlogan> onEndEditDelegate;
    private void onChangedValue(string value)//输入域更改时
    {
        if (onEndEditDelegate != null)
            onEndEditDelegate(this);
        isEditor = false;
    }
    public System.Action<TypeSlogan> onSlotEndDelegate;
    private void onSlotEnd()//拖动丢弃之后
    {
        if (onSlotEndDelegate != null)
            onSlotEndDelegate(this);
    }
    public System.Action<int> onDropEndDelegate;
    private void onDropEnd()//拖动丢弃之后
    {
        if (onDropEndDelegate != null)
            onDropEndDelegate(typeId);
    }
    #endregion

    // Use this for initialization
    void Start () {
        
        GetComponent<Button>().onClick.AddListener(onClickSlogan);
        _updateBtn.onClick.AddListener(onClickUpdate);
        _typeName.onEndEdit.AddListener(onChangedValue);
	}

    #region 鼠标事件
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_updateBtn != null)
            _updateBtn.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_updateBtn != null)
            _updateBtn.gameObject.SetActive(false);
    }

    public Transform slot;//将在位置
    private Transform protoSlot;//原来的位置
    private int protoSibling;//原来位于第几号位置
    public void OnBeginDrag(BaseEventData eventData)
    {
        PointerEventData data = (PointerEventData)eventData;
        slot = null;
        protoSlot = this.transform.parent;
        protoSibling = this.transform.GetSiblingIndex();
        this.transform.position = data.position;
        this.transform.SetParent(MainMenuController.Instance.canvasTrans);

        if (isInMgr)
        {
            GameObject cloneSlogan = Instantiate(this.gameObject);
            cloneSlogan.gameObject.name = this.gameObject.name;
            cloneSlogan.transform.SetParent(protoSlot);
            cloneSlogan.transform.SetSiblingIndex(protoSibling);
            TypeSlogan slogan = cloneSlogan.GetComponent<TypeSlogan>();
            slogan.onClickSloganDelegate = this.onClickSloganDelegate;
            slogan.onEndEditDelegate = this.onEndEditDelegate;
            slogan.onSlotEndDelegate = this.onSlotEndDelegate;
            slogan.onDropEndDelegate = this.onDropEndDelegate;
        }

        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(BaseEventData eventData)
    {
        PointerEventData data = (PointerEventData)eventData;
        this.transform.position = data.position;
    }

    public void OnEndDrag(BaseEventData eventData)
    {
        if(isInMgr)
        {
            if(slot != null&& slot != protoSlot)
            {
                onSlotEnd();
                this.transform.SetParent(slot);
                this.GetComponent<CanvasGroup>().blocksRaycasts = true;
                SetOutMgr();
            }
            else if(slot != null && slot == protoSlot)
                Destroy(this.gameObject);
            else
            {
                onDropEnd();
                Destroy(this.gameObject);
            }
        }
        else
        {
            if (slot != null && slot == protoSlot)
            {
                this.transform.SetParent(slot);
                this.transform.SetSiblingIndex(protoSibling);
                this.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            else
            {
                onDropEnd();
                Destroy(this.gameObject);
            }
        }
    }
    #endregion

    public void ReturnName()
    {
        typeName = _protoName;
    }

   
}
