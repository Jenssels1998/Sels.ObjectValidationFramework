using Microsoft.Extensions.Logging;
using Sels.Core.Extensions.General.Generic;
using Sels.Core.Extensions.General.Validation;
using Sels.Core.Extensions.Logging;
using Sels.Core.Extensions.Reflection.Object;
using Sels.Core.Extensions.Reflection.Types;
using Sels.ObjectValidationFramework.Extensions;
using Sels.ObjectValidationFramework.Validator;
using Sels.ObjectValidationFramework.Validator.Case;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sels.ObjectValidationFramework
{
    public static class ObjectValidator
    {
        public static IEnumerable<TError> Validate<TObject, TError>(ValidationProfile<TError> profile, TObject objectToValidate)
        {
            profile.ValidateVariable(nameof(profile));

            var objectType = typeof(TObject);

            var errors = profile.ValidateObject<TError>(objectToValidate, objectType);

            return errors;
        }

        public static IEnumerable<TError> Validate<TError>(ValidationProfile<TError> profile, object objectToValidate, Type objectType)
        {
            profile.ValidateVariable(nameof(profile));
            objectType.ValidateVariable(nameof(objectType));

            var errors = profile.ValidateObject<TError>(objectToValidate, objectType);

            return errors;
        }

        private static IEnumerable<TError> ValidateObject<TError>(this ValidationProfile<TError> profile, object objectToValidate, Type objectType)
        {
            var errors = new List<TError>();

            var validators = profile.Validators.GetValidatorsForType(objectType);

            // Validate object
            foreach (var validator in validators)
            {
                var objectErrors = validator.ValidateDelegate.Invoke<IEnumerable<TError>>(objectToValidate);
                errors.AddRange(objectErrors);
            }

            if(objectToValidate != null)
            {
                // If we find a collection we look at the item type and see if we have a validator for it
                if (objectType.IsItemContainer() && !objectType.IsString())
                {
                    var itemType = objectType.GetItemTypeFromContainer();
                    // We have a validator for the item type. We now loop over the items in the collection and trigger validation
                    foreach (var item in (IEnumerable)objectToValidate)
                    {
                        errors.AddRange(profile.ValidateObject<TError>(item, itemType));
                    }
                }

                // Fallthrough properties
                errors.AddRange(ValidatePropertyTypes(profile, objectToValidate));
            }          

            return errors;
        }

        private static IEnumerable<TError> ValidatePropertyTypes<TError>(this ValidationProfile<TError> profile, object objectToValidate)
        {
            var errors = new List<TError>();

            if (objectToValidate == null) return errors;
            if (profile.IsIgnored(objectToValidate.GetType())) return errors;

            foreach (var property in objectToValidate.GetProperties())
            {
                try
                {
                    if (profile.IsIgnored(property)) continue;

                    var propertyType = property.PropertyType;

                    var propertyValue = property.GetValue(objectToValidate);

                    // Validate underlying type
                    errors.AddRange(profile.ValidateObject<TError>(propertyValue, propertyType));
                }
                catch(Exception ex)
                {
                    profile.Logger.LogException(LogLevel.Warning, () => $"Could not fallthrough validate property <{property}> on object <{objectToValidate}>", ex);
                }              
            }

            return errors;
        }
    }
}
