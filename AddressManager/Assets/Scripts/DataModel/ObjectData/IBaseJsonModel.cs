using UnityEngine;
using System.Collections;
using SimpleJson;

public interface IBaseJsonModel {
	IBaseJsonModel initialize(JsonObject obj);
}
