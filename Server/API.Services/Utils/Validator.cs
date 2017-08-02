using API.Interfaces.ServicesExceptions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API.Services.Utils {
    public class Validator<T> {//https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation -> Pensar em usar em vez desta class
        private readonly ValidatorConfiguration<T> _config;

        public Validator(ValidatorConfiguration<T> config) {
            _config = config;
        }

        public void Valid(T target) {
            Valid(target, _config);
        }

        public bool TryValid(T target) {
            return TryValid(target, _config);
        }

        public static void Valid(T target, ValidatorConfiguration<T> config) {
            IEnumerable<string> invalidFields = config.validations
                .Where(p => !p.Value.Invoke(target))
                .Select(p => p.Key)
                .Distinct();

            if(invalidFields.Count() != 0) {
                string separator = ", ";

                string msg = invalidFields.Aggregate(
                    new StringBuilder("The following fields are invalid: "),
                    (sb, s) => {
                        sb.Append(s);
                        sb.Append(separator);
                        return sb;
                    },
                    sb => {
                        sb.Remove(sb.Length - separator.Length, separator.Length);
                        return sb.ToString();
                    }
                );

                throw new InvalidFieldsException(msg);
            }
        }

        public static bool TryValid(T target, ValidatorConfiguration<T> config) {
            return config.validations
                .Where(p => !p.Value.Invoke(target))
                .Select(p => p.Key)
                .Distinct()
                .Count() == 0;
        }
    }
}
