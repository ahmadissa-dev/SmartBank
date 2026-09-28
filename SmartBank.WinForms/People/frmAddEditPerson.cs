using SmartBank.Business;
using SmartBank.Infrastructure.Logging;
using SmartBank.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartBank.WinForms
{
    public partial class frmAddEditPerson : Form
    {
        private readonly CountryService _countryService = new CountryService();
        private readonly PersonPhoneService _personPhoneService = new PersonPhoneService();
        private string _selectedProfilePhotoPath = string.Empty;
        private Country _selectedCountry;
        private int _phoneValidationVersion;

        private const int MaxProfilePhotoSizeInBytes = 1024 * 1024 * 5; // 5MB
        private const int MinProfilePhotoWidth = 200;
        private const int MinProfilePhotoHeight = 200;
        private static readonly string[] AllowedProfilePhotoExtensions = { ".png", ".jpg", ".jpeg" };

        public frmAddEditPerson()
        {
            InitializeComponent();
        }

        private async void frmAddEditPerson_Load(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            ConfigureDateOfBirthPicker();

            bool areCountriesLoaded = await TryLoadCountriesAsync();

            SetCountrySelectionAvailability(areCountriesLoaded);
        }

        private void ConfigureDateOfBirthPicker()
        {
            dtpDateOfBirth.MaxDate = DateTime.Today.AddYears(-18);
            dtpDateOfBirth.MinDate = DateTime.Today.AddYears(-120);

            dtpDateOfBirth.Value = dtpDateOfBirth.MaxDate;
        }

        private async Task<bool> TryLoadCountriesAsync()
        {
            try
            {
                List<Country> countries = await _countryService.GetActiveCountriesAsync();

                cbCountries.DataSource = countries;
                cbCountries.DisplayMember = nameof(Country.CountryName);
                cbCountries.ValueMember = nameof(Country.CountryID);
                cbCountries.SelectedIndex = -1;

                return true;
            }
            catch (InvalidOperationException ex)
            {
                EventViewerLogger.LogWarning(ex, "No active countries were found while loading frmAddEditPerson");

                MessageBox.Show(
                    "No active countries were found. You cannot add a person until countries are available",
                    "Missing Data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception ex)
            {
                EventViewerLogger.LogError(ex, "Failed to load countries in frmAddEditPerson_Load");

                MessageBox.Show(
                    "Failed to load countries",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            return false;
        }

        private void SetCountrySelectionAvailability(bool isAvailable)
        {
            cbCountries.Enabled = isAvailable;

            if (!isAvailable)
            {
                cbCountries.DataSource = null;
            }
        }

        private void btnChangePhoto_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = string.Empty;

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            string fileName = openFileDialog1.FileName;

            ImageValidationError validationError = ImageValidator.Validate(fileName);

            if (validationError != ImageValidationError.None)
            {
                ShowImageValidationError(validationError);
                return;
            }

            pbPersonImage.Load(fileName);
        }

        private void ShowImageValidationError(ImageValidationError imageValidationError)
        {
            string message;

            switch (imageValidationError)
            {
                case ImageValidationError.None:
                    return;

                case ImageValidationError.FileNotFound:
                    message = "The selected image file does not exist.";
                    break;

                case ImageValidationError.InvalidExtension:
                    message = "Only PNG, JPG, and JPEG images are allowed.";
                    break;

                case ImageValidationError.FileTooLarge:
                    message = "Image size must not exceed 5 MB.";
                    break;

                case ImageValidationError.DimensionsTooSmall:
                    message = "Image dimensions must be at least 200 x 200 pixels.";
                    break;

                case ImageValidationError.InvalidImage:
                    message = "The selected file is not a valid image.";
                    break;

                default:
                    message = "An unexpected error occurred while validating the image.";
                    break;
            }
            catch (Exception ex)
            {
                EventViewerLogger.LogWarning(ex, "Invalid image file selected in frmAddEditPerson");

                MessageBox.Show(
                message,
                    "Invalid Image",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            return true;
        }

        private void cbCountries_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedCountry = cbCountries.SelectedItem as Country;

            if (_selectedCountry == null)
            {
                stbCallingCode.Clear();
                return;
            }

            stbCallingCode.Text = $"+{_selectedCountry.CallingCode}";
        }

        private async void cbCountries_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(stbPhoneNumber.Text))
                return;

            await CanUsePhoneNumberAsync();
        }

        private void stbPhoneNumber_Enter(object sender, EventArgs e)
        {
            if (_selectedCountry == null)
            {
                MessageBox.Show("Please select a country before entering a phone number",
                                "Country Required", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                stbPhoneNumber.Clear();
                cbCountries.Focus();
            }
        }

        private async void stbPhoneNumber_Leave(object sender, EventArgs e)
        {
            await CanUsePhoneNumberAsync();
        }

        private async Task<bool> CanUsePhoneNumberAsync()
        {
            int version = ++_phoneValidationVersion;

            string phoneNumber = stbPhoneNumber.Text.Trim();

            if (string.IsNullOrEmpty(phoneNumber))
            {
                errorProvider1.SetError(
                    stbPhoneNumber,
                    "Phone number is required."
                    );
                return false;
            }

            if (!(cbCountries.SelectedValue is int))
            {
                errorProvider1.SetError(
                    stbPhoneNumber,
                    "Please select a country."
                    );
                return false;
            }

            if (!_personPhoneService.IsPhoneNumberValid(phoneNumber))
            {
                errorProvider1.SetError(
                    stbPhoneNumber,
                    "Phone number is not valid."
                    );
                return false;
            }

            int countryID = (int)cbCountries.SelectedValue;

            try
            {
                bool exists = await _personPhoneService
                                .PhoneNumberExistsAsync(phoneNumber, countryID);

                if (version != _phoneValidationVersion
                    || stbPhoneNumber.Text.Trim() != phoneNumber)
                    return false;

                if (exists)
                {
                    errorProvider1.SetError(
                        stbPhoneNumber,
                        "This number is already taken; please choose another number."
                    );
                    return false;
                }
            }
            catch (Exception ex)
            {
                EventViewerLogger.LogError(
                ex,
                "Failed to check phone number uniqueness during phone number validation."
                );

                if (version != _phoneValidationVersion
                    || stbPhoneNumber.Text.Trim() != phoneNumber)
                    return false;

                errorProvider1.SetError(
                    stbPhoneNumber,
                    "Unable to check the phone number right now. Please try again."
                );

                return false;
            }

            errorProvider1.SetError(
                stbPhoneNumber,
                string.Empty
            );

            return true;
        }
    }
}
