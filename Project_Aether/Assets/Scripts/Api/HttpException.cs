using System.Net;
using UnityEngine;

public class HttpException : System.Exception
{
    public HttpStatusCode StatusCode { get; }

    public HttpException(HttpStatusCode statusCode, string message = "HTTP Exception") : base(message)  
    {
        this.StatusCode = statusCode;
    }
}
