using NaturalPersonAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonAPI.Domain
{
    public class Relation
    {
        [Key]
        public long Id { get; set; }
        public string RelationType { get; set; }
        public long parentPersonId { get; set; }
        public long RelatedPersonId { get; set; }
    }
}
