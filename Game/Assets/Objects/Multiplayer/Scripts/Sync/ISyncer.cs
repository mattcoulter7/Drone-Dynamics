using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using SocketRunner;

public interface ISyncer
{
    void RegisterIn(ISyncIn obj);
    void DeregisterIn(string _id);
    void RegisterOut(ISyncOut obj);
    void DeregisterOut(string _id);
}