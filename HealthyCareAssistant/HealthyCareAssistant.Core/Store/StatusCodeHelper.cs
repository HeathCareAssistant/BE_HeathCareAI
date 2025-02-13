using HealthyCareAssistant.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Core.Store
{
    public enum StatusCodeHelper
    {
        [CustomName("Success")]
        OK = 200,

        [CustomName("Bad Request")]
        BadRequest = 400,

        [CustomName("Unauthorized")]
        Unauthorized = 401,

        [CustomName("Forbidden")]
        Forbidden = 403,

        [CustomName("Not Found")]
        NotFound = 404,

        [CustomName("Internalz Server Error")]
        ServerError = 500
    }
}
