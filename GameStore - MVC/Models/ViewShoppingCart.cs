using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameStoreV1._0.Models
{
    public class ViewShoppingCart
    {
        [Key]
        public string GID { get; set; }
        public string gameName { get; set; }
        public string picture { get; set; }
        public long price { get; set; }
    }
}