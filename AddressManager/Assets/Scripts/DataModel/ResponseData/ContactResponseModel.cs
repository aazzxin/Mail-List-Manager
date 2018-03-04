using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public class ContactResponseModel : BaseResponse {

    public static string KeyContacts = "contacts";

    public ContactResponseModel()
    {

    }

    public ContactResponseModel(JsonObject obj) : base (obj) {

    }

    public List<NickNameResponseModel> contacts
    {
        get {
            return getList<NickNameResponseModel>(KeyContacts);
        }
    }
}
