using SmartBank.Business;
using SmartBank.Infrastructure.Logging;
using SmartBank.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartBank.WinForms
{
    public partial class frmAddEditPerson : Form
    {
        private readonly CountryService _countryService = new CountryService();
        public frmAddEditPerson()
        {
            InitializeComponent();
        }

        private async void frmAddEditPerson_Load(object sender, EventArgs e)
        {
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
    }
}
