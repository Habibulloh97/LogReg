using System;
using System.ComponentModel.DataAnnotations;
namespace LogReg.Models
{
    public class LoginUser
    {

        [Required]
        [EmailAddress]
        public string LEmail {get;set;}
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage ="Password must be at leats 8 characters long!")]
        public string LPassword {get;set;} 
    }
}