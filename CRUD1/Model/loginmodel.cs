using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD1.Model
{
    /*
      login class hold user’s credentials on the server. 
    Login is a simple class that contains two properties: UserName and Password.
     */
    public class loginmodel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string Email { get; set; }
        public string? Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }


    }
}
