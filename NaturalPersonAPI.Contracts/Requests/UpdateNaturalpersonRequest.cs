using System;
using System.Collections.Generic;
using System.Text;
using static NaturalPersonAPI.Contracts.Requests.CreateNaturalPersonRequest;

namespace NaturalPersonAPI.Contracts.Requests
{
    public class UpdateNaturalpersonRequest
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string PersonalNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public int CityId { get; set; }
        public List<PhoneNumberDto> PhoneNumbers { get; set; }
    }
}
