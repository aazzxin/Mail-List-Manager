using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TypeSlot : MonoBehaviour,IDropHandler {
    public void OnDrop(PointerEventData eventData)
    {
        TypeSlogan slogan = eventData.pointerDrag.GetComponent<TypeSlogan>();
        slogan.slot = slotTrans;
    }

    [SerializeField]
    private Transform slotTrans;

    private void Start()
    {

    }
}
