using Domain.Domain.Enums;
using FluentValidation;
using NaturalPersonAPI.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NaturalPersonAPI.Validators
{
    public class CreateNaturalPersonValidator : AbstractValidator<CreateNaturalPersonRequest>
    {
        public CreateNaturalPersonValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(50);




            RuleFor(x => x.FirstName)
                .Matches(@"^[ა-ჰ]+$").When(x => Regex.IsMatch(x.FirstName, @"[ა-ჰ]"))
                .WithMessage("FirstName should contain only Latin or only Georgian letters.")
                .Matches(@"^[a-zA-Z]+$").When(x => Regex.IsMatch(x.FirstName, @"[a-zA-Z]"))
                .WithMessage("FirstName should contain only Latin or only Georgian letters.");

            RuleFor(x => x.LastName)
               .NotEmpty()
               .MinimumLength(2)
               .MaximumLength(50);




            RuleFor(x => x.LastName)
                .Matches(@"^[ა-ჰ]+$").When(x => Regex.IsMatch(x.LastName, @"[ა-ჰ]"))
                .WithMessage("FirstName should contain only Latin or only Georgian letters.")
                .Matches(@"^[a-zA-Z]+$").When(x => Regex.IsMatch(x.LastName, @"[a-zA-Z]"))
                .WithMessage("FirstName should contain only Latin or only Georgian letters.");


            RuleFor(x => x.Gender)
                .Must(x => x.ToUpper() == "MALE" || x.ToUpper() == "FEMALE")
                .WithMessage("Choose 'Male' or 'Female'");


            RuleFor(x => x.PersonalNumber)
                .Matches(@"\d{11}")
                .WithMessage("Personal number should contain 11 digits");

            RuleFor(x => x.BirthDate)
                .LessThan(new DateTime(DateTime.Now.Year - 18, DateTime.Now.Month, DateTime.Now.Day))
                .WithMessage("Person should at least 18 years old.");


            RuleForEach(x => x.PhoneNumbers)
                .Must(x => x.PhoneNumberType.ToUpper() == "HOME" || x.PhoneNumberType.ToUpper() == "OFFICE")
                .WithMessage("Choose 'Home' or 'Office'")
                .Must(x => x.Phone.Length >= 4 && x.Phone.Length <= 50)
                .WithMessage("Phone number should be between 4 and 50 characters");

            RuleFor(x => x.PhoneNumbers)
                .NotNull()
                .WithMessage("Person shoud have at least one phone number");

        }
    }
}
