using Newtonsoft.Json.Linq;

public interface ISyncOut
{
    string _id { get; set; }
    Protocol protocol { get; set; }
    /// <summary>
    /// Called when data is sent out to the server
    /// </summary>
    void SyncOut(ref JObject data, ISyncer syncer);
}