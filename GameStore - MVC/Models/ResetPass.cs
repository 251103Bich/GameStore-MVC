using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameStoreV1._0.Models
{
  public class ResetPass
  {
    [StringLength(20, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 20 characters.")]
    [RegularExpression(@"^\S*$", ErrorMessage = "Password cannot contain whitespace.")]
    public string newPass { get; set; }
    [StringLength(20, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 20 characters.")]
    [Compare("newPass", ErrorMessage = "Passwords do not match.")]
    [RegularExpression(@"^\S*$", ErrorMessage = "Password cannot contain whitespace.")]
    public string confirmPass { get; set; }
  }
}