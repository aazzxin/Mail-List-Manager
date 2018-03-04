using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;

public class BaseModel : JsonObject, IBaseJsonModel {

	static private string KeyTimeout = "timeout";

	public BaseModel () {

	}

	public BaseModel (JsonObject obj) {
		this.initialize (obj);
	}

	public IBaseJsonModel initialize (JsonObject obj) {
		foreach (KeyValuePair<string, object> item in obj) {
			this.Add (item);
		}
		return this;
	}

	public bool timeout {
		get { 
			return getBool (KeyTimeout);
		}
	}

	public void setValue (string key, object value) {
		if (this.ContainsKey (key)) {
			this.Remove (key);
		}
		this.Add (key, value);
	}

	public string getString (string key) {
		System.Object data = new System.Object ();
		this.TryGetValue (key, out data);
		return data != null ? data.ToString () : null;
	}

	public int getInt32 (string key) {
		System.Object data = new System.Object ();
		this.TryGetValue (key, out data);
		return data != null ? System.Convert.ToInt32 (data) : -10086;
	}

	public bool getBool (string key) {
		System.Object data = new System.Object ();
		this.TryGetValue (key, out data);
		return data != null ? System.Convert.ToBoolean (data) : false;
	}

	public T getObject<T> (string key) where T : IBaseJsonModel, new() {
		JsonObject obj = (JsonObject)this [key];
		T model = new T ();
		model.initialize (obj);
		return model;
	}

	public List<T> getList<T> (string key) where T : IBaseJsonModel, new() {
		List<T> objList = new List<T> ();
		object data = new object ();
		this.TryGetValue(key, out data);
		if (data != null) {
			JsonArray _objects = (JsonArray)data;
			foreach (JsonObject obj in _objects) {
				T model = new T();
				model.initialize (obj);
				objList.Add (model);
			}
		}
		return objList;
	}
}
