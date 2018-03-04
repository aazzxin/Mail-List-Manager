using UnityEngine;
using System;
using System.Collections;
using Pomelo.DotNetClient;
using SimpleJson;

namespace NetworkModel {

	public enum HostType {
		Local,
		Product,
		Development
	}

	public class ServiceGate : NetworkBase {
		public static ServiceGate s_gateServer;
		public static String Host = "127.0.0.1";//"127.0.0.1";
		public static int Port = 3014;

		public static void switchServer (HostType type, string host, string port) {
			switch (type) {
			case HostType.Local:
				LocalDataModel.currentHost = host;
				break;
			case HostType.Product:
				LocalDataModel.currentHost = "127.0.0.1"; 
				break;
			case HostType.Development:
				LocalDataModel.currentHost = "127.0.0.1";
				break;
			}
			if (port != null && port.Length > 0) {
				LocalDataModel.currentLoginPort = System.Convert.ToInt32(port);
			} else {
				LocalDataModel.currentLoginPort = 0;
			}
			s_gateServer = null;
		}

		public static ServiceGate getInstance () {
			if (s_gateServer == null) {
				if (LocalDataModel.currentHost != null) {
					Host = LocalDataModel.currentHost;
				}
				s_gateServer = new ServiceGate (Host, Port);
			}
			return s_gateServer;
		}

		public static void connect(Action<JsonObject> callback) {
			ProgressHUDManager.showHUD ("正在分配服务器");
			ServiceGate.getInstance().connectToServer ((JsonObject obj) => {
				ProgressHUDManager.hideCurrentHUD();
				BaseModel model = new BaseModel(obj); 
				if (model.timeout) {
					reconnectGate (callback);
				} else {
					if (callback != null) {
						callback (obj);
					}	
				}
			});
		}

		private static void reconnectGate (Action<JsonObject> callback) {
			ProgressHUDManager.showYesOrNoHUD ("网络无法连接，点击“确定”重试", (bool yesOrNo) => {
				if (yesOrNo) {
					connect (callback);
				} else {
					ProgressHUDManager.showComfirmHUD("连接中止，重连请重启");
				}
			});
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NetworkModel.GateServer"/> class.
		/// </summary>
		/// <param name="host">Host.</param>
		/// <param name="port">Port.</param>
		public ServiceGate (string host, int port) : base(host, port)
		{

		}

		public static void queryEntry(Action<EntryResponse> callBack) {
			string route = "gate.gateHandler.queryEntry";
			JsonObject param = new JsonObject ();
			ServiceGate.getInstance().request(route, param, ((JsonObject obj) => {
//				Debug.Log("obj = " + obj);
				EntryResponse model = new EntryResponse (obj);
				if (callBack != null) {
					callBack(model);
				}
				ServiceGate.disconnect();
			}));
		}

		public static void disconnect () {
			ServiceGate.getInstance ().pclient.disconnect();
		}


	}
}
