using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnonymousBidder.Models
{
    public class FilePathModel
    {
        public Guid FilePathGUID { get; set; }
        public string FilePathName { get; set; }
        public Guid FilePath_AuctionGUID { get; set; }
    }
}