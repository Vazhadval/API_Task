using NaturalPersonAPI.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalPersonAPI.Contracts.Responses
{
    public class CreateNaturalPersonResponse
    {
        public NaturalPerson CreatedPerson { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
