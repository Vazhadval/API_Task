using NaturalPersonAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonAPI.Domain
{
    public class NaturalPerson
    {
        [Key]
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string PersonalNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public int CityId { get; set; }
        [ForeignKey(nameof(CityId))]
        public City City { get; set; }
        public virtual List<PhoneNumber> PhoneNumbers { get; set; }
        public string Photo { get; set; }

        [NotMapped]
        public List<NaturalPerson> RelatedPeople { get; set; }

    }
}
