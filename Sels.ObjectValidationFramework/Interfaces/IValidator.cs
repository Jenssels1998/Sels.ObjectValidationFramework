using Sels.ObjectValidationFramework.Validator;
using Sels.ObjectValidationFramework.Validator.Case;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Sels.ObjectValidationFramework
{
    public interface IValidator<TObject, TError> : IValidatorSetup<TObject, TError>
    {
        /// <summary>
        /// Validation will only be performed if all conditions result in true.
        /// </summary>
        /// <param name="condition">Delegate to check object state</param>
        /// <returns>Same instance of IValidator used to call this method</returns>
        IValidator<TObject, TError> AddCondition(Predicate<TObject> condition);
        /// <summary>
        /// Ignores a property for validation. By default the ObjectValidator will check if any IValidators exist for the types on the properties and the underlying types on IEnumerable properties.
        /// </summary>
        /// <param name="property">Property that will be ignored</param>
        /// <returns>Same instance of IValidator used to call this method</returns>
        IValidator<TObject, TError> IgnorePropertyForValidation(Expression<Func<TObject, object>> property);
        /// <summary>
        /// This error will be returned when the validator receives a null object
        /// </summary>
        /// <param name="nullMessage">Delegate that creates TError</param>
        /// <returns>Same instance of IValidator used to call this method</returns>
        IValidator<TObject, TError> IfNull(Func<TError> nullMessage);
        /// <summary>
        /// Validation setup using validator will only be performed if condition returns true
        /// </summary>
        /// <param name="condition">Condition for validation</param>
        /// <param name="validator">Validator used to setup validation</param>
        /// <returns>Same instance of IValidator used to call this method</returns>
        IValidator<TObject, TError> ConditionalValidation(Predicate<TObject> condition, Action<IValidatorSetup<TObject, TError>> validator);
        /// <summary>
        /// Action performed before ObjectValidator starts validation on TObject
        /// </summary>
        /// <param name="action">Action performed</param>
        /// <returns>Same instance of IValidator used to call this method</returns>
        IValidator<TObject, TError> AddPreValidationAction(Action<TObject> action);
        /// <summary>
        /// Action performed after ObjectValidator validated TObject
        /// </summary>
        /// <param name="action">Action performed</param>
        /// <returns>Same instance of IValidator used to call this method</returns>
        IValidator<TObject, TError> AddPostValidationAction(Action<TObject, IEnumerable<TError>> action);
    }
}
