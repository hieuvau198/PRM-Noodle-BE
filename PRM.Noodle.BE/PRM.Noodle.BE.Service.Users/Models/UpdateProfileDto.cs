using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Users.Models
{
    public class UpdateProfileDto
    {
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}
