using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameStoreV1._0.Models
{
    public class MoneyHistory
    {
        [Key]
        public string MID {get; set;}
        public string username {get; set;}
        public long money {get; set;}
        public DateTime date {get; set;}
        public string status {get; set;}
    }
}