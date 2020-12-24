using Microsoft.Extensions.Logging;
using Sels.Core.Components.Serialization;
using Sels.Core.Extensions.General.Generic;
using Sels.Core.Extensions.General.Validation;
using Sels.Core.Extensions.Logging;
using Sels.Core.Extensions.Reflection.Object;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sels.ObjectValidationFramework.Validator.Case
{
    internal class PropertyValidationExecutor<TObject, TPropertyValue, TError> : BasePropertyValidationExecutor<TObject>
    {
        // Fields       
        private readonly Func<(TPropertyValue PropertyValue, PropertyInfo Property, TObject Object), TError> _errorMessage;
        private readonly Predicate<TPropertyValue> _propertyValueChecker;
        

        // Properties
        internal override PropertyInfo TargetProperty { get; }
        internal override Delegate ValidateDelegate { get; }

        internal PropertyValidationExecutor(PropertyInfo property, Predicate<TPropertyValue> propertyValueChecker, Func<(TPropertyValue PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage, ValidationType validationType, Predicate<TObject> condition, ILogger logger) : base(validationType, condition, logger)
        {
            propertyValueChecker.ValidateVariable(nameof(propertyValueChecker));
            errorMessage.ValidateVariable(nameof(errorMessage));
            
            property.ValidateVariable(nameof(property));

            _propertyValueChecker = propertyValueChecker;
            _errorMessage = errorMessage;           
            
            TargetProperty = property;

            ValidateDelegate = this.CreateDelegateForMethod(nameof(Validate));
        }

        internal (bool HasError, TError Error) Validate(TPropertyValue propertyValue, TObject parentObject)
        {
            _logger.LogMessage(LogLevel.Debug, () => $"Validating Property {TargetProperty.Name} on Type {TargetProperty.ReflectedType}");
            TError error = default;

            var result = _propertyValueChecker(propertyValue);

            if (!ShouldGenerateError(result))
            {
                _logger.LogObject<JsonProvider>(LogLevel.Debug, () => $"Property {TargetProperty.Name} on Type {TargetProperty.ReflectedType} passed validation (ValidationType {ValidationType}). Was:", propertyValue);
                return (false, error);
            }
            else
            {
                _logger.LogObject<JsonProvider>(LogLevel.Debug, () => $"Property {TargetProperty.Name} on Type {TargetProperty.ReflectedType} did not pass validation (ValidationType {ValidationType}). Was:", propertyValue);
                error = _errorMessage((propertyValue, TargetProperty, parentObject));
                return (true, error);
            }
        }


    }
}
