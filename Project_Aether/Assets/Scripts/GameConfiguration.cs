using System;

[Serializable]
public class GameConfiguration
{
    public ApiKeys ApiKeys;

    public GameConfiguration() : base()
    {
        this.ApiKeys = new ApiKeys();
    }
}
