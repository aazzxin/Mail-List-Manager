using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;

public class AddressResponseModel : BaseResponse {

    private static string KeyNickName = "nickName";
    private static string KeySex = "sex";
    private static string KeyTel = "tel";
    private static string KeyEmail = "email";
    private static string KeyTypes = "types";
    private static string KeyRemarks = "remarks";

    public AddressResponseModel()
    {

    }

    public AddressResponseModel(JsonObject obj) : base (obj) {

    }

    public string nickName
    {
        get { return getString(KeyNickName); }
    }
    public string sex
    {
        get { return getString(KeySex); }
    }
    public string tel
    {
        get { return getString(KeyTel); }
    }
    public string email
    {
        get { return getString(KeyEmail); }
    }
    public List<ContactTypeModel> types
    {
        get { return getList<ContactTypeModel>(KeyTypes); }
    }
    public string remarks
    {
        get { return getString(KeyRemarks); }
    }
}
