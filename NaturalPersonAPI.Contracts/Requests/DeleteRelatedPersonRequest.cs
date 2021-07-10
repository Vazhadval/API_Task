using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalPersonAPI.Contracts.Requests
{
    public class DeleteRelatedPersonRequest
    {
        public long ParentPersonId { get; set; }
        public long RelatedPersonId { get; set; }
    }
}
