using Microsoft.AspNetCore.JsonPatch;
using NaturalPersonAPI.Contracts;
using NaturalPersonAPI.Contracts.Requests;
using NaturalPersonAPI.Contracts.Responses;
using NaturalPersonAPI.Domain;
using NaturalPersonAPI.Domain.Enums;
using NaturalPersonAPI.Helper;
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
        Task<bool> PersonExistsAsync(string personalNumber);
        Task<NaturalPerson> GetPersonByIdAsync(long id, bool includeRelatedPeople);
        Task<bool> DeleteRelatedPerson(long parentId, long relatedId);
        Task<bool> DeletePerson(long personId);
        Task<bool> SetPhotoToPersonAsync(long personId, string photoPath);
        PagedList<NaturalPerson> SearchPeople(SearchPeopleRequest searchTerm);
        Task<Dictionary<string, int>> GetPersonReport(long personId);
        Task<bool> CityExistsAsync(int cityId);
        Task<bool> SaveChangesAsync();
    }
}
