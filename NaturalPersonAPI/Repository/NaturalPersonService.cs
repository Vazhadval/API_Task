using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using NaturalPersonAPI.Contracts.Requests;
using NaturalPersonAPI.Contracts.Responses;
using NaturalPersonAPI.DataContext;
using NaturalPersonAPI.Domain;
using NaturalPersonAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonAPI.Repository
{
    public class NaturalPersonService : INaturalPersonService
    {
        private readonly AppDbContext _context;

        public NaturalPersonService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NaturalPerson> AddRelatedPersonAsync(long parentPersonId, RelationType relationType, NaturalPerson p)
        {
            var city = await CityExistsAsync(p.CityId);
            if (!city)
            {
                return null;
            }

            var parent = await _context.NaturalPeople.FirstOrDefaultAsync(x => x.Id == parentPersonId);
            if (parent == null)
            {
                return null;
            }

            await CreatePersonAsync(p);

            await _context.Relations.AddAsync(new Relation { parentPersonId = parentPersonId, RelatedPersonId = p.Id, RelationType = relationType.ToString() });

            await _context.SaveChangesAsync();
            return p;

        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.FirstOrDefaultAsync(x => x.Id == cityId) != null;
        }

        public async Task<NaturalPerson> CreatePersonAsync(NaturalPerson p)
        {
            if (p.PhoneNumbers?.Count() > 0)
            {
                foreach (var phone in p.PhoneNumbers)
                {
                    await _context.PhoneNumbers.AddAsync(phone);
                }
            }

            await _context.NaturalPeople.AddAsync(p);

            var created = await _context.SaveChangesAsync();


            return created > 0 ? p : null;
        }

        public async Task<bool> DeleteRelatedPerson(long parentId, long relatedId)
        {
            //check if relations exists
            var relation = await _context.Relations.FirstOrDefaultAsync(x => x.parentPersonId == parentId && x.RelatedPersonId == relatedId);
            if (relation == null)
            {
                return false;
            }

            var relatedPerson = await GetPersonByIdAsync(relatedId, false);
            //remove related person
            _context.NaturalPeople.Remove(relatedPerson);
            //remove relation
            _context.Relations.Remove(relation);
            //remove phone numbers also
            _context.PhoneNumbers.RemoveRange(relatedPerson.PhoneNumbers);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<NaturalPerson> GetPersonByIdAsync(long id, bool includeRelatedPople)
        {
            if (!includeRelatedPople)
            {
                var a = await _context.NaturalPeople.Include(x => x.City).Include(x => x.PhoneNumbers).FirstOrDefaultAsync(x => x.Id == id);
                return a;
            }


            var p = await _context.NaturalPeople.Include(x => x.City).Include(x => x.PhoneNumbers).FirstOrDefaultAsync(x => x.Id == id);
            if (p == null)
            {
                return null;
            }

            var relations = await _context.Relations.Where(x => x.parentPersonId == id).Select(y => y.RelatedPersonId).ToListAsync();
            if (relations.Count() < 1)
            {
                return null;
            }

            p.RelatedPeople = new List<NaturalPerson>();

            foreach (var relatedPersonid in relations)
            {
                var related = await _context.NaturalPeople.Include(x => x.City).Include(x => x.PhoneNumbers).FirstOrDefaultAsync(x => x.Id == relatedPersonid);
                p.RelatedPeople.Add(related);
            }

            return p;

        }

        public async Task<GetPersonReportResponse> GetPersonReport(long personId)
        {
            var person = await GetPersonByIdAsync(personId, false);
            if (person == null)
            {
                return null;
            }


            var relations = _context.Relations.Where(x => x.parentPersonId == personId);

            var dict = new Dictionary<string, int>();

            foreach (var relation in relations)
            {
                if (dict.ContainsKey(relation.RelationType))
                {
                    dict[relation.RelationType]++;
                }
                else
                {
                    dict.Add(relation.RelationType, 1);
                }
            }
            return new GetPersonReportResponse
            {
                Relations = dict
            };

        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public IEnumerable<NaturalPerson> SearchPeople(SearchPeopleRequest searchTerm)
        {
            return _context.NaturalPeople.Include(x => x.City).Include(x => x.PhoneNumbers);
        }

        public async Task<bool> SetPhotoToPersonAsync(long personId, string photoPath)
        {
            var p = await _context.NaturalPeople.FirstOrDefaultAsync(x => x.Id == personId);
            p.Photo = photoPath;


            return await _context.SaveChangesAsync() > 0;
        }
    }
}
