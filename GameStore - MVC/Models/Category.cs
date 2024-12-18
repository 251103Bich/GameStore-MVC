using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameStoreV1._0.Models
{
    public class Category
    {
        [Key]
        public string CID { get; set; }
        public string categoryName { get; set; }
    }
}