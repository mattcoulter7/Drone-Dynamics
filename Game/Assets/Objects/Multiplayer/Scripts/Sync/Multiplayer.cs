using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketRunner;

public class Multiplayer : MonoBehaviour
{
    [SerializeField] private GameObject multiplayerPrefab;
    private Connection connection = null;
    private MultiplayerObject[] multiplayerObjects;

    private Dictionary<string, GameObject> globalObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> localObjects = new Dictionary<string, GameObject>();
    private void Start()
    {
        connection = FindObjectOfType<Connection>();
        multiplayerObjects = FindObjectsOfType<MultiplayerObject>();
        if (connection != null)
        {
            connection.On("data", SyncGlobal);

            // Start a background task to listen for incoming messages
            StartCoroutine(SyncCoroutine());
        }
    }
    public IEnumerator SyncCoroutine()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            SyncLocal();
            yield return new WaitForEndOfFrame();
        }
    }
    void SyncLocal()
    {
        if (connection == null) return;

        foreach (var obj in multiplayerObjects)
        {
            localObjects[obj._id] = obj.gameObject;
            connection.Send("data", obj.AsJson(), Protocol.UDP);
        }
    }

    void SyncGlobal(JObject data, Client sender)
    {
        string _id = data.Value<string>("_id");
        if (localObjects.ContainsKey(_id)) return;

        GameObject obj = null;
        globalObjects.TryGetValue(_id, out obj);
        if (obj == null) {
            // create the new object
            string tag = data.Value<string>("tag"); // TODO: create by tag linked to prefab

            obj = Instantiate(multiplayerPrefab);
            globalObjects[_id] = obj;
        }

        if (obj == null) return;

        GlobalMultiplayerObject gmo = obj.GetComponent<GlobalMultiplayerObject>();

        gmo.targetPosition = new Vector3(
            (float)data["transform"]["position"][0],
            (float)data["transform"]["position"][1],
            (float)data["transform"]["position"][2]
        );

        gmo.targetEulerAngles = new Vector3(
            (float)data["transform"]["eulerAngles"][0],
            (float)data["transform"]["eulerAngles"][1],
            (float)data["transform"]["eulerAngles"][2]
        );
        gmo.playerName = (string)data["playerName"];
    }
}
