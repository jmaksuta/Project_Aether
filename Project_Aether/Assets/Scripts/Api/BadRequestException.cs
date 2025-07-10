using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Api
{
    public class BadRequestException : HttpException
    {
        public BadRequestException(string message = "HTTP Bad Request Exception") : base(HttpStatusCode.BadRequest, message)
        {
        }
    }
}
