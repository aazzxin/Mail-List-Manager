using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ContactCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField]
    private Button _cellBtn;
    [SerializeField]
    private Button _deleteBtn;
    [SerializeField]
    private Text _nickName;

    public Image cellImage
    {
        get { return _cellBtn.image; }
        private set { _cellBtn.image = value; }
    }

    public int contactId;

    public string nickName
    {
        get { return _nickName.text;}
        set { _nickName.text = value; }
    }


    public void Init(int contactId,string nickName, System.Action<int> cellDelegate, System.Action<ContactCell> deleteDelegate)
    {
        this.contactId = contactId;
        this.nickName = nickName;
        _nickName.gameObject.SetActive(true);
        cellImage.sprite = MainMenuController.Instance.activeCell;
        //随机取色块,
        float h=0x00, s=0x00, v=0x00;
        h = Random.Range(0, 359);
        h /= 360;
        s = Random.Range(80, 200);
        s /= 256;
        v = Random.Range(155, 210);
        v /= 256;
        this.cellImage.color = Color.HSVToRGB(h, s, v);
        onClickCellDelegate = cellDelegate;
        onClickDeleteDelegate = deleteDelegate;
    }
    public void SetDefault()
    {
        Destroy(_deleteBtn);
    }
    private byte[] IntToByteArray(int n)
    {
        byte[] b = new byte[4];
        b[0] = (byte)(n & 0xff);
        b[1] = (byte)(n >> 8 & 0xff);
        b[2] = (byte)(n >> 16 & 0xff);
        b[3] = (byte)(n >> 24 & 0xff);
        return b;
    }
    public System.Action<int> onClickCellDelegate;

    private void OnClickCell()
    {
        if (onClickCellDelegate != null)
            onClickCellDelegate(contactId);
    }
    public System.Action<ContactCell> onClickDeleteDelegate;
    private void onClickDelete()//点击删除
    {
        if (onClickDeleteDelegate != null)
            onClickDeleteDelegate(this);
    }

    // Use this for initialization
    void Start () {
        _cellBtn.onClick.AddListener(OnClickCell);
        _deleteBtn.onClick.AddListener(onClickDelete);
    }

    #region 鼠标事件
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_deleteBtn != null)
            _deleteBtn.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_deleteBtn != null)
            _deleteBtn.gameObject.SetActive(false);
    }
    #endregion
}
