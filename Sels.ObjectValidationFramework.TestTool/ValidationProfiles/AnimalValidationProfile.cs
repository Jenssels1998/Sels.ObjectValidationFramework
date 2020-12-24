using Sels.Core.Extensions.General.Generic;
using Sels.ObjectValidationFramework.TestTool.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using Sels.Core.Components.Display.ObjectLabel;

namespace Sels.ObjectValidationFramework.TestTool.ValidationProfiles
{
    public class AnimalValidationProfile : ValidationProfile<string>
    {
        public AnimalValidationProfile()
        {
            IgnorePropertyForFallThrough<Animal>(x => x.Owner);

            CreateValidator<Animal>()
                  .AddValidValidation(x => x.Age, x => x > 0, x => $"Animal: {x.Property.GetLabel()} must be above 0. Was<{x.PropertyValue}>");
        }
    }
}
