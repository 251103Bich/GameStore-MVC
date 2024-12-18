using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace GameStoreV1._0.Models
{
    public class ChangePassword
    {
        [Key]
        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 20 characters.")]
        public string username { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password must be between 5 and 20 characters.")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Password cannot contain whitespace.")]
        public string password { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Password must be between 5 and 20 characters.")]
        [Compare("password", ErrorMessage = "Passwords do not match.")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Password cannot contain whitespace.")]
        public string PasswordAgain { get; set; }
        [Required]
        public string email { get; set; }
        public string fullname { get; set; }
        public string gender { get; set; }
        public DateTime? birthday { get; set; }
        public long money { get; set; }
        public string type { get; set; }
        public string status { get; set; }
    }
}