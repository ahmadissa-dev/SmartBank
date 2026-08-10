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
        private string _selectedProfilePhotoPath = string.Empty;

        private const int MaxProfilePhotoSizeInBytes  = 1024 * 1024 * 5; // 5MB
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

            bool areCountriesLoaded = await TryLoadCountriesAsync();

            SetCountrySelectionAvailability(areCountriesLoaded);
        }

        private async Task<bool> TryLoadCountriesAsync()
        {
            try
            {
                List<Country> countries = await _countryService.GetAllCountriesAsync();

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

            if (!IsImageValid(fileName))
                return;

            _selectedProfilePhotoPath = fileName;
            pbPersonImage.Load(_selectedProfilePhotoPath);
        }

        private bool IsImageValid(string imagePath)
        {

            if (!File.Exists(imagePath))
            {
                MessageBox.Show(
                    "The selected image file does not exist.",
                    "Invalid Image",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            FileInfo fileInfo = new FileInfo(imagePath);

            if (!AllowedProfilePhotoExtensions.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "Only PNG, JPG, and JPEG images are allowed.",
                    "Invalid Image",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            if (fileInfo.Length > MaxProfilePhotoSizeInBytes )
            {
                MessageBox.Show(
                    "Image size must not exceed 5 MB.",
                    "Invalid Image",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            try
            {
                using (Image image = Image.FromFile(imagePath))
                {
                    if (image.Height < MinProfilePhotoHeight || image.Width < MinProfilePhotoWidth)
                    {
                        MessageBox.Show(
                            "Image dimensions must be at least 200 x 200 pixels.",
                            "Invalid Image",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                EventViewerLogger.LogWarning(ex, "Invalid image file selected in frmAddEditPerson");

                MessageBox.Show(
                    "The selected file is not a valid image.",
                    "Invalid Image",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            return true;
        }
    }
}
