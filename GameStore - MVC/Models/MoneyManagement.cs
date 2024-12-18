using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GameStoreV1._0.Models
{
    public class MoneyManagement
    {
        public string ODID { get; set; }
        [ForeignKey("ODID")]
        public OrderDetail OrderDetail { get; set; }
        public string username { get; set; }
        [ForeignKey("username")]
        public Profile Profile { get; set; }
        public string admin { get; set; }
        public long sellerMoney { get; set; }
        public long adMoney { get; set; }
        public DateTime date { get; set; }
    }
}