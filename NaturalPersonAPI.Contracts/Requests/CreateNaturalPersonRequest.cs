using Domain.Domain.Enums;
using Microsoft.AspNetCore.Http;
using NaturalPersonAPI.Domain;
using NaturalPersonAPI.Domain.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalPersonAPI.Contracts.Requests
{
    public class CreateNaturalPersonRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string PersonalNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public int CityId { get; set; }
        public List<PhoneNumberDto> PhoneNumbers { get; set; }



        public class PhoneNumberDto
        {
            public string PhoneNumberType { get; set; }
            public string Phone { get; set; }
        }
    }
}
