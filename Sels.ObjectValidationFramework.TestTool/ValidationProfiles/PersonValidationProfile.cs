using Sels.Core.Extensions.General.Generic;
using Sels.ObjectValidationFramework.TestTool.Objects;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Sels.Core.Components.Display.ObjectLabel;

namespace Sels.ObjectValidationFramework.TestTool.ValidationProfiles
{
    public class PersonValidationProfile : ValidationProfile<string>
    {
        public PersonValidationProfile() : base()
        {
            IgnorePropertyForFallThrough<Person>(x => x.Parent);

            CreateValidator<Person>()
                .IfNull(() => $"{typeof(Person).GetLabel()} cannot be null")
                .AddValidValidation(x => x.FirstName, x => x.HasValue(), x => $"Person {x.Object.Id}: {x.Property.GetLabel()} cannot be null or whitespace. Was <{x.PropertyValue}>")
                .AddValidValidation(x => x.LastName, x => x.HasValue(), x => $"Person {x.Object.Id}: {x.Property.GetLabel()} cannot be null or whitespace. Was <{x.PropertyValue}>")
                .AddInvalidValidation(x => x.Parent, x => x == null, x => $"Person {x.Object.Id}: {x.Property.GetLabel()} cannot be null")
                .AddInvalidValidation(x => x.Gender, x => x == Gender.Null, x => $"Person {x.Object.Id}: {x.Property.GetLabel()} cannot be Null. Was <{x.PropertyValue}>")
                .AddValidValidation(x =>x.Owner, x => x == null, x => $"Person {x.Object.Id}: {x.Property.GetLabel()} must be null.")
                .ConditionalValidation(x => x.Gender == Gender.Male, x =>
                {
                    x.AddValidValidation(x => x.NickName, x => x.HasValue(), x => $"Person {x.Object.Id}: {x.Property.GetLabel()} cannot be null or whitespace. Was <{x.PropertyValue}>");
                });

            ImportProfile<AnimalValidationProfile>();
        }
    }
}
