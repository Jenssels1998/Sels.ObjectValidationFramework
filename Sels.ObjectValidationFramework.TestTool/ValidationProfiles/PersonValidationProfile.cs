using Sels.Core.Extensions.General.Generic;
using Sels.ObjectValidationFramework.TestTool.Objects;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Sels.ObjectValidationFramework.TestTool.ValidationProfiles
{
    public class PersonValidationProfile : ValidationProfile<string>
    {
        public PersonValidationProfile() : base()
        {
            IgnorePropertyForValidation<Person>(x => x.Parent);

            CreateValidator<Person>()
                .IfNull(() => $"{nameof(Person)} cannot be null")
                .AddValidValidation(x => x.FirstName, x => x.HasValue(), (x,y) => $"Person {y.Id}: {nameof(Person.FirstName)} cannot be null or whitespace. Was<{x}>")
                .AddValidValidation(x => x.LastName, x => x.HasValue(), (x, y) => $"Person {y.Id}: {nameof(Person.LastName)} cannot be null or whitespace. Was<{x}>")
                .AddInvalidValidation(x => x.Parent, x => x == null, (x, y) => $"Person {y.Id}: {nameof(Person.Parent)} cannot be null")
                .AddInvalidValidation(x => x.Gender, x => x == Gender.Null, (x, y) => $"Person {y.Id}: {nameof(Person.Gender)} cannot be Null. Was<{x}>")
                .AddValidValidation(x =>x.Owner, x => x == null, (x, y) => $"Person {y.Id}: {nameof(Person.Owner)} must be null.")
                .ConditionalValidation(x => x.Gender == Gender.Male, x =>
                {
                    x.AddValidValidation(x => x.NickName, x => x.HasValue(), (x, y) => $"Person {y.Id}: {nameof(Person.NickName)} cannot be null or whitespace. Was<{x}>");
                });

            ImportProfile<AnimalValidationProfile>();
        }
    }
}
