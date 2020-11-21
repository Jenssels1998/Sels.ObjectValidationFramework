using Sels.Core.Extensions.General.Generic;
using Sels.ObjectValidationFramework.TestTool.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sels.ObjectValidationFramework.TestTool.ValidationProfiles
{
    public class AnimalValidationProfile : ValidationProfile<string>
    {
        public AnimalValidationProfile()
        {
            IgnorePropertyForValidation<Animal>(x => x.Owner);

            CreateValidator<Animal>()
                  .AddValidValidation(x => x.Age, x => x > 0, (x, y) => $"Animal: {nameof(Animal.Age)} must be above 0. Was<{x}>");
        }
    }
}
