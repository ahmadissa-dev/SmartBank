using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SmartBank.WinForms
{
    public enum InputValidationMode
    {
        Any,
        DigitsOnly,
        DecimalNumber,
        LettersOnly,
        LettersAndSpaces,
    }

    public class SmartTextBox : TextBox
    {

        private readonly Dictionary<InputValidationMode, Func<char, bool>> _validationRules;

        private const char EmptyChar = '\0';

        private string _hintText = "Enter anything";
        private bool _isHintVisible = false;
        private char _realPasswordChar = EmptyChar;

        private readonly Color _textColor = SystemColors.WindowText;
        private readonly Color _hintColor = SystemColors.GrayText;

        [Category("Smart TextBox")]
        [Description("Text shown when the TextBox is empty.")]
        [DefaultValue("Enter anything")]
        public string HintText
        {
            get
            {
                return _hintText;
            }
            set
            {
                _hintText = value?.Trim() ?? string.Empty;

                if (IsHintVisible)
                {
                    SetActualText();
                }

                TryShowHint();
            }
        }

        /// <summary>
        /// Gets or sets the actual user text.
        /// When the hint is visible, this property returns an empty string
        /// </summary>
        public override string Text
        {
            get
            {
                return _isHintVisible ? string.Empty : base.Text;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    SetActualText();
                    TryShowHint();
                    return;
                }

                SetActualText(value);
            }
        }

        [Category("Smart TextBox")]
        [Description("Password character used for the actual user text. The hint text is never masked.")]
        [DefaultValue('\0')]
        public char SmartPasswordChar
        {
            get
            {
                return _realPasswordChar;
            }
            set
            {
                _realPasswordChar = value;

                ApplyPasswordMasking();
            }
        }

        // Hide the inherited PasswordChar to avoid bypassing SmartPasswordChar logic
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new char PasswordChar
        {
            get
            {
                return SmartPasswordChar;
            }
            set
            {
                SmartPasswordChar = value;
            }
        }


        [Browsable(false)]
        public bool IsHintVisible
        {
            get
            {
                return _isHintVisible;
            }
        }

        [Category("Smart TextBox")]
        [Description("Determines what kind of input is allowed")]
        [DefaultValue(InputValidationMode.Any)]
        public InputValidationMode ValidationMode { get; set; } = InputValidationMode.Any;

        public SmartTextBox()
        {
            BorderStyle = BorderStyle.Fixed3D;

            _validationRules = new Dictionary<InputValidationMode, Func<char, bool>>
            {
                { InputValidationMode.Any, keyChar => true },
                { InputValidationMode.DigitsOnly, char.IsDigit },
                { InputValidationMode.DecimalNumber,keyChar => char.IsDigit(keyChar) || CanAddDecimalPoint(keyChar) },
                { InputValidationMode.LettersOnly,keyChar => char.IsLetter(keyChar)},
                { InputValidationMode.LettersAndSpaces,keyChar => char.IsLetter(keyChar) || keyChar == ' '}
            };
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            TryShowHint();
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            HideHint();
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            TryShowHint();
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!IsKeyAllowed(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            base.OnKeyPress(e);
        }

        private void TryShowHint()
        {
            if (!string.IsNullOrWhiteSpace(base.Text))
                return;

            if (string.IsNullOrWhiteSpace(_hintText))
                return;

            _isHintVisible = true;

            base.Text = _hintText;
            base.ForeColor = _hintColor;
            ApplyPasswordMasking();
        }

        private void HideHint()
        {
            if (!_isHintVisible)
                return;

            SetActualText();
        }

        private void SetActualText(string value = "")
        {
            _isHintVisible = false;

            base.Text = value ?? string.Empty;
            base.ForeColor = _textColor;
            ApplyPasswordMasking();
        }

        private void ApplyPasswordMasking()
        {
            base.PasswordChar = _isHintVisible ? EmptyChar : _realPasswordChar;
        }

        private bool IsKeyAllowed(char keyChar)
        {
            if (char.IsControl(keyChar)) return true;

            if (_validationRules.TryGetValue(this.ValidationMode, out Func<char, bool> rule))
                return rule(keyChar);
            
            return true;
        }

        // Allows only one decimal point and prevents starting the value with a dot
        private bool CanAddDecimalPoint(char keyChar)
        {
            return keyChar == '.' && !Text.Contains(".") && Text.Length > 0;
        }
    }
}