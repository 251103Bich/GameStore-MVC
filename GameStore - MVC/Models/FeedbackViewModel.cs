using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace GameStoreV1._0.Models
{
    public class FeedbackViewModel
    {
        public string username { get; set; }
        public string GID { get; set; }

        [Required(ErrorMessage = "Feedback is required")]
        public string feedback { get; set; }
        [Required(ErrorMessage = "Rate is required")]
        public int rate { get; set; }
        public string fullname { get; set; }
        public string status { get; set; }
        public DateTime date { get; set; }
    }
}