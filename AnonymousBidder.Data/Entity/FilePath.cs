using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AnonymousBidder.Data.Entity
{
    public class FilePath : BaseEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid FilePathGUID { get; set; }
        [Required]
        public string FilePathName { get; set; }
        [Required]
        public string FileType { get; set; }
        [Required]
        public string FileHash { get; set; }
        [Required]
        public Guid FilePath_AuctionGUID { get; set; }
        [ForeignKey("FilePath_AuctionGUID")]
        public virtual Auction Auction { get; set; }
    }
}