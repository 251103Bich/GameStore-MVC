using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStoreV1._0.Models
{
    public class SellerRevenue
    {
        public string sellerName { get; set; }
        public string cusName { get; set; }
        public DateTime date { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public long gamePrice { get; set; }
        public long sellerPlusMoney { get; set; }
        public long sellerMoney { get; set; }
        public string GID { get; set; }
        public string gameName { get; set; }
        public string picture { get; set; }
        public string status { get; set; }
    }
}