using SmartBank.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SmartBank.DataAccess
{
    public class CountryRepository
    {
        private const string GetActiveCountriesCommand = "dbo.usp_GetActiveCountries";

        private Country ReadCountry(SqlDataReader reader)
        {
            int countryIdOrdinal = reader.GetOrdinal("CountryID");
            int countryNameOrdinal = reader.GetOrdinal("CountryName");
            int countryCallingCodeOrdinal = reader.GetOrdinal("CallingCode");
            int countryIsActiveOrdinal = reader.GetOrdinal("IsActive");

            return new Country
            (
               reader.GetInt32(countryIdOrdinal),
               reader.GetString(countryNameOrdinal),
               reader.GetString(countryCallingCodeOrdinal),
               reader.GetBoolean(countryIsActiveOrdinal)
            );
        }
        public async Task<List<Country>> GetActiveCountriesAsync()
        {
            List<Country> countries = new List<Country>();

            using (SqlConnection connection = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(GetActiveCountriesCommand, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Country country = ReadCountry(reader);

                            countries.Add(country);
                        }
                    }
                }
            }

            return countries;
        }
    }
}
