using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class Serializable : MonoBehaviour {

    internal virtual JSONObject Serialize()
    {
        JSONObject rootObj = new JSONObject();
        rootObj.Add("name", transform.name);

        JSONObject pos = new JSONObject();
        pos.Add("x", transform.position.x);
        pos.Add("y", transform.position.y);
        pos.Add("z", transform.position.z);

        JSONObject rot = new JSONObject();
        rot.Add("x", transform.eulerAngles.x);
        rot.Add("y", transform.eulerAngles.y);
        rot.Add("z", transform.eulerAngles.z);

        rootObj.Add("position", pos);
        rootObj.Add("rotation", rot);

        return rootObj;
    }

    internal virtual void Deserialize(JSONObject jsonObj)
    {
        Vector3 pos = new Vector3(jsonObj["position"]["x"].AsInt,
                                jsonObj["position"]["y"].AsInt,
                                jsonObj["position"]["z"].AsInt);
        Vector3 rot = new Vector3(jsonObj["rotation"]["x"].AsInt,
                                jsonObj["rotation"]["y"].AsInt,
                                jsonObj["rotation"]["z"].AsInt);

        transform.position = pos;
        transform.rotation = Quaternion.Euler(rot);
    }
}
