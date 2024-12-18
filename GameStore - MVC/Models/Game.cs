using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameStoreV1._0.Models
{
    public class Game
    {
        [Key]
        public string GID { get; set; }
        public string gameName { get; set; }
        public long price { get; set; }
        public string picture { get; set; }
        public DateTime date { get; set; }
        public DateTime? updateDate { get; set; }
        public string description { get; set; }
        public string configuration { get; set; }
        public string sellerName { get; set; }
        public string status { get; set; }
        public string downloadFile { get; set; }

    }
}