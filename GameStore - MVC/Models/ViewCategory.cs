using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameStoreV1._0.Models
{
    public class ViewCategory
    {
        public string GID { get; set; }
        [Required(ErrorMessage = "Game name cannot be null")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "The game name must be between 1 and 50 characters long")]
        public string gameName { get; set; }
        [Required(ErrorMessage = "Price cannot be null")]
        [Range(0, long.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public long price { get; set; }
        [Required(ErrorMessage = "Picture cannot be null")]
        public string picture { get; set; }
        public DateTime date { get; set; }
        public DateTime? updateDate { get; set; }
        [Required(ErrorMessage = "Description cannot be null")]
        [StringLength(4000, MinimumLength = 10, ErrorMessage = "The description must be between 10 and 4000 characters long")]
        public string description { get; set; }
        [Required(ErrorMessage = "Configuration cannot be null")]
        [StringLength(4000, MinimumLength = 10, ErrorMessage = "The configuration must be between 10 and 4000 characters long")]
        public string configuration { get; set; }
        public string sellerName { get; set; }
        public string downloadFile { get; set; }

        public string status { get; set; }
        public List<Category> viewCategory { get; set; }
        public List<Category> AllCategory { get; set; }
    }
}