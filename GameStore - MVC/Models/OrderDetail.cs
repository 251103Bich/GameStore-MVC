using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameStoreV1._0.Models
{
    public class OrderDetail
    {
        public string OID { get; set; }
        
        public string GID { get; set; }
        public long price { get; set; }
        [Key]
        public string ODID { get; set; }

    }
}