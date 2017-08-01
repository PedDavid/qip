using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API.Services.Utils {
    public class ValidatorConfiguration<T> {
        public readonly List<KeyValuePair<string, Func<T, bool>>> validations = new List<KeyValuePair<string, Func<T, bool>>>();

        public ValidatorConfiguration<T> NotNull<F>(string fieldName, Func<T, F> fieldExtractor) {
            validations.Add(
                new KeyValuePair<string, Func<T, bool>>(
                    fieldName,
                    t => fieldExtractor(t) != null
                )
            );
            return this;
        }

        public ValidatorConfiguration<T> UseValidator<F>(string fieldName, Func<T, IEnumerable<F>> fieldExtractor, Validator<F> validator) {
            validations.Add(
                new KeyValuePair<string, Func<T, bool>>(
                    fieldName,
                    t => fieldExtractor(t).All(f => validator.TryValid(f))
                )
            );
            return this;
        }

        public ValidatorConfiguration<T> UseValidator<F>(string fieldName, Func<T, IEnumerable<F>> fieldExtractor, ValidatorConfiguration<F> configSubValidator) {
            validations.Add(
                new KeyValuePair<string, Func<T, bool>>(
                    fieldName,
                    t => fieldExtractor(t).All(f => Validator<F>.TryValid(f, configSubValidator))
                )
            );
            return this;
        }
    }
}
