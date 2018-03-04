using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public class NickNameResponseModel : BaseResponse {

    private static string KeyContactId = "contactId";
    private static string KeyNickName = "nickName";

    public NickNameResponseModel()
    {

    }

    public NickNameResponseModel(JsonObject obj) : base (obj) {

    }

    public int contactId
    {
        get { return getInt32(KeyContactId); }
    }

    public string nickName
    {
        get { return getString(KeyNickName); }
    }
}
