using NaturalPersonAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonAPI.Domain
{
    public class PhoneNumber
    {
        [Key]
        public long Id { get; set; }
        public string Type { get; set; }
        public string Phone { get; set; }
        public long NaturalPersonId { get; set; }
    }
}
