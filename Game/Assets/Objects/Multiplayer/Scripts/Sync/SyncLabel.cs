using Newtonsoft.Json.Linq;
using UnityEngine;

public class SyncLabel : SyncComponent
{
    public string playerName = "Some Random";
    public GameObject playerLabelPrefab;

    private RectTransform labelsParent;
    private GameObject playerLabelInstance;
    private PlayerLabel playerLabelComponentInstance;

    public override void Spawn(JObject data, ISyncer syncer)
    {
        base.Spawn(data, syncer);
        // Called when the object is first loaded in
        labelsParent = GameObject.FindGameObjectWithTag("PlayerLabels").transform as RectTransform;
        if (labelsParent != null && playerLabelPrefab != null)
        {
            playerLabelInstance = Instantiate(playerLabelPrefab, labelsParent);

            playerLabelComponentInstance = playerLabelInstance.GetComponent<PlayerLabel>();

            playerLabelComponentInstance.objectToFollow = transform;
            playerLabelComponentInstance.SetName(playerName);
        }
    }
    public override void SyncIn(JObject data, ISyncer syncer)
    {
        playerName = data.Value<string>("playerName");
        if (playerLabelComponentInstance != null)
            playerLabelComponentInstance.SetName(playerName);
    }
    public override void SyncOut(ref JObject data, ISyncer syncer)
    {
        base.SyncOut(ref data, syncer);
        data["playerName"] = playerName;
    }

    private void OnDestroy()
    {
        if (playerLabelInstance != null)
            Destroy(playerLabelInstance);
    }
}
