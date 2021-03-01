using Microsoft.Extensions.Logging;
using Sels.Core.Extensions;
using Sels.Core.Extensions;
using Sels.Core.Extensions.Logging;
using Sels.Core.Extensions.Reflection;
using Sels.Core.Extensions.Reflection;
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
        public static IEnumerable<TError> Validate<TError>(ValidationProfile<TError> profile, object objectToValidate, Type objectType = null)
        {
            profile.ValidateVariable(nameof(profile));

            if(objectToValidate == null && objectType == null)
            {
                profile.Logger.LogMessage(LogLevel.Warning, $"Could not validate object because {nameof(objectToValidate)} and {nameof(objectType)} was null");

                return new List<TError>();
            }

            var errors = profile.ValidateObject(objectToValidate, objectType ?? objectToValidate.GetType(), new List<object>());

            return errors;
        }

        private static IEnumerable<TError> ValidateObject<TError>(this ValidationProfile<TError> profile, object objectToValidate, Type objectType, List<object> stackTrace)
        {
            var errors = new List<TError>();

            if(objectToValidate != null && !objectToValidate.GetType().IsValueType)
            {
                // Only validate if haven't validated the object before
                if (stackTrace.Contains(objectToValidate))
                {
                    return errors;
                }
                else
                {
                    stackTrace.Add(objectToValidate);
                }
            }

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
                        errors.AddRange(profile.ValidateObject(item, itemType, stackTrace));
                    }
                }

                // Fallthrough properties
                if (profile.FallThroughProperties && profile.Validators.Count > 1)
                {
                    errors.AddRange(ValidatePropertyTypes(profile, objectToValidate, stackTrace));
                }
            }          

            return errors;
        }

        private static IEnumerable<TError> ValidatePropertyTypes<TError>(this ValidationProfile<TError> profile, object objectToValidate, List<object> stackTrace)
        {
            var errors = new List<TError>();

            if (objectToValidate == null || profile.IsIgnored(objectToValidate.GetType())) return errors;

            foreach (var property in objectToValidate.GetProperties())
            {
                try
                {
                    if (profile.IsIgnored(property)) continue;

                    var propertyType = property.PropertyType;

                    var propertyValue = property.GetValue(objectToValidate);

                    // Validate underlying type
                    errors.AddRange(profile.ValidateObject(propertyValue, propertyType, stackTrace));
                }
                catch (StackOverflowException stackEx)
                {
                    profile.Logger.LogException(LogLevel.Error, () => $"Could not fallthrough validate property <{property}> on object <{objectToValidate}> because stack overflow was detected. Potential property loop.", stackEx);
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
