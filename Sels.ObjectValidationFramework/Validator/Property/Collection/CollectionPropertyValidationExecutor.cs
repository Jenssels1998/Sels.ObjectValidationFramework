using Microsoft.Extensions.Logging;
using Sels.ObjectValidationFramework.Validator.Case;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Sels.Core.Extensions;
using System.Security.Cryptography.X509Certificates;
using Sels.Core.Extensions.Reflection;
using Sels.Core.Extensions.Logging;
using Sels.Core.Extensions.Reflection;
using Sels.Core.Extensions;
using Sels.Core.Components.Serialization;

namespace Sels.ObjectValidationFramework.Validator.Property.Collection
{
    internal class CollectionPropertyValidationExecutor<TObject, TElement, TError> : BasePropertyValidationExecutor<TObject>
    {
        // Fields
        private readonly List<(Predicate<TElement> ElementValueChecker, Func<(TElement ElementValue, PropertyInfo Property, TObject Object), TError> ErrorMessage)> _validatorExecutors = new List<(Predicate<TElement> ElementValueChecker, Func<(TElement ElementValue, PropertyInfo Property, TObject Object), TError> ErrorMessage)>();

        // Properties
        internal override PropertyInfo TargetProperty { get; }
         internal override Delegate ValidateDelegate { get; }

        internal CollectionPropertyValidationExecutor(PropertyInfo property, (Predicate<TElement> ElementValueChecker, Func<(TElement ElementValue, PropertyInfo Property, TObject Object), TError> ErrorMessage) validatorExecutor, ValidationType validationType, Predicate<TObject> condition, ILogger logger) : base(validationType, condition, logger)
        {
            property.ValidateVariable(x => x.PropertyType.IsTypedEnumerable(), x => $"Property must be of type IEnumerable<{typeof(TElement)}>. Type was <{x.PropertyType}>");

            AddValidatorExecutor(validatorExecutor);

            TargetProperty = property;

            ValidateDelegate = this.CreateDelegateForMethod(nameof(Validate));
        }

        internal void AddValidatorExecutor((Predicate<TElement> ElementValueChecker, Func<(TElement ElementValue, PropertyInfo Property, TObject Object), TError> ErrorMessage) validatorExecutor)
        {
            validatorExecutor.ValidateVariable(nameof(validatorExecutor));
            validatorExecutor.ElementValueChecker.ValidateVariable(nameof(validatorExecutor.ElementValueChecker));
            validatorExecutor.ErrorMessage.ValidateVariable(nameof(validatorExecutor.ErrorMessage));

            _validatorExecutors.Add(validatorExecutor);
        }

        internal (bool HasError, IEnumerable<TError> Errors) Validate(IEnumerable<TElement> collection, TObject parentObject)
        {
            _logger.LogMessage(LogLevel.Debug, () => $"Validating Property {TargetProperty.Name} on Type {TargetProperty.ReflectedType}");

            List<TError> errors = new List<TError>();

            if (collection.IsDefault() || !_validatorExecutors.HasValue()) return (false, default);

            foreach(var executor in _validatorExecutors)
            {
                foreach(var item in collection)
                {
                    var result = executor.ElementValueChecker(item);

                    if (!ShouldGenerateError(result))
                    {
                        _logger.LogObject<JsonProvider>(LogLevel.Debug, () => $"Element in Property {TargetProperty.Name} on Type {TargetProperty.ReflectedType} passed validation (ValidationType {ValidationType}). Was:", item);
                    }
                    else
                    {
                        _logger.LogObject<JsonProvider>(LogLevel.Debug, () => $"Element in Property {TargetProperty.Name} on Type {TargetProperty.ReflectedType} did not pass validation (ValidationType {ValidationType}). Was:", item);
                        errors.Add(executor.ErrorMessage((item, TargetProperty, parentObject)));
                    }
                }    
            }

          
            return (errors.HasValue(), errors);
        }
    }
}
