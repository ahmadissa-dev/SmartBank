using System;

namespace SmartBank.WinForms.Controls
{
    internal class InputValidationPolicy
    {
        public Func<string, char, bool> CanAcceptCharacter { get; }
        public Predicate<string> IsTextValid { get; }

        public InputValidationPolicy(Func<string, char, bool> canAcceptCharacter, Predicate<string> isTextValid)
        {
            CanAcceptCharacter = canAcceptCharacter;
            IsTextValid = isTextValid;
        }
    }
}
