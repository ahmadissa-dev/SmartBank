using SmartBank.DataAccess;
using System.Linq;
using System.Threading.Tasks;

namespace SmartBank.Business
{
    public class PersonPhoneService
    {
        private readonly PersonPhoneRepository _personPhoneRepository = new PersonPhoneRepository();

        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber, int countryID)
        {
            return await _personPhoneRepository.
                         PhoneNumberExistsAsync(phoneNumber, countryID);
        }

        public bool IsPhoneNumberValid(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            if (!phoneNumber.All(c => c >= '0' && c <= '9'))
                return false;

            if (phoneNumber.Length < 5 || phoneNumber.Length > 15)
                return false;

            return true;
        }
    }
}
