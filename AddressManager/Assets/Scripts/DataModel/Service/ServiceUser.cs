using UnityEngine;
using System;
using System.Collections;
using SimpleJson;

namespace NetworkModel {

	public class ServiceUser {
		
		public static NetworkBase s_instance;
		public static NetworkBase getInstance () {
			if (s_instance == null) {
				s_instance = NetworkBase.getInstanceService (LocalDataModel.host, LocalDataModel.port);
				s_instance.ResetServiceDelegate += ResetService;
			}
			return s_instance;
		}

		public static void connect(Action<JsonObject> callback) {
			ServiceUser.getInstance().connectToServer ((JsonObject obj) => {
				if (callback != null) {
					callback(obj);
				}
			});
		}

		public static void ResetService () {
			s_instance.ResetServiceDelegate -= ResetService;
			s_instance = null;
		}

        #region 用户
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="userId">UserID.</param>
        /// <param name="password">User password.</param>
        /// <param name="callback">Callback.</param>
        public static void register(string userId,string password,Action<UserModel> callback)
        {
            string route = "connector.connectHandler.register";
            JsonObject userData = new JsonObject();
            userData.Add("userId", userId);
            userData.Add("password", password);

            ServiceUser.getInstance().request(route, userData, (JsonObject obj) =>
              {
                  UserModel userModel = new UserModel(obj);
                  if(userModel.code==200)
                  {
                      LocalDataModel.setUserInfo(userModel);
                  }
                  if(callback!=null)
                  {
                      callback(userModel);
                  }
              });
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="password">User password.</param>
        /// <param name="callback">Callback.</param>
        public static void login (string userId, string password, Action<UserModel> callback) {
			string route = "connector.connectHandler.login";
			JsonObject userData = new JsonObject ();
			userData.Add ("userId", userId);
			userData.Add ("password", password);

			ServiceUser.getInstance().request (route, userData, ((JsonObject obj) => {
				UserModel userModel = new UserModel(obj);
				if (userModel.code == 200) {
                    LocalDataModel.setUserInfo(userModel);
                } else {
					LocalDataModel.isLogin = false;
					LocalDataModel.userId = null;
                    LocalDataModel.password = null;
				}
				if (callback != null) {
					callback(userModel);
				}
			}));
		}
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="password">New password.</param>
        /// <param name="callback">Callback.</param>
        public static void modifyPassword(string password, Action<BaseResponse> callback)
        {
            string route = "connector.connectHandler.modifyPassword";
            JsonObject userData = new JsonObject();
            userData.Add("userId", LocalDataModel.userId);
            userData.Add("password", password);

            ServiceUser.getInstance().request(route, userData, ((JsonObject obj) => {
                BaseResponse responseModel = new BaseResponse(obj);
                if (responseModel.code == 200)
                {
                    LocalDataModel.password = password;
                }
                if (callback != null)
                {
                    callback(responseModel);
                }
            }));
        }
        /// <summary>
        /// 修改昵称
        /// </summary>
        /// <param name="password">New password.</param>
        /// <param name="callback">Callback.</param>
        public static void modifyNickName(string nickName, Action<BaseResponse> callback)
        {
            string route = "connector.connectHandler.modifyNickName";
            JsonObject userData = new JsonObject();
            userData.Add("userId", LocalDataModel.userId);
            userData.Add("nickName", nickName);

            ServiceUser.getInstance().request(route, userData, ((JsonObject obj) => {
                BaseResponse responseModel = new BaseResponse(obj);
                if (responseModel.code == 200)
                {
                    LocalDataModel.userName = nickName;
                }
                if (callback != null)
                {
                    callback(responseModel);
                }
            }));
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="callback">Callback.</param>
        public static void deleteUser(Action<BaseResponse> callback)
        {
            string route = "connector.connectHandler.deleteUser";
            JsonObject userData = new JsonObject();
            userData.Add("userId", LocalDataModel.userId);

            ServiceUser.getInstance().request(route, userData, ((JsonObject obj) => {
                BaseResponse responseModel = new BaseResponse(obj);
                if (responseModel.code == 200) 
                {
                    LocalDataModel.isLogin = false;
                    LocalDataModel.userId = null;
                    LocalDataModel.password = null;
                    LocalDataModel.userName = null;
                }
                if (callback != null)
                {
                    callback(responseModel);
                }
            }));
        }
        #endregion

        #region 联系人
        /// <summary>
        /// 显示联系人列表
        /// </summary>
        /// <param name="callback">Callback.</param>
        public static void showContact( Action<ContactResponseModel> callback)
        {
            searchContact("", "", "", callback);
        }
        /// <summary>
        /// 查询联系人列表
        /// </summary>
        /// <param name="nickName">Nick name user search.</param>
        /// <param name="sex">Sex user search.</param>
        /// <param name="typeName">Type name user search.</param>
        /// <param name="callback">Callback.</param>
        public static void searchContact(string nickName,string sex,string typeName , Action<ContactResponseModel> callback)
        {
            string route = "connector.connectHandler.searchContact";
            JsonObject userData = new JsonObject();
            userData.Add("userId", LocalDataModel.userId);
            userData.Add("nickName", nickName);
            userData.Add("sex", sex);
            userData.Add("typeName", typeName);

            ServiceUser.getInstance().request(route, userData, ((JsonObject obj) => {
                ContactResponseModel contactModel = new ContactResponseModel(obj);
                if (callback != null)
                {
                    callback(contactModel);
                }
            }));
        }

        /// <summary>
        /// 读取id为contactId的联系人
        /// </summary>
        /// <param name="contactId">Contact ID user read.</param>
        /// <param name="callback">Callback.</param>
        public static void readContact(int contactId,Action<AddressResponseModel> callback)
        {
            string route = "connector.connectHandler.readContact";
            JsonObject userData = new JsonObject();
            userData.Add("userId", LocalDataModel.userId);
            userData.Add("contactId", contactId);

            ServiceUser.getInstance().request(route, userData, ((JsonObject obj) => {
                AddressResponseModel addressModel = new AddressResponseModel(obj);
                if (callback != null)
                {
                    callback(addressModel);
                }
            }));
        }
        /// <summary>
		/// 添加联系人
		/// </summary>
        /// <param name="address">Address message.</param>
		/// <param name="callback">Callback.</param>
		public static void addContact(AddressMessage address,Action<ContactResponseModel> callback)
        {
            string route = "connector.connectHandler.addContact";
            JsonObject userData = new JsonObject();
            userData.Add("userId", LocalDataModel.userId);
            userData.Add("sex", address.sex);
            userData.Add("nickName", address.nickName);
            userData.Add("tel", address.tel);
            userData.Add("email", address.email);
            userData.Add("types", address.types);
            userData.Add("remarks", address.remarks);

            ServiceUser.getInstance().request(route, userData, (JsonObject obj) =>
            {
                ContactResponseModel contactModel = new ContactResponseModel(obj);
                if (callback != null)
                {
                    callback(contactModel);
                }
            });
        }
        /// <summary>
		/// 修改联系人
		/// </summary>
        /// <param name="address">Address message.</param>
		/// <param name="callback">Callback.</param>
		public static void updateContact(AddressMessage address, Action<BaseResponse> callback)
        {
            string route = "connector.connectHandler.updateContact";
            JsonObject userData = new JsonObject();
            userData.Add("userId", LocalDataModel.userId);
            userData.Add("contactId", MainMenuController.Instance.selectId);
            userData.Add("sex", address.sex);
            userData.Add("nickName", address.nickName);
            userData.Add("tel", address.tel);
            userData.Add("email", address.email);
            userData.Add("newTypes", address.newType);
            userData.Add("deleteTypes", address.deleteType);
            userData.Add("remarks", address.remarks);

            ServiceUser.getInstance().request(route, userData, (JsonObject obj) =>
            {
                BaseResponse responseModel = new BaseResponse(obj);
                if (callback != null)
                {
                    callback(responseModel);
                }
            });
        }
        /// <summary>
        /// 删除联系人
        /// </summary>
        /// <param name="contactId">Contact ID.</param>
        /// <param name="callback">Callback.</param>
        public static void dropContact(int contactId, Action<BaseResponse> callback)
        {
            string route = "connector.connectHandler.dropContact";
            JsonObject userData = new JsonObject();
            userData.Add("userId", LocalDataModel.userId);
            userData.Add("contactId", contactId);

            ServiceUser.getInstance().request(route, userData, (JsonObject obj) =>
            {
                BaseResponse responseModel = new BaseResponse(obj);
                if (callback != null)
                {
                    callback(responseModel);
                }
            });
        }
        #endregion

        #region 类别
        /// <summary>
        /// 显示类别列表
        /// </summary>
        /// <param name="callback">Callback.</param>
        public static void showType(Action<TypesModel> callback)
        {
            string route = "connector.connectHandler.showType";
            JsonObject userData = new JsonObject();
            userData.Add("userId", LocalDataModel.userId);

            ServiceUser.getInstance().request(route, userData, ((JsonObject obj) => {
                TypesModel typesModel = new TypesModel(obj);
                if (callback != null)
                {
                    callback(typesModel);
                }
            }));
        }
        /// <summary>
		/// 添加类别
		/// </summary>
        /// <param name="typeName">Type name.</param>
		/// <param name="callback">Callback.</param>
		public static void addType(string typeName, Action<BaseResponse> callback)
        {
            string route = "connector.connectHandler.addType";
            JsonObject userData = new JsonObject();
            userData.Add("userId", LocalDataModel.userId);
            userData.Add("typeName", typeName);

            ServiceUser.getInstance().request(route, userData, (JsonObject obj) =>
            {
                BaseResponse responseModel = new BaseResponse(obj);
                if (callback != null)
                {
                    callback(responseModel);
                }
            });
        }
        /// <summary>
		/// 修改类别
		/// </summary>
        /// <param name="typeId">Type ID.</param>
        /// <param name="typeName">Type Name.</param>
		/// <param name="callback">Callback.</param>
		public static void updateType(int typeId,string typeName, Action<BaseResponse> callback)
        {
            string route = "connector.connectHandler.updateType";
            JsonObject userData = new JsonObject();
            userData.Add("userId", LocalDataModel.userId);
            userData.Add("typeId", typeId);
            userData.Add("typeName", typeName);

            ServiceUser.getInstance().request(route, userData, (JsonObject obj) =>
            {
                BaseResponse responseModel = new BaseResponse(obj);
                if (callback != null)
                {
                    callback(responseModel);
                }
            });
        }
        /// <summary>
		/// 删除类别
		/// </summary>
        /// <param name="typeId">Type ID.</param>
		/// <param name="callback">Callback.</param>
		public static void dropType(int typeId, Action<BaseResponse> callback)
        {
            string route = "connector.connectHandler.dropType";
            JsonObject userData = new JsonObject();
            userData.Add("userId", LocalDataModel.userId);
            userData.Add("typeId", typeId);

            ServiceUser.getInstance().request(route, userData, (JsonObject obj) =>
            {
                BaseResponse responseModel = new BaseResponse(obj);
                if (callback != null)
                {
                    callback(responseModel);
                }
            });
        }
        #endregion
    }
}
