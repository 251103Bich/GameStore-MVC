using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using GameStoreV1._0.Models;

namespace GameStore.Models
{
    public class CategoryConnect
    {
        public string GID { get; set; }
        public string CID { get; set; }
        [ForeignKey("CID")]
        public Category Category { get; set; }
        [ForeignKey("GID")]
        public Game Game { get; set; }
    }
}