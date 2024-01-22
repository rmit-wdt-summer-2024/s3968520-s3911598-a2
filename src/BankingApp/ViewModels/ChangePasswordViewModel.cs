using System.ComponentModel.DataAnnotations;

namespace BankingApp.Models;

public class ChangePasswordViewModel
{
    public int CustomerID { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }
    
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match")]
    public string ConfirmPassword { get; set; }
    
}