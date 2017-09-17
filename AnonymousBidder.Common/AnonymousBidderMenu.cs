using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymousBidder.Common
{
    class AnonymousBidderMenu
    {
        public AnonymousBidderMenu()
        {
            SubMenus = new List<AnonymousBidderMenu>();
        }
        public string MenuName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public int Sequence { get; set; }
        public AnonymousBidderMenu ParentMenu { get; set; }
        public List<AnonymousBidderMenu> SubMenus { get; set; }
        public string AreaName { get; set; }
    }
}
