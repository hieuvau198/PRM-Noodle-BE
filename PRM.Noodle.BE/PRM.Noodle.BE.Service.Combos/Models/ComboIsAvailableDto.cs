using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Service.Combos.Models
{
    public class ComboIsAvailableDto
    {
        [Required]
        public bool IsAvailable { get; set; }
    }
}
