using Sels.Core.Extensions.General.Generic;
using Sels.Core.Extensions.General.Validation;
using Sels.Core.Extensions.Io.FileSystem;
using Sels.Core.Extensions.Object.ItemContainer;
using Sels.Core.Extensions.Object.String;
using Sels.Core.Extensions.Object.Time;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Sels.ObjectValidationFramework
{
    public static class ValidatorSetupExtensions
    {
        #region Property
        #region Generic 
        public static IValidator<TObject, TError> CannotBeNull<TObject, TPropertyValue, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, TPropertyValue>> property, Func<(TPropertyValue PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddInvalidValidation(property, x => x == null, errorMessage);
        }

        public static IValidator<TObject, TError> CannotBeDefault<TObject, TPropertyValue, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, TPropertyValue>> property, Func<(TPropertyValue PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddInvalidValidation(property, x => x.IsDefault(), errorMessage);
        }

        public static IValidator<TObject, TError> MustBeNull<TObject, TPropertyValue, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, TPropertyValue>> property, Func<(TPropertyValue PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x == null, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeDefault<TObject, TPropertyValue, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, TPropertyValue>> property, Func<(TPropertyValue PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x.IsDefault(), errorMessage);
        }

        #endregion
        #region String
        public static IValidator<TObject, TError> CannotBeNullOrEmpty<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddInvalidValidation(property, x => string.IsNullOrEmpty(x), errorMessage);
        }

        public static IValidator<TObject, TError> CannotBeNullOrWhiteSpace<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddInvalidValidation(property, x => string.IsNullOrWhiteSpace(x), errorMessage);
        }

        public static IValidator<TObject, TError> MustMatchRegex<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, string regexPattern, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            regexPattern.ValidateVariable(nameof(regexPattern));
            var regex = new Regex(regexPattern);
            return validator.AddValidValidation(property, x => regex.IsMatch(x), errorMessage);
        }

        public static IValidator<TObject, TError> MustMatchRegex<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, Regex regex, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            regex.ValidateVariable(nameof(regex));
            return validator.AddValidValidation(property, x => regex.IsMatch(x), errorMessage);
        }

        public static IValidator<TObject, TError> CannotMatchRegex<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, string regexPattern, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            regexPattern.ValidateVariable(nameof(regexPattern));
            var regex = new Regex(regexPattern);
            return validator.AddInvalidValidation(property, x => regex.IsMatch(x), errorMessage);
        }

        public static IValidator<TObject, TError> CannotMatchRegex<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, Regex regex, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            regex.ValidateVariable(nameof(regex));
            return validator.AddInvalidValidation(property, x => regex.IsMatch(x), errorMessage);
        }

        public static IValidator<TObject, TError> MustHaveMinLength<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, int minLength, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            minLength.ValidateVariable(nameof(minLength));
            return validator.AddValidValidation(property, x => x.Length >= minLength, errorMessage);
        }

        public static IValidator<TObject, TError> MustHaveMaxLength<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, int maxLength, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            maxLength.ValidateVariable(nameof(maxLength));
            return validator.AddValidValidation(property, x => x.Length <= maxLength, errorMessage);
        }

        public static IValidator<TObject, TError> MustHaveLengthBetween<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, int minLength, int maxLength, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            minLength.ValidateVariable(nameof(minLength));
            maxLength.ValidateVariable(nameof(maxLength));
            return validator.AddValidValidation(property, x => x.Length > minLength && x.Length < maxLength, errorMessage);
        }

        public static IValidator<TObject, TError> CannotContain<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, char[] invalidChars, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            invalidChars.ValidateVariable(nameof(invalidChars));
            return validator.AddInvalidValidation(property, x => x.Contains(invalidChars), errorMessage);
        }

        public static IValidator<TObject, TError> CannotContainAll<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, char[] invalidChars, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            invalidChars.ValidateVariable(nameof(invalidChars));
            return validator.AddInvalidValidation(property, x => x.ContainsAll(invalidChars), errorMessage);
        }

        public static IValidator<TObject, TError> MustContain<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, char[] requiredStrings, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            requiredStrings.ValidateVariable(nameof(requiredStrings));
            return validator.AddValidValidation(property, x => x.Contains(requiredStrings), errorMessage);
        }

        public static IValidator<TObject, TError> MustContainAll<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, char[] requiredStrings, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            requiredStrings.ValidateVariable(nameof(requiredStrings));
            return validator.AddValidValidation(property, x => x.ContainsAll(requiredStrings), errorMessage);
        }

        public static IValidator<TObject, TError> CannotContain<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, string[] invalidStrings, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            invalidStrings.ValidateVariable(nameof(invalidStrings));
            return validator.AddInvalidValidation(property, x => x.Contains(invalidStrings), errorMessage);
        }

        public static IValidator<TObject, TError> CannotContainAll<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, string[] invalidStrings, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            invalidStrings.ValidateVariable(nameof(invalidStrings));
            return validator.AddInvalidValidation(property, x => x.ContainsAll(invalidStrings), errorMessage);
        }

        public static IValidator<TObject, TError> MustContain<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, string[] requiredStrings, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            requiredStrings.ValidateVariable(nameof(requiredStrings));
            return validator.AddValidValidation(property, x => x.Contains(requiredStrings), errorMessage);
        }

        public static IValidator<TObject, TError> MustContainAll<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, string[] requiredStrings, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            requiredStrings.ValidateVariable(nameof(requiredStrings));
            return validator.AddValidValidation(property, x => x.ContainsAll(requiredStrings), errorMessage);
        }
        #endregion
        #region DateTime
        private const string DateFormat = "yyyyMMdd";

        public static IValidator<TObject, TError> MustBeInPast<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, DateTime>> property, Func<(DateTime PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x.IsInPast() && !x.IsToday(), errorMessage);
        }

        public static IValidator<TObject, TError> MustBeInFuture<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, DateTime>> property, Func<(DateTime PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x.IsInFuture() && !x.IsToday(), errorMessage);
        }

        public static IValidator<TObject, TError> MustBeTodayOrInPast<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, DateTime>> property, Func<(DateTime PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x.IsInPast() || x.IsToday(), errorMessage);
        }

        public static IValidator<TObject, TError> MustBeTodayOrInFuture<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, DateTime>> property, Func<(DateTime PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x.IsInFuture() || x.IsToday(), errorMessage);
        }

        public static IValidator<TObject, TError> MustBeToday<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, DateTime>> property, Func<(DateTime PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x.IsToday(), errorMessage);
        }

        public static IValidator<TObject, TError> MustBeTomorrow<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, DateTime>> property, Func<(DateTime PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x.ToString(DateFormat).Equals(DateTime.Now.AddDays(1).ToString(DateFormat)), errorMessage);
        }
        public static IValidator<TObject, TError> MustBeBetween<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, DateTime>> property, DateTime minDate, DateTime maxDate, Func<(DateTime PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x >= minDate && x <= maxDate, errorMessage);
        }


        public static IValidator<TObject, TError> MustBeInPast<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, DateTime?>> property, Func<(DateTime? PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x.HasValue && x.Value.IsInPast() && !x.Value.IsToday(), errorMessage);
        }

        public static IValidator<TObject, TError> MustBeInFuture<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, DateTime?>> property, Func<(DateTime? PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x.HasValue && x.Value.IsInFuture() && !x.Value.IsToday(), errorMessage);
        }

        public static IValidator<TObject, TError> MustBeTodayOrInPast<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, DateTime?>> property, Func<(DateTime? PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x.HasValue && x.Value.IsInPast() || x.Value.IsToday(), errorMessage);
        }

        public static IValidator<TObject, TError> MustBeTodayOrInFuture<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, DateTime?>> property, Func<(DateTime? PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x.HasValue && x.Value.IsInFuture() || x.Value.IsToday(), errorMessage);
        }

        public static IValidator<TObject, TError> MustBeToday<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, DateTime?>> property, Func<(DateTime? PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x.HasValue && x.Value.IsToday(), errorMessage);
        }

        public static IValidator<TObject, TError> MustBeTomorrow<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, DateTime?>> property, Func<(DateTime? PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x.HasValue && x.Value.ToString(DateFormat).Equals(DateTime.Now.AddDays(1).ToString(DateFormat)), errorMessage);
        }

        public static IValidator<TObject, TError> MustBeBetween<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, DateTime?>> property, DateTime minDate, DateTime maxDate, Func<(DateTime? PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x.HasValue && x.Value >= minDate && x.Value <= maxDate, errorMessage);
        }
        #endregion
        #region Numeric
        public static IValidator<TObject, TError> MustBePositive<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, double>> property, Func<(double PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x > 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeZeroOrPositive<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, double>> property, Func<(double PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x >= 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeNegative<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, double>> property, Func<(double PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x < 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeZeroOrNegative<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, double>> property, Func<(double PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x <= 0, errorMessage);
        }

        public static IValidator<TObject, TError> CannotBeZero<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, double>> property, Func<(double PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddInvalidValidation(property, x => x == 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeBetween<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, double>> property, double min, double max, Func<(double PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x > min && x < max, errorMessage);
        }

        public static IValidator<TObject, TError> MustBePositive<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, decimal>> property, Func<(decimal PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x > 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeZeroOrPositive<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, decimal>> property, Func<(decimal PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x >= 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeNegative<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, decimal>> property, Func<(decimal PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x < 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeZeroOrNegative<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, decimal>> property, Func<(decimal PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x <= 0, errorMessage);
        }

        public static IValidator<TObject, TError> CannotBeZero<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, decimal>> property, Func<(decimal PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddInvalidValidation(property, x => x == 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeBetween<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, decimal>> property, decimal min, decimal max, Func<(decimal PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x > min && x < max, errorMessage);
        }
        #endregion
        #region Collection
        public static IValidator<TObject, TError> CannotBeEmpty<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<TElement>>> property, Func<(IEnumerable<TElement> PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x != null && x.Count() > 0, errorMessage);
        }
        public static IValidator<TObject, TError> CannotBeEmpty<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, ICollection<TElement>>> property, Func<(ICollection<TElement> PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x != null && x.Count > 0, errorMessage);
        }
        public static IValidator<TObject, TError> CannotBeEmpty<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, TElement[]>> property, Func<(TElement[] PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x != null && x.Length > 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeEmpty<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<TElement>>> property, Func<(IEnumerable<TElement> PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x == null && x.Count() == 0, errorMessage);
        }
        public static IValidator<TObject, TError> MustBeEmpty<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, ICollection<TElement>>> property, Func<(ICollection<TElement> PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x == null && x.Count() == 0, errorMessage);
        }
        public static IValidator<TObject, TError> MustBeEmpty<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, TElement[]>> property, Func<(TElement[] PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property,x => x == null && x.Count() == 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustContainAtLeast<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<TElement>>> property, int minAmount, Func<(IEnumerable<TElement> PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            minAmount.ValidateVariable(nameof(minAmount));
            return validator.AddValidValidation(property, x => x != null && x.Count() >= minAmount, errorMessage);
        }
        public static IValidator<TObject, TError> MustContainAtLeast<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, ICollection<TElement>>> property, int minAmount, Func<(ICollection<TElement> PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            minAmount.ValidateVariable(nameof(minAmount));
            return validator.AddValidValidation(property, x => x != null && x.Count >= minAmount, errorMessage);
        }
        public static IValidator<TObject, TError> MustContainAtLeast<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, TElement[]>> property, int minAmount, Func<(TElement[] PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            minAmount.ValidateVariable(nameof(minAmount));
            return validator.AddValidValidation(property, x => x != null && x.Length >= minAmount, errorMessage);
        }

        public static IValidator<TObject, TError> MustContainAtMax<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<TElement>>> property, int maxAmount, Func<(IEnumerable<TElement> PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            maxAmount.ValidateVariable(nameof(maxAmount));
            return validator.AddValidValidation(property, x => x != null && x.Count() <= maxAmount, errorMessage);
        }
        public static IValidator<TObject, TError> MustContainAtMax<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, ICollection<TElement>>> property, int maxAmount, Func<(ICollection<TElement> PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            maxAmount.ValidateVariable(nameof(maxAmount));
            return validator.AddValidValidation(property, x => x != null && x.Count <= maxAmount, errorMessage);
        }
        public static IValidator<TObject, TError> MustContainAtMax<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, TElement[]>> property, int maxAmount, Func<(TElement[] PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            maxAmount.ValidateVariable(nameof(maxAmount));
            return validator.AddValidValidation(property, x => x != null && x.Length <= maxAmount, errorMessage);
        }

        public static IValidator<TObject, TError> MustContainBetween<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<TElement>>> property, int minAmount, int maxAmount, Func<(IEnumerable<TElement> PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            maxAmount.ValidateVariable(nameof(maxAmount));
            minAmount.ValidateVariable(nameof(minAmount));
            return validator.AddValidValidation(property, x => x != null && x.Count() >= minAmount && x.Count() <= maxAmount, errorMessage);
        }
        public static IValidator<TObject, TError> MustContainBetween<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, ICollection<TElement>>> property, int minAmount, int maxAmount, Func<(ICollection<TElement> PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            maxAmount.ValidateVariable(nameof(maxAmount));
            minAmount.ValidateVariable(nameof(minAmount));
            return validator.AddValidValidation(property, x => x != null && x.Count >= minAmount && x.Count <= maxAmount, errorMessage);
        }
        public static IValidator<TObject, TError> MustContainBetween<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, TElement[]>> property, int minAmount, int maxAmount, Func<(TElement[] PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            maxAmount.ValidateVariable(nameof(maxAmount));
            minAmount.ValidateVariable(nameof(minAmount));
            return validator.AddValidValidation(property, x => x != null && x.Length >= minAmount && x.Length <= maxAmount, errorMessage);
        }

        public static IValidator<TObject, TError> AllElementsMustBeUnique<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<TElement>>> property, Func<(IEnumerable<TElement> PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => x.AreAllUnique(), errorMessage);
        }
        #endregion
        #region IO 
        public static IValidator<TObject, TError> MustExistAndCannotBeNull<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, FileSystemInfo>> property, Func<(FileSystemInfo PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddInvalidValidation(property, x => x == null || !x .Exists, errorMessage);
        }

        public static IValidator<TObject, TError> IsValidFileName<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => File.Exists(x), errorMessage);
        }

        public static IValidator<TObject, TError> IsValidDirectory<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => Directory.Exists(x), errorMessage);
        }

        public static IValidator<TObject, TError> FileCanBeOpened<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, FileInfo>> property, Func<(FileInfo PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => !x.Exists || !x.IsLocked(), errorMessage);
        }

        public static IValidator<TObject, TError> FileCanBeOpened<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, string>> property, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidValidation(property, x => { var file = new FileInfo(x); return !file.Exists || !file.IsLocked(); }, errorMessage);
        }
        #endregion
        #endregion

        #region Collection Property
        #region Generic 
        public static IValidator<TObject, TError> ElementCannotBeNull<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<TElement>>> property, Func<(TElement ElementValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddInvalidCollectionValidation(property, x => x == null, errorMessage);
        }

        public static IValidator<TObject, TError> ElementCannotBeDefault<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<TElement>>> property, Func<(TElement ElementValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddInvalidCollectionValidation(property, x => x.IsDefault(), errorMessage);
        }

        public static IValidator<TObject, TError> ElementMustBeNull<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<TElement>>> property, Func<(TElement ElementValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x == null, errorMessage);
        }

        public static IValidator<TObject, TError> ElementMustBeDefault<TObject, TElement, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<TElement>>> property, Func<(TElement ElementValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x.IsDefault(), errorMessage);
        }

        #endregion
        #region String
        public static IValidator<TObject, TError> CannotBeNullOrEmpty<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddInvalidCollectionValidation(property, x => string.IsNullOrEmpty(x), errorMessage);
        }

        public static IValidator<TObject, TError> CannotBeNullOrWhiteSpace<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddInvalidCollectionValidation(property, x => string.IsNullOrWhiteSpace(x), errorMessage);
        }

        public static IValidator<TObject, TError> MustMatchRegex<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, string regexPattern, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            regexPattern.ValidateVariable(nameof(regexPattern));
            var regex = new Regex(regexPattern);
            return validator.AddValidCollectionValidation(property, x => regex.IsMatch(x), errorMessage);
        }

        public static IValidator<TObject, TError> MustMatchRegex<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, Regex regex, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            regex.ValidateVariable(nameof(regex));
            return validator.AddValidCollectionValidation(property, x => regex.IsMatch(x), errorMessage);
        }

        public static IValidator<TObject, TError> CannotMatchRegex<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, string regexPattern, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            regexPattern.ValidateVariable(nameof(regexPattern));
            var regex = new Regex(regexPattern);
            return validator.AddInvalidCollectionValidation(property, x => regex.IsMatch(x), errorMessage);
        }

        public static IValidator<TObject, TError> CannotMatchRegex<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, Regex regex, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            regex.ValidateVariable(nameof(regex));
            return validator.AddInvalidCollectionValidation(property, x => regex.IsMatch(x), errorMessage);
        }

        public static IValidator<TObject, TError> MustHaveMinLength<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, int minLength, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            minLength.ValidateVariable(nameof(minLength));
            return validator.AddValidCollectionValidation(property, x => x.Length >= minLength, errorMessage);
        }

        public static IValidator<TObject, TError> MustHaveMaxLength<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, int maxLength, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            maxLength.ValidateVariable(nameof(maxLength));
            return validator.AddValidCollectionValidation(property, x => x.Length <= maxLength, errorMessage);
        }

        public static IValidator<TObject, TError> MustHaveLengthBetween<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, int minLength, int maxLength, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            minLength.ValidateVariable(nameof(minLength));
            maxLength.ValidateVariable(nameof(maxLength));
            return validator.AddValidCollectionValidation(property, x => x.Length > minLength && x.Length < maxLength, errorMessage);
        }

        public static IValidator<TObject, TError> CannotContain<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, char[] invalidChars, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            invalidChars.ValidateVariable(nameof(invalidChars));
            return validator.AddInvalidCollectionValidation(property, x => x.Contains(invalidChars), errorMessage);
        }

        public static IValidator<TObject, TError> CannotContainAll<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, char[] invalidChars, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            invalidChars.ValidateVariable(nameof(invalidChars));
            return validator.AddInvalidCollectionValidation(property, x => x.ContainsAll(invalidChars), errorMessage);
        }

        public static IValidator<TObject, TError> MustContain<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, char[] requiredStrings, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            requiredStrings.ValidateVariable(nameof(requiredStrings));
            return validator.AddValidCollectionValidation(property, x => x.Contains(requiredStrings), errorMessage);
        }

        public static IValidator<TObject, TError> MustContainAll<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, char[] requiredStrings, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            requiredStrings.ValidateVariable(nameof(requiredStrings));
            return validator.AddValidCollectionValidation(property, x => x.ContainsAll(requiredStrings), errorMessage);
        }

        public static IValidator<TObject, TError> CannotContain<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, string[] invalidStrings, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            invalidStrings.ValidateVariable(nameof(invalidStrings));
            return validator.AddInvalidCollectionValidation(property, x => x.Contains(invalidStrings), errorMessage);
        }

        public static IValidator<TObject, TError> CannotContainAll<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, string[] invalidStrings, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            invalidStrings.ValidateVariable(nameof(invalidStrings));
            return validator.AddInvalidCollectionValidation(property, x => x.ContainsAll(invalidStrings), errorMessage);
        }

        public static IValidator<TObject, TError> MustContain<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, string[] requiredStrings, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            requiredStrings.ValidateVariable(nameof(requiredStrings));
            return validator.AddValidCollectionValidation(property, x => x.Contains(requiredStrings), errorMessage);
        }

        public static IValidator<TObject, TError> MustContainAll<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<string>>> property, string[] requiredStrings, Func<(string PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            requiredStrings.ValidateVariable(nameof(requiredStrings));
            return validator.AddValidCollectionValidation(property, x => x.ContainsAll(requiredStrings), errorMessage);
        }
        #endregion
        #region DateTime
        public static IValidator<TObject, TError> MustBeInPast<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<DateTime>>> property, Func<(DateTime PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x < DateTime.Now, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeInFuture<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<DateTime>>> property, Func<(DateTime PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x > DateTime.Now, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeToday<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<DateTime>>> property, Func<(DateTime PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x.ToString(DateFormat).Equals(DateTime.Now.ToString(DateFormat)), errorMessage);
        }

        public static IValidator<TObject, TError> MustBeTomorrow<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<DateTime>>> property, Func<(DateTime PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x.ToString(DateFormat).Equals(DateTime.Now.AddDays(1).ToString(DateFormat)), errorMessage);
        }

        public static IValidator<TObject, TError> MustBeBetween<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<DateTime>>> property, DateTime minDate, DateTime maxDate, Func<(DateTime PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x >= minDate && x <= maxDate, errorMessage);
        }
        #endregion
        #region Numeric
        public static IValidator<TObject, TError> MustBePositive<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<double>>> property, Func<(double PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x > 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeZeroOrPositive<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<double>>> property, Func<(double PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x >= 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeNegative<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<double>>> property, Func<(double PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x < 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeZeroOrNegative<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<double>>> property, Func<(double PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x <= 0, errorMessage);
        }

        public static IValidator<TObject, TError> CannotBeZero<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<double>>> property, Func<(double PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddInvalidCollectionValidation(property, x => x == 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeBetween<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<double>>> property, double min, double max, Func<(double PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x > min && x < max, errorMessage);
        }

        public static IValidator<TObject, TError> MustBePositive<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<decimal>>> property, Func<(decimal PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x > 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeZeroOrPositive<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<decimal>>> property, Func<(decimal PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x >= 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeNegative<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<decimal>>> property, Func<(decimal PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x < 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeZeroOrNegative<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<decimal>>> property, Func<(decimal PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x <= 0, errorMessage);
        }

        public static IValidator<TObject, TError> CannotBeZero<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<decimal>>> property, Func<(decimal PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddInvalidCollectionValidation(property, x => x == 0, errorMessage);
        }

        public static IValidator<TObject, TError> MustBeBetween<TObject, TError>(this IValidatorSetup<TObject, TError> validator, Expression<Func<TObject, IEnumerable<decimal>>> property, decimal min, decimal max, Func<(decimal PropertyValue, PropertyInfo Property, TObject Object), TError> errorMessage)
        {
            return validator.AddValidCollectionValidation(property, x => x > min && x < max, errorMessage);
        }
        #endregion
        #endregion
        #region Object

        #endregion
    }
}
