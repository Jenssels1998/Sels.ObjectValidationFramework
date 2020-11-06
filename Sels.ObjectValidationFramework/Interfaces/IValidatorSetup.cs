using Sels.ObjectValidationFramework.Validator;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Sels.ObjectValidationFramework
{
    public interface IValidatorSetup<TObject, TError>
    {
        #region Property
        /// <summary>
        /// Sets up validation on a property of TObject. Error message is created when propertyValueChecker returns false.
        /// </summary>
        /// <typeparam name="TPropertyValue">Return type of selected property</typeparam>
        /// <param name="property">Expression with object property as body</param>
        /// <param name="propertyValueChecker">Checks state of TPropertyValue</param>
        /// <param name="errorMessage">Delegate that creates TError when validation fails</param>
        /// <returns>Same instance of IValidator used to call this method</returns>
        IValidator<TObject, TError> AddValidValidation<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> property, Predicate<TPropertyValue> propertyValueChecker, Func<TPropertyValue, TObject, TError> errorMessage);
        /// <summary>
        /// Sets up validation on a property of TObject. Error message is created when propertyValueChecker returns true.
        /// </summary>
        /// <typeparam name="TPropertyValue">Return type of selected property</typeparam>
        /// <param name="property">Expression with object property as body</param>
        /// <param name="propertyValueChecker">Checks state of TPropertyValue</param>
        /// <param name="errorMessage">Delegate that creates TError when validation fails</param>
        /// <returns>Same instance of IValidator used to call this method</returns>
        IValidator<TObject, TError> AddInvalidValidation<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> property, Predicate<TPropertyValue> propertyValueChecker, Func<TPropertyValue, TObject, TError> errorMessage);
        /// <summary>
        /// Sets up validation on a property of TObject. Error message is created when validationType is Valid and propertyValueChecker returns false OR when validationType is Invalid and propertyValueChecker returns true.
        /// </summary>
        /// <typeparam name="TPropertyValue">Return type of selected property</typeparam>
        /// <param name="validationType">Enum that dictates when to generate the error message</param>
        /// <param name="property">Expression with object property as body</param>
        /// <param name="propertyValueChecker">Checks state of TPropertyValue</param>
        /// <param name="errorMessage">Delegate that creates TError when validation fails</param>
        /// <returns>Same instance of IValidator used to call this method</returns>
        /// <returns></returns>

        IValidator<TObject, TError> AddValidation<TPropertyValue>(ValidationType validationType, Expression<Func<TObject, TPropertyValue>> property, Predicate<TPropertyValue> propertyValueChecker, Func<TPropertyValue, TObject, TError> errorMessage);
        #endregion

        #region Collection Property
        /// <summary>
        /// Sets up validation on a IEnumerable<> property of TObject. Error message is created when elementValueChecker returns false.
        /// </summary>
        /// <typeparam name="TElement">Element type of collection</typeparam>
        /// <param name="property">IEnumerable<> property</param>
        /// <param name="elementValueChecker">Checks state of TElement</param>
        /// <param name="errorMessage">Delegate that creates TError when validation fails</param>
        /// <returns>Same instance of IValidator used to call this method</returns>
        IValidator<TObject, TError> AddValidCollectionValidation<TElement>(Expression<Func<TObject, IEnumerable<TElement>>> property, Predicate<TElement> elementValueChecker, Func<TElement, TObject, TError> errorMessage);
        /// <summary>
        /// Sets up validation on a IEnumerable<> property of TObject. Error message is created when elementValueChecker returns true.
        /// </summary>
        /// <typeparam name="TElement">Element type of collection</typeparam>
        /// <param name="property">IEnumerable<> property</param>
        /// <param name="elementValueChecker">Checks state of TElement</param>
        /// <param name="errorMessage">Delegate that creates TError when validation fails</param>
        /// <returns>Same instance of IValidator used to call this method</returns>
        IValidator<TObject, TError> AddInvalidCollectionValidation<TElement>(Expression<Func<TObject, IEnumerable<TElement>>> property, Predicate<TElement> elementValueChecker, Func<TElement, TObject, TError> errorMessage);
        /// <summary>
        /// Sets up validation on a IEnumerable<> property of TObject. Error message is created when validationType is Valid and elementValueChecker returns false OR when validationType is Invalid and propertyValueChecker returns true.
        /// </summary>
        /// <typeparam name="TElement">Element type of collection</typeparam>
        /// <param name="validationType">Enum that dictates when to generate the error message</param>
        /// <param name="property">IEnumerable<> property</param>
        /// <param name="elementValueChecker">Checks state of TElement</param>
        /// <param name="errorMessage">Delegate that creates TError when validation fails</param>
        /// <returns></returns>
        IValidator<TObject, TError> AddCollectionValidation<TElement>(ValidationType validationType, Expression<Func<TObject, IEnumerable<TElement>>> property, Predicate<TElement> elementValueChecker, Func<TElement, TObject, TError> errorMessage);
        #endregion

        #region Object
        /// <summary>
        /// Sets up validation on TObject. Error message is created when objectValueChecker returns false.
        /// </summary>
        /// <param name="objectValueChecker">Checks state of TObject</param>
        /// <param name="errorMessage">Delegate that creates TError when validation fails</param>
        /// <returns>Same instance of IValidator used to call this method</returns>
        IValidator<TObject, TError> AddValidValidation(Predicate<TObject> objectValueChecker, Func<TObject, TError> errorMessage);
        /// <summary>
        /// Sets up validation on TObject. Error message is created when objectValueChecker returns true.
        /// </summary>
        /// <param name="objectValueChecker">Checks state of TObject</param>
        /// <param name="errorMessage">Delegate that creates TError when validation fails</param>
        /// <returns>Same instance of IValidator used to call this method</returns>
        IValidator<TObject, TError> AddInvalidValidation(Predicate<TObject> objectValueChecker, Func<TObject, TError> errorMessage);
        /// <summary>
        /// Sets up validation on TObject. Error message is created when validationType is Valid and objectValueChecker returns false OR when validationType is Invalid and objectValueChecker returns true.
        /// </summary>
        /// <param name="validationType">Enum that dictates when to generate the error message</param>
        /// <param name="objectValueChecker">Checks state of TObject</param>
        /// <param name="errorMessage">Delegate that creates TError when validation fails</param>
        /// <returns>Same instance of IValidator used to call this method</returns>
        /// <returns></returns>

        IValidator<TObject, TError> AddValidation(ValidationType validationType, Predicate<TObject> objectValueChecker, Func<TObject, TError> errorMessage);
        #endregion
    }
}
