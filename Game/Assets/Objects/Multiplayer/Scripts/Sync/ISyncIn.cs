using Newtonsoft.Json.Linq;

public interface ISyncIn
{
    string _id { get; set; }
    /// <summary>
    /// Called when the component is synced for the first time
    /// </summary>
    void Spawn(JObject data, ISyncer syncer);
    /// <summary>
    /// Called when data is received in from the server
    /// </summary>
    void SyncIn(JObject data, ISyncer syncer);
}