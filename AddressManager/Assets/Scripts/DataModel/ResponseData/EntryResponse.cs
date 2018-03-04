using UnityEngine;
using System.Collections;
using SimpleJson;

public class EntryResponse : BaseResponse {

	private static string KeyHost = "host";
	private static string KeyPort = "port";

	public EntryResponse () {

	}

	public EntryResponse (JsonObject obj) : base (obj) {

	}

	public string host  {
		get {
			return getString (KeyHost);
		}
	}

	public int port  {
		get {
			return getInt32 (KeyPort);
		}
	}
}
