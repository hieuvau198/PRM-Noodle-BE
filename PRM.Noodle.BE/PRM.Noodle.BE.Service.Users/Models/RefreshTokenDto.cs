using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Users.Models
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
