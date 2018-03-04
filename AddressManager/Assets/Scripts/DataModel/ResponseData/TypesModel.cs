using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public class TypesModel : BaseResponse {

    private static string KeyTypes = "types";

    public TypesModel()
    {

    }

    public TypesModel(JsonObject obj) : base (obj) {

    }

    public List<ContactTypeModel> types
    {
        get { return getList<ContactTypeModel>(KeyTypes); }
    }
}
