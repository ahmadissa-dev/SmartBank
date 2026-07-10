using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartBank.WinForms.Controls
{
    internal static class InputValidationPolicyProvider
    {
        private readonly static Dictionary<InputValidationMode, InputValidationPolicy> _policies = new Dictionary<InputValidationMode, InputValidationPolicy>
        {
            {
              InputValidationMode.Any,
              new InputValidationPolicy(
                  (text, keyChar) => true,
                  text => true
              )
            },

            {
                InputValidationMode.DigitsOnly,
                new InputValidationPolicy(
                    (text, keyChar) => char.IsDigit(keyChar),
                    text => text.All(char.IsDigit)
                )
            },

            {
                InputValidationMode.DecimalNumber,
                new InputValidationPolicy(
                    (text, keyChar) => char.IsDigit(keyChar) || (keyChar == '.' && CanAddDecimalPoint(text)),
                    text => Decimal.TryParse(text, out decimal result)
                )
            },

            {
                InputValidationMode.LettersOnly,
                new InputValidationPolicy(
                    (text, keyChar) => char.IsLetter(keyChar),
                    text => text.All(char.IsLetter)
                )
            },

            {
                InputValidationMode.LettersAndSpaces,
                new InputValidationPolicy(
                    (text, keyChar) => char.IsLetter(keyChar) || keyChar == ' ',
                    text => text.All(c => char.IsLetter(c) || c == ' ')
                )
            },
        };

        public static InputValidationPolicy GetPolicy(InputValidationMode validationMode)
        {
            if (_policies.TryGetValue(validationMode, out InputValidationPolicy policy))
            {
                return policy;
            }

            return _policies[InputValidationMode.Any];
        }

        // Allows only one decimal point and prevents starting the value with a dot
        public static bool CanAddDecimalPoint(string text)
        {
            return !text.Contains('.') && !string.IsNullOrEmpty(text);
        }
    }
}
