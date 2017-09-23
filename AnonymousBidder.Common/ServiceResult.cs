using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymousBidder.Common
{
    public class ServiceResult
    {
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public object Params { get; set; }
    }
}
