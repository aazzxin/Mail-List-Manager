using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public class ContactTypeModel : BaseModel {

    private static string KeyTypeId = "typeId";
    private static string KeyTypeName = "typeName";

    public ContactTypeModel()
    {

    }

    public ContactTypeModel(JsonObject obj) : base (obj) {

    }

    public int typeId
    {
        get { return getInt32(KeyTypeId); }
    }
    public string typeName
    {
        get { return getString(KeyTypeName); }
    }
}
