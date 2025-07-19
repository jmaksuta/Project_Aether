using System;
using System.Text.Json.Serialization;

[Serializable]
public class ApiKeys
{
    public string GameServer;

    public ApiKeys() : base()
    {
        this.GameServer = string.Empty;
    }
}
