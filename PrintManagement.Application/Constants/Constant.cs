using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Constants
{
    public class Constant
    {
        public class AppSettingKeys
        {
            public const string DEFAULT_CONNECTION = "DefaultConnection";
        }
        public class DefaultRoute
        {
            public const string DEFAULT_CONTROLLER_ROUTE = "api/[controller]/[action]";
        }
    }
}
