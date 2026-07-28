using SmartBank.Business;
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
            bool isCountryListLoaded = false;

            try
            {
                await LoadCountriesAsync();
                isCountryListLoaded = true;
            }
            catch (InvalidOperationException ioex)
            {

                MessageBox.Show(
                    "No active countries were found. You cannot add a person until countries are available.",
                    "Missing Data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to load countries",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (!isCountryListLoaded)
                    DisableSaveBecauseCountriesAreUnavailable();
            }
        }

        private void DisableSaveBecauseCountriesAreUnavailable()
        {
            cbCountries.DataSource = null;
            cbCountries.Enabled = false;
            btnSave.Enabled = false;
        }

        private async Task LoadCountriesAsync()
        {
            List<Country> countries = await _countryService.GetAllCountriesAsync();

            cbCountries.DataSource = countries;
            cbCountries.DisplayMember = nameof(Country.CountryName);
            cbCountries.ValueMember = nameof(Country.CountryID);
        }
    }
}
