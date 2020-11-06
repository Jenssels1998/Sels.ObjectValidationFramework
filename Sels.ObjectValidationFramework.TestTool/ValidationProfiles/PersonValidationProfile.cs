using Sels.Core.Extensions.General.Generic;
using Sels.ObjectValidationFramework.TestTool.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sels.ObjectValidationFramework.TestTool.ValidationProfiles
{
    public class PersonValidationProfile : ValidationProfile<string>
    {
        public PersonValidationProfile() : base()
        {
            CreateValidator<Person>()
                .IfNull(() => $"{nameof(Person)} cannot be null")
                .IgnorePropertyForValidation(x => x.Parent)
                .AddValidValidation(x => x.FirstName, x => x.HasValue(), (x,y) => $"Person {y.Id}: {nameof(Person.FirstName)} cannot be null or whitespace. Was<{x}>")
                .AddValidValidation(x => x.LastName, x => x.HasValue(), (x, y) => $"Person {y.Id}: {nameof(Person.LastName)} cannot be null or whitespace. Was<{x}>")
                .AddValidValidation(x => x.Age, x => x.HasValue(), (x, y) => $"Person {y.Id}: {nameof(Person.Age)} must be above 0. Was<{x}>")
                .AddInvalidValidation(x => x.Parent, x => x == null, (x, y) => $"Person {y.Id}: {nameof(Person.Parent)} cannot be null")
                .AddInvalidValidation(x => x.Gender, x => x == Gender.Null, (x, y) => $"Person {y.Id}: {nameof(Person.Gender)} cannot be Null. Was<{x}>")
                .ConditionalValidation(x => x.Gender == Gender.Male, x =>
                {
                    x.AddValidValidation(x => x.NickName, x => x.HasValue(), (x, y) => $"Person {y.Id}: {nameof(Person.NickName)} cannot be null or whitespace. Was<{x}>");
                })
                ;
            
                
            
        }
    }
}
