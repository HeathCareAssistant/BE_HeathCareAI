using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Service.Config
{
    public static class EnumHelper
    {
        public static string GetEnumValue<T>(T enumVal)
        {
            return enumVal?.GetType()
                           .GetMember(enumVal.ToString())
                           .First()
                           .GetCustomAttribute<EnumMemberAttribute>()?
                           .Value ?? enumVal.ToString();
        }
    }
}
