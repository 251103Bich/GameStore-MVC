using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameStoreV1._0.Models
{
    public class Order
    {
        [Key]
        public string OID { get; set; } 
        public string username { get; set; }
        public long total { get; set; } 
        public DateTime date { get; set; }    

    }
}