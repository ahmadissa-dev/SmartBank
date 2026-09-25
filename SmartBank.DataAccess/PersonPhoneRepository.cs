using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SmartBank.DataAccess
{
    public class PersonPhoneRepository
    {
        private const string PhoneNumberExistsCommand = "dbo.usp_PersonPhoneExists";

        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber, int countryID)
        {
            using (SqlConnection connection = new SqlConnection(DataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(PhoneNumberExistsCommand, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    command.Parameters.AddWithValue("@CountryID", countryID);

                    await connection.OpenAsync();

                    return (bool)await command.ExecuteScalarAsync();
                }
            }
        }
    }
}
