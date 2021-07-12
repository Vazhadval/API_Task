using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalPersonAPI.Contracts.Responses
{
    public class DeletePersonResponse : BaseResponse
    {
        public long DeletedPersonId { get; set; }
    }
}
