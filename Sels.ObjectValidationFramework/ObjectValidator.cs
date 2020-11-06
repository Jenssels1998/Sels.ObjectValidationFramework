using Sels.Core.Extensions.General.Generic;
using Sels.Core.Extensions.General.Validation;
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

            var validators = profile.Validators.Value;
            var objectType = typeof(TObject);

            var errors = validators.Validate<TError>(objectToValidate, objectType);

            return errors;
        }

        public static IEnumerable<TError> Validate<TError>(ValidationProfile<TError> profile, object objectToValidate, Type objectType)
        {
            profile.ValidateVariable(nameof(profile));
            objectType.ValidateVariable(nameof(objectType));

            var validators = profile.Validators.Value;

            var errors = validators.Validate<TError>(objectToValidate, objectType);

            return errors;
        }


        private static IEnumerable<TError> Validate<TError>(this IEnumerable<BaseValidator> validators, object objectToValidate, Type objectType)
        {
            var errors = new List<TError>();

            var objectValidators = validators.GetValidatorsForType(objectType);

            // Validate object
            foreach (var validator in objectValidators)
            {
                var objectErrors = validator.ValidateDelegate.Invoke<IEnumerable<TError>>(objectToValidate);
                errors.AddRange(objectErrors);
            }

            // Validate underlying types
            errors.AddRange(validators.ValidatePropertyTypes<TError>(objectToValidate));
                

            return errors;
        }

        private static IEnumerable<TError> ValidatePropertyTypes<TError>(this IEnumerable<BaseValidator> validators, object objectToValidate)
        {
            var errors = new List<TError>();

            if (objectToValidate == null) return errors;

            foreach(var property in objectToValidate.GetProperties())
            {
                var propertyType = property.PropertyType;
                var propertyValue = property.GetValue(objectToValidate);

                if (propertyValue == null) continue;

                // Validate underlying type
                if (validators.HasValidatorForProperty(property))
                {
                    errors.AddRange(validators.Validate<TError>(propertyValue, property.PropertyType));
                }

                // If we find a collection we look at the item type and see if we have a validator for it
                if (propertyType.IsItemContainer())
                {
                    var itemType = propertyType.GetItemTypeFromContainer();
                    if (validators.HasValidatorForType(itemType))
                    {
                        // We have a validator for the item type. We now loop over the items in the collection and trigger validation
                        foreach (var item in (IEnumerable)propertyValue)
                        {
                            errors.AddRange(validators.Validate<TError>(item, itemType));
                        }                        
                    }
                }

                
            }

            return errors;
        }
    }
}
