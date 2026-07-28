using SmartBank.DataAccess;
using SmartBank.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartBank.Business
{
    public class CountryService
    {
        private readonly CountryRepository _countryRepository = new CountryRepository();

        public async Task<List<Country>> GetAllCountriesAsync()
        {
            List<Country> countries = await _countryRepository.GetAllCountriesAsync();

            if(countries.Count == 0)
            {
                throw new InvalidOperationException("No active countries were found");
            }

            return countries;
        }
    }
}
