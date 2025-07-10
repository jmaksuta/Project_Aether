using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Api
{
    public class UnauthorizedException : HttpException
    {
        public UnauthorizedException(string message = "HTTP Unauthorized Exception") : base(HttpStatusCode.Unauthorized, message)
        {
        }

    }
}
