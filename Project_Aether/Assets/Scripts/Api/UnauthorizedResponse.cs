using System;

[Serializable]
public class UnauthorizedResponse
{
    public string Message { get; set; }

    public UnauthorizedResponse() : base()
    {
        this.Message = string.Empty;
    }

    public UnauthorizedResponse(string Message) : this()
    {
        this.Message = Message;
    }
}
