using Newtonsoft.Json.Linq;
using UnityEngine;

public class MultiplayerObject : MonoBehaviour
{
    public string _id { get; private set; }
    public string playerName = "Player 1";
    public void Start()
    {
        _id = System.Guid.NewGuid().ToString();
    }
    public JObject AsJson()
    {
        JObject obj = new JObject();
        obj["_id"] = _id;
        obj["playerName"] = playerName;
        obj["tag"] = gameObject.tag;

        obj["transform"] = new JObject();
        obj["transform"]["position"] = new JArray(
            transform.position.x,
            transform.position.y,
            transform.position.z
        );
        obj["transform"]["eulerAngles"] = new JArray(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            transform.eulerAngles.z
        );

        return obj;
    }
}
