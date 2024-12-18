using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStoreV1._0.Models
{
    public class AdminRevenueViewModel
    {
        public string adusername { get; set; }
        public string cusName { get; set; }
        public string gameName { get; set; }
        public string GID { get; set; }
        public long gamePrice { get; set; }
        public string picture { get; set; }
        public long admoney { get; set; }
        public long adMoneyPlus { get; set; }
        public long sellerMoneyPlus { get; set; }
        public DateTime date { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public string status { get; set; }
    }
}