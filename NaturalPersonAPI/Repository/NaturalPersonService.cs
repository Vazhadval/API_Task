using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using NaturalPersonAPI.Contracts;
using NaturalPersonAPI.Contracts.Requests;
using NaturalPersonAPI.Contracts.Responses;
using NaturalPersonAPI.DataContext;
using NaturalPersonAPI.Domain;
using NaturalPersonAPI.Domain.Enums;
using NaturalPersonAPI.Helper;
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

        public async Task<bool> DeletePerson(long personId)
        {
            var p = await _context.NaturalPeople.FirstOrDefaultAsync(x => x.Id == personId);
            if (p == null)
            {
                return false;
            }

            var phoneNumbersOfPerson = _context.PhoneNumbers.Where(x => x.NaturalPersonId == personId);
            var relations = _context.Relations.Where(x => x.parentPersonId == personId || x.RelatedPersonId == personId);

            _context.PhoneNumbers.RemoveRange(phoneNumbersOfPerson);
            _context.Relations.RemoveRange(relations);
            _context.NaturalPeople.Remove(p);

            return await _context.SaveChangesAsync() > 0;
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

            p.RelatedPeople = new List<NaturalPerson>();

            foreach (var relatedPersonid in relations)
            {
                var related = await _context.NaturalPeople.Include(x => x.City).Include(x => x.PhoneNumbers).FirstOrDefaultAsync(x => x.Id == relatedPersonid);
                p.RelatedPeople.Add(related);
            }

            return p;

        }

        public async Task<Dictionary<string, int>> GetPersonReport(long personId)
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
            return dict;

        }

        public async Task<bool> PersonExistsAsync(string personalNumber)
        {
            return await _context.NaturalPeople.AnyAsync(x => x.PersonalNumber == personalNumber);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public PagedList<NaturalPerson> SearchPeople(SearchPeopleRequest searchTerm)
        {
            var people = _context.NaturalPeople.Include(x => x.City).Include(x => x.PhoneNumbers) as IQueryable<NaturalPerson>;

            if (!string.IsNullOrEmpty(searchTerm.FirstName))
            {
                people = people.Where(x => x.FirstName.ToUpper().Contains(searchTerm.FirstName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchTerm.LastName))
            {
                people = people.Where(x => x.LastName.ToUpper().Contains(searchTerm.LastName.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchTerm.PesonalNumber))
            {
                people = people.Where(x => x.PersonalNumber.ToUpper().Contains(searchTerm.PesonalNumber.ToUpper()));
            }

            return PagedList<NaturalPerson>.ToPagedList(people, searchTerm.PageNumber, searchTerm.PageSize);
        }

        public async Task<bool> SetPhotoToPersonAsync(long personId, string photoPath)
        {
            var p = await _context.NaturalPeople.FirstOrDefaultAsync(x => x.Id == personId);
            p.Photo = photoPath;


            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<NaturalPerson> UpdatePersonAsync(NaturalPerson p)
        {
            var personFromDb = await GetPersonByIdAsync(p.Id, true);
            if (personFromDb == null)
            {
                return null;
            }

            if (p.FirstName != null)
            {
                personFromDb.FirstName = p.FirstName;
            }

            if (p.LastName != null)
            {
                personFromDb.LastName = p.LastName;
            }

            if (p.Gender != null)
            {
                personFromDb.Gender = p.Gender;
            }

            if (p.PersonalNumber != null)
            {
                personFromDb.PersonalNumber = p.PersonalNumber;
            }

            if (p.BirthDate != default(DateTime))
            {
                personFromDb.BirthDate = p.BirthDate;
            }

            if (p.CityId > 0)
            {
                personFromDb.CityId = p.CityId;
                personFromDb.City = await GetCityById(p.CityId);
            }

            if (p.PhoneNumbers.Count() > 0)
            {
                personFromDb.PhoneNumbers = p.PhoneNumbers;
            }


            _context.NaturalPeople.Update(personFromDb);
            await _context.SaveChangesAsync();

            return personFromDb;
        }

        private async Task<City> GetCityById(int cityId)
        {
            return await _context.Cities.FirstOrDefaultAsync(x => x.Id == cityId);
        }
    }
}
