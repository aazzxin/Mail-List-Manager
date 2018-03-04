using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using System.ComponentModel;
using Pomelo.DotNetClient;

namespace NetworkModel {

	public class NetworkBase 
	{
        private static NetworkBase s_instanceService = new NetworkBase(LocalDataModel.host, LocalDataModel.port);
		public static NetworkBase getInstanceService(string host, int port) {
			if (s_instanceService == null) {
				s_instanceService = new NetworkBase (host, port);
			}
			return s_instanceService;
		}

		public static NetworkBase Instance { get{ return s_instanceService; }}
		public Action InitSuccessDelegate;
		public Action<JsonObject> ConnectSuccessDelegate;
		public Action<NetWorkState> NetworkChangedDelegate;
		public Action<UserModel> LoginResponseDelegate;
		
		public Action ResetServiceDelegate;

		public PomeloClient pclient { get; private set; }
		public NetWorkState state { get; private set; }
		public string host { get; private set; }
		public int port { get; private set; }

		public bool isReconnecting = false;
		public bool isLogin = false;
		public bool isAtRoom = false;
		public bool isAtRun = false;

		private Dictionary<string, Action<JsonObject>> onMethodList;

		public NetworkBase (string host, int port) {
			this.pclient = new PomeloClient ();
			this.host = host;
			this.port = port;
			this.pclient.NetWorkStateChangedEvent += networkSatateChanged;
		}

		public static void Reset() {
			if (s_instanceService == null)
				return;

			if (s_instanceService.ResetServiceDelegate != null) {
				s_instanceService.ResetServiceDelegate ();
			}
			s_instanceService.InitSuccessDelegate = null;
			s_instanceService.ConnectSuccessDelegate = null;
			s_instanceService.NetworkChangedDelegate = null;
			s_instanceService.LoginResponseDelegate = null;
			
			s_instanceService.pclient.NetWorkStateChangedEvent -= s_instanceService.networkSatateChanged;
			s_instanceService.disconnect ();
			s_instanceService.state = NetWorkState.DISCONNECTED;
			s_instanceService = null;
		}

        public void registerNetworkStateListener(Action<NetWorkState> actionListener)
        {
            NetworkChangedDelegate += actionListener;
        }

        public void removeNetworkStateListner(Action<NetWorkState> actionListener)
        {
            NetworkChangedDelegate -= actionListener;
        }

        private void networkSatateChanged(NetWorkState state)
        {
            this.state = state;
            if (NetworkChangedDelegate != null)
            {
                NetworkChangedDelegate(this.state);
            }
        }

        public void connectToServer () {
			connectToServer (null, null);
		}

		public void connectToServer (JsonObject user) {
			connectToServer (user, null);
		}

		public void connectToServer (Action<JsonObject> callback) {
			connectToServer (null, callback);
		}

		public void connectToServer (JsonObject user, Action<JsonObject> callback) {
            if (this.state != NetWorkState.CONNECTED)
            {
                Debug.Log("正在初始化：host: " + host + " port:" + port);
                int reqId = NetworkTimmer.startTimeoutCounter((JsonObject obj) =>
                {
                    if (callback != null)
                    {
                        callback(obj);
                    }
                });
                Loom.RunAsync(() =>
                {
                    pclient.initClient(host, port, () =>
                    {
                        Loom.QueueOnMainThread(() =>
                        {
                            Debug.Log("初始化成功: host:" + host + " port:" + port);
                            if (InitSuccessDelegate != null)
                            {
                                InitSuccessDelegate();
                            }
                            Debug.Log("开始连接Socket");
                        });
                        pclient.connect(user, (data) =>
                        {
                            Debug.Log("连接Socket成功");
                            Loom.QueueOnMainThread(() =>
                            {
                                NetworkTimmer.stopTimeoutCounter(reqId);
                                // do something with 'data' if need
                                if (ConnectSuccessDelegate != null)
                                {
                                    ConnectSuccessDelegate(data);
                                }
                                if (callback != null)
                                {
                                    callback(data);
                                }
                            });
                        });
                    });
                });
            }
            else
            {
                Loom.QueueOnMainThread(() =>
                {
                    Debug.Log("重复连接：host: " + host + " port:" + port);
                    if (callback != null)
                    {
                        callback(new JsonObject());
                    }
                });
            }
        }


        public void request (string route, JsonObject param, Action<JsonObject> callback) {
            if (state == NetWorkState.CONNECTED)
            {
                Debug.Log("request:" + route + " param = " + param.ToString());
                int reqId = NetworkTimmer.startTimeoutCounter(callback);
                pclient.request(route, param, (data) =>
                {
                    // do something with 'data' if need
                    Loom.QueueOnMainThread(() =>
                    {
                        Debug.Log(route + ": data = " + data);
                        NetworkTimmer.stopTimeoutCounter(reqId);
                        if (callback != null)
                        {
                            try
                            {
                                callback(data);
                            }
                            catch (Exception e)
                            {
                                Debug.Log(e);
                                sendErrToService(route + ":data=" + data.ToString(), e);
                            }
                        }
                    });
                });

            }
            else
            {
                // show the error or try to reconnect
                reconnect(null);
            }
        }

		public void notify (string route, JsonObject param) {
            if (state == NetWorkState.CONNECTED)
            {
                pclient.notify(route, param);
            }
            else
            {
                // show the error or try to reconnect
                reconnect(null);
            }
        }

		public void onMethod (string eventName, Action<JsonObject> callback) {
            pclient.on(eventName, (JsonObject obj) =>
            {
                Loom.QueueOnMainThread(() =>
                {
                    Debug.Log("onPush:" + eventName + "==" + obj.ToString());
                    if (callback != null)
                    {
                        try
                        {
                            callback(obj);
                        }
                        catch (Exception e)
                        {
                            sendErrToService(eventName + ":data=" + obj.ToString(), e);
                        }
                    }
                });
            });
        }

		public void sendErrToService (string err, Exception e) {
			string route = "connector.connectHandler.pushErrMsg";
			JsonObject param = new JsonObject ();
			string errorMsg = err + " ===ErrMsg:" + e.Message + " ===Stack Trace:" + e.StackTrace;
			param.Add ("err", errorMsg);
			param.Add ("userId", LocalDataModel.userId);
			request (route, param, null);
		}

		public void disconnect () {
            pclient.disconnect();
            pclient.NetWorkStateChangedEvent -= networkSatateChanged;
        }

		// 重置pomeloClient
		public void refreshPclient () {
            pclient.NetWorkStateChangedEvent -= networkSatateChanged;
            pclient = new PomeloClient();
            pclient.NetWorkStateChangedEvent += networkSatateChanged;
        }
			
		/// 重连
		public void reconnect (Action<bool> callback) {
			reconnect (callback, true);
		}

		public void reconnect (Action<bool> callback, bool needLogin) {
			if (isReconnecting == true)
				return;
			Loom.QueueOnMainThread (() => {
				ProgressHUDManager.showComfirmHUD ("已断开连接，点击“确认”重连", () => {
					Debug.Log ("开始重连");
					if (Application.internetReachability == NetworkReachability.NotReachable) {
						Debug.Log ("网络不通");
						ProgressHUDManager.showComfirmHUD ("连接失败，请检查网络状态", () => {
							reconnect (callback);	
						});
						return;
					}
					isReconnecting = true;
                    if (pclient != null)
                    {
                        refreshPclient();
                    }

                    ProgressHUDManager.showHUD ("正在重新连接");	
					connectToServer ((JsonObject obj) => {
						BaseModel model = new BaseModel(obj);
						if (model.timeout) {	// 判断连接超时
							isReconnecting = false;
							reconnect (callback);
							return;
						}
						Debug.Log ("重新连接结束, 正在尝试登陆");
						ServiceUser.login(LocalDataModel.userId, LocalDataModel.password,(UserModel user) => {
							if (user.code == 200) {
								Debug.Log ("登陆成功");
								notifyLoginComplete (user);
								
							} else {
								Debug.Log ("登陆失败");
								safeCallCallback(callback, false);
							}
						});
					});		
				});
			});
		}
			
		private void notifyLoginComplete (UserModel model) {
			if (LoginResponseDelegate != null) {
				Debug.Log ("Reconnect:登陆完成通知");
				LoginResponseDelegate (model);
			}
		}

		private void safeCallCallback (Action<bool> callback, bool result) {
			ProgressHUDManager.hideCurrentHUD ();
			isReconnecting = false;
			if (callback != null) {
				callback (result);
			}
		}

//		public static void uploadFile () {
//			Mac mac = new Mac ("9fcqe5c-ItxWkakHm5u06Zb10xqmV19Zo7rz8wQU", "lMXApO5gNXZx34Y-ebu3BSh_tKz7lvvb6IMU_me4");
//
//			string bucket = "gamerunfast";
//			string saveKey = "1.png";
//			string localFile = "D:\\QFL\\1.png";
//
//			// 上传策略，参见 
//			// http://developer.qiniu.com/article/developer/security/put-policy.html
//			PutPolicy putPolicy = new PutPolicy();
//
//			// 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
//			// putPolicy.Scope = bucket + ":" + saveKey;
//			putPolicy.Scope = bucket;
//
//			// 上传策略有效期(对应于生成的凭证的有效期)          
//			putPolicy.setExpires(3600);
//
//			// 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
//			putPolicy.DeleteAfterDays = 1;
//
//			// 生成上传凭证，参见
//			// http://developer.qiniu.com/article/developer/security/upload-token.html            
//			string token = UploadManager.createUploadToken(mac, putPolicy);
//
//			SimpleUploader su = new SimpleUploader();
//
//			// 支持自定义参数
//			//var extra = new System.Collections.Generic.Dictionary<string, string>();
//			//extra.Add("FileType", "UploadFromLocal");
//			//extra.Add("YourKey", "YourValue");
//			//uploadFile(...,extra)
//
//			HttpResult result = su.uploadFile(localFile, saveKey, token);
//
//			Console.WriteLine(result);
//		}
	}
}