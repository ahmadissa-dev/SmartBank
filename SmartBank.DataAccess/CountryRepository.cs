using SmartBank.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SmartBank.DataAccess
{
    public class CountryRepository
    {
        public async Task<List<Country>> GetAllCountriesAsync()
        {
            List<Country> countries = new List<Country>();

            using (SqlConnection connection = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("dbo.usp_GetAllCountries", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        int countryIdOrdinal = reader.GetOrdinal("CountryID");
                        int countryNameOrdinal = reader.GetOrdinal("CountryName");

                        while (await reader.ReadAsync())
                        {
                            Country country = new Country
                            (
                               reader.GetInt32(countryIdOrdinal),
                               reader.GetString(countryNameOrdinal)
                            );

                            countries.Add(country);
                        }
                    }
                }
            }

            return countries;
        }
    }
}
