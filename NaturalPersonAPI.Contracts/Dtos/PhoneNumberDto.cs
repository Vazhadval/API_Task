using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalPersonAPI.Contracts.Dtos
{
    public class PhoneNumberDto
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Phone { get; set; }
        public long NaturalPersonId { get; set; }
    }
}
