using Microsoft.AspNetCore.JsonPatch;
using NaturalPersonAPI.Contracts.Requests;
using NaturalPersonAPI.Domain;
using NaturalPersonAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonAPI.Repository
{
    public interface INaturalPersonService
    {
        Task<NaturalPerson> CreatePersonAsync(NaturalPerson p);
        Task<NaturalPerson> AddRelatedPersonAsync(long parentPersonId, RelationType relationType, NaturalPerson p);
        Task<NaturalPerson> GetPersonByIdAsync(long id);
        Task<bool> SetPhotoToPersonAsync(long personId, string photoPath);
        IEnumerable<NaturalPerson> SearchPeople();
        Task<bool> CityExistsAsync(int cityId);
        Task<bool> SaveChangesAsync();
    }
}
