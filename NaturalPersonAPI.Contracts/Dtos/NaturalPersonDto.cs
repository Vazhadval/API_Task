using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalPersonAPI.Contracts.Dtos
{
    public class NaturalPersonDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string PersonalNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public int CityId { get; set; }
        public CityDto City { get; set; }
        public virtual List<PhoneNumberDto> PhoneNumbers { get; set; }
        public string Photo { get; set; }


    }
}
