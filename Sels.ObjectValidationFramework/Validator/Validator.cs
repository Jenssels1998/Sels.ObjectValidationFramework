using Microsoft.Extensions.Logging;
using Sels.Core.Extensions.General.Validation;
using Sels.Core.Extensions.Logging;
using Sels.Core.Extensions.Object.Time;
using Sels.ObjectValidationFramework.Validator.Case;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Sels.Core.Components.Caching;
using System.Collections.ObjectModel;
using System.Reflection;
using Sels.Core.Extensions.Reflection.Object;
using Sels.Core.Extensions.General.Generic;
using System.Linq;
using Sels.Core.Extensions.Reflection.Expressions;
using Sels.Core.Components.Variable.Actions;
using Sels.ObjectValidationFramework.Validator.Object;
using Sels.Core.Extensions.Execution.Linq;
using Sels.ObjectValidationFramework.Validator.Property.Collection;

namespace Sels.ObjectValidationFramework.Validator
{
    public class Validator<TObject, TError> : BaseValidator, IValidator<TObject, TError>
    {
        // Fields
        private readonly ILogger _logger;
        private readonly List<BasePropertyValidationExecutor<TObject>> _propertyValidationExecutors = new List<BasePropertyValidationExecutor<TObject>>();
        private readonly List<BasePropertyValidationExecutor<TObject>> _propertyCollectionValidationExecutors = new List<BasePropertyValidationExecutor<TObject>>();
        private readonly List<BaseValidationExecutor<TObject>> _validationExecutors = new List<BaseValidationExecutor<TObject>>();
        private readonly List<Predicate<TObject>> _conditions = new List<Predicate<TObject>>();
        private readonly List<Action<TObject>> _preValidationActions = new List<Action<TObject>>();
        private readonly List<Action<TObject, IEnumerable<TError>>> _postValidationActions = new List<Action<TObject, IEnumerable<TError>>>();
        private Func<TError> _nullMessage;

        // Properties
        internal override Type TargetObjectType => typeof(TObject);
        internal override Delegate ValidateDelegate { get; }

        // State
        internal Predicate<TObject> _validationCondition;
       
        internal Validator(ILogger logger)
        {
            logger.ValidateVariable(nameof(logger));

            _logger = logger;

            ValidateDelegate = this.CreateDelegateForMethod(nameof(Validate));
        }

        #region Validation Setup
        public IValidator<TObject, TError> AddCondition(Predicate<TObject> condition)
        {
            _logger.LogMessage(LogLevel.Information, () => $"Adding condition for validation of Object({typeof(TObject)})");
            condition.ValidateVariable(nameof(condition));

            _conditions.Add(condition);

            return this;
        }
        public IValidator<TObject, TError> IfNull(Func<TError> nullMessage)
        {
            _logger.LogMessage(LogLevel.Information, () => $"Adding null message for Object({typeof(TObject)})");
            _nullMessage = nullMessage;

            return this;
        }
        public IValidator<TObject, TError> ConditionalValidation(Predicate<TObject> condition, Action<IValidatorSetup<TObject, TError>> validator)
        {
            condition.ValidateVariable(nameof(condition));
            validator.ValidateVariable(nameof(validator));
            _validationCondition.ValidateVariable(x => x.IsDefault(), x => new NotSupportedException("Double conditional validation is not supported"));

            using (var logger = _logger.CreateTimedLogger(LogLevel.Information, () => $"Adding conditional validation for Object({typeof(TObject)})", x => $"Added conditional validation for Object({typeof(TObject)}) in {x.PrintTotalMs()}"))
            {
                // Set global condition and clear it with dispose. Created validation using validator will have condition injected
                using (new VariableClearer<Predicate<TObject>>(condition, x => _validationCondition = x))
                {
                    validator(this);
                }
            }
                
            return this;
        }
        public IValidator<TObject, TError> AddPreValidationAction(Action<TObject> action)
        {
            action.ValidateVariable(nameof(action));

            _preValidationActions.Add(action);

            return this;
        }
        public IValidator<TObject, TError> AddPostValidationAction(Action<TObject, IEnumerable<TError>> action) 
        {
            action.ValidateVariable(nameof(action));

            _postValidationActions.Add(action);

            return this;
        }
        #region Property
        public IValidator<TObject, TError> AddValidValidation<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> property, Predicate<TPropertyValue> propertyValueChecker, Func<TPropertyValue, TObject, TError> errorMessage)
        {
            return AddValidation(ValidationType.Valid, property, propertyValueChecker, errorMessage);
        }

        public IValidator<TObject, TError> AddInvalidValidation<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> property, Predicate<TPropertyValue> propertyValueChecker, Func<TPropertyValue, TObject, TError> errorMessage)
        {
            return AddValidation(ValidationType.Invalid, property, propertyValueChecker, errorMessage);
        }

        public IValidator<TObject, TError> AddValidation<TPropertyValue>(ValidationType validationType, Expression<Func<TObject, TPropertyValue>> property, Predicate<TPropertyValue> propertyValueChecker, Func<TPropertyValue, TObject, TError> errorMessage)
        {
            property.ValidateVariable(nameof(property));
            propertyValueChecker.ValidateVariable(nameof(propertyValueChecker));
            errorMessage.ValidateVariable(nameof(errorMessage));

            using (var logger = _logger.CreateTimedLogger(LogLevel.Information, () => $"Adding property validation case for Object({typeof(TObject)})", x => $"Added property validation case for Object({typeof(TObject)}) in {x.PrintTotalMs()}"))
            {
                try
                {
                    var propertyInfo = property.ExtractProperty(nameof(property));

                    var validationCase = new PropertyValidationExecutor<TObject, TPropertyValue, TError>(propertyInfo, propertyValueChecker, errorMessage, validationType, _validationCondition, _logger);

                    _propertyValidationExecutors.Add(validationCase);
                }
                catch (Exception ex)
                {
                    logger.EndLog(x => $"Could not add validation case Object({typeof(TObject)}) ({x.PrintTotalMs()})", ex);
                    throw;
                }
            }

            return this;
        }
        #endregion

        #region Property Collection
        public IValidator<TObject, TError> AddValidCollectionValidation<TElement>(Expression<Func<TObject, IEnumerable<TElement>>> property, Predicate<TElement> elementValueChecker, Func<TElement, TObject, TError> errorMessage)
        {
            return AddCollectionValidation(ValidationType.Valid, property, elementValueChecker, errorMessage);
        }

        public IValidator<TObject, TError> AddInvalidCollectionValidation<TElement>(Expression<Func<TObject, IEnumerable<TElement>>> property, Predicate<TElement> elementValueChecker, Func<TElement, TObject, TError> errorMessage)
        {
            return AddCollectionValidation(ValidationType.Invalid, property, elementValueChecker, errorMessage);
        }

        public IValidator<TObject, TError> AddCollectionValidation<TElement>(ValidationType validationType, Expression<Func<TObject, IEnumerable<TElement>>> property, Predicate<TElement> elementValueChecker, Func<TElement, TObject, TError> errorMessage)
        {
            property.ValidateVariable(nameof(property));
            elementValueChecker.ValidateVariable(nameof(elementValueChecker));
            errorMessage.ValidateVariable(nameof(errorMessage));

            using (var logger = _logger.CreateTimedLogger(LogLevel.Information, () => $"Adding collection property validation case for Object({typeof(TObject)})", x => $"Added collection property validation case for Object({typeof(TObject)}) in {x.PrintTotalMs()}"))
            {
                try
                {
                    var propertyInfo = property.ExtractProperty(nameof(property));

                    // Check if we already have validator for this property
                    foreach(var collectionValidator in _propertyCollectionValidationExecutors)
                    {
                        if(collectionValidator is CollectionPropertyValidationExecutor<TObject, TElement, TError> typedCollectionValidator && typedCollectionValidator.TargetProperty.Equals(propertyInfo))
                        {
                            typedCollectionValidator.AddValidatorExecutor((elementValueChecker, errorMessage));
                            return this;
                        }
                    }

                    var validationCase = new CollectionPropertyValidationExecutor<TObject, TElement, TError>(propertyInfo, (elementValueChecker, errorMessage), validationType, _validationCondition, _logger);

                    _propertyCollectionValidationExecutors.Add(validationCase);
                }
                catch (Exception ex)
                {
                    logger.EndLog(x => $"Could not add validation case Object({typeof(TObject)}) ({x.PrintTotalMs()})", ex);
                    throw;
                }
            }

            return this;
        }
        #endregion

        #region Object
        public IValidator<TObject, TError> AddValidValidation(Predicate<TObject> objectValueChecker, Func<TObject, TError> errorMessage)
        {
            return AddValidation(ValidationType.Valid, objectValueChecker, errorMessage);
        }

        public IValidator<TObject, TError> AddInvalidValidation(Predicate<TObject> objectValueChecker, Func<TObject, TError> errorMessage)
        {
            return AddValidation(ValidationType.Invalid, objectValueChecker, errorMessage);
        }

        public IValidator<TObject, TError> AddValidation(ValidationType validationType, Predicate<TObject> objectValueChecker, Func<TObject, TError> errorMessage)
        {
            objectValueChecker.ValidateVariable(nameof(objectValueChecker));
            errorMessage.ValidateVariable(nameof(errorMessage));

            using (var logger = _logger.CreateTimedLogger(LogLevel.Information, () => $"Adding validation for Object({typeof(TObject)})", x => $"Added validation for Object({typeof(TObject)}) in {x.PrintTotalMs()}"))
            {
                try
                {
                    var validationCase = new ObjectValidationExecutor<TObject, TError>(objectValueChecker, errorMessage, validationType, _validationCondition, _logger);

                    _validationExecutors.Add(validationCase);
                }
                catch (Exception ex)
                {
                    logger.EndLog(x => $"Could not add validation Object({typeof(TObject)}) ({x.PrintTotalMs()})", ex);
                    throw;
                }
            }

            return this;
        }
        #endregion
        #endregion

        #region Validation Execution
        private IEnumerable<TError> ValidateProperties(TObject value)
        {
            var errors = new List<TError>();
            var properties = value.GetProperties();

            using (var timedLogger = _logger.CreateTimedLogger(LogLevel.Information, () => $"Validating properties on Object({typeof(TObject)})", x => $"Validated properties on Object({typeof(TObject)}) in {x.PrintTotalMs()}"))
            {
                if (properties.HasValue() && _propertyValidationExecutors.HasValue())
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var property = properties[i];

                        // Get all validation cases for this property
                        foreach (var validationCase in _propertyValidationExecutors.Where(x => x.TargetProperty.Equals(property) && x.CanRun(value)))
                        {
                            var (hasError, error) = validationCase.ValidateDelegate.Invoke<(bool HasError, TError Error)>(property.GetValue(value), value);
                            if (hasError)
                            {
                                errors.Add(error);
                            }
                        }
                    }
                }
            }

            return errors;
        }

        private IEnumerable<TError> ValidateCollectionProperties(TObject value)
        {
            var errors = new List<TError>();
            var properties = value.GetProperties();

            using (var timedLogger = _logger.CreateTimedLogger(LogLevel.Information, () => $"Validating properties on Object({typeof(TObject)})", x => $"Validated properties on Object({typeof(TObject)}) in {x.PrintTotalMs()}"))
            {
                if (properties.HasValue() && _propertyCollectionValidationExecutors.HasValue())
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var property = properties[i];

                        // Get all validation cases for this property
                        foreach (var validationCase in _propertyCollectionValidationExecutors.Where(x => x.TargetProperty.Equals(property) && x.CanRun(value)))
                        {
                            var (hasError, validationErrors) = validationCase.ValidateDelegate.Invoke<(bool HasError, IEnumerable<TError> Errors)>(property.GetValue(value), value);
                            if (hasError)
                            {
                                errors.AddRange(validationErrors);
                            }
                        }
                    }
                }
            }

            return errors;
        }

        private IEnumerable<TError> ValidateObject(TObject value)
        {
            var errors = new List<TError>();
            var properties = value.GetProperties();

            using (var timedLogger = _logger.CreateTimedLogger(LogLevel.Information, () => $"Validating Object({typeof(TObject)})", x => $"Validated Object({typeof(TObject)}) in {x.PrintTotalMs()}"))
            {
                if (_validationExecutors.HasValue())
                {
                    // Get all validation cases for this object
                    foreach (var validationCase in _validationExecutors.Where(x => x.CanRun(value)))
                    {
                        var (hasError, error) = validationCase.ValidateDelegate.Invoke<(bool HasError, TError Error)>(value);
                        if (hasError)
                        {
                            errors.Add(error);
                        }
                    }
                }            
            }

            return errors;
        }

        internal IEnumerable<TError> Validate(TObject value)
        {
            var errors = new List<TError>();

            using(var timedLogger = _logger.CreateTimedLogger(LogLevel.Information, () => $"Running validation on Object({typeof(TObject)})", x => $"Ran validation on Object({typeof(TObject)}) in {x.PrintTotalMs()}"))
            {
                // Return if TObject is null.
                if (value == null)
                {
                    if (_nullMessage.HasValue())
                    {
                        errors.Add(_nullMessage());
                    }

                    return errors;
                }

                // Don't validate if any condition fails
                foreach (var condition in _conditions)
                {
                    if (!condition(value))
                    {
                        return errors;
                    }
                }

                // Execute pre validation actions
                _preValidationActions.Execute(x => x(value));

                errors.AddRange(ValidateProperties(value));
                errors.AddRange(ValidateCollectionProperties(value));
                errors.AddRange(ValidateObject(value));

                // Execute post validation actions
                _postValidationActions.Execute(x => x(value, errors));

                return errors;
            }
        }
        #endregion
    }
}
