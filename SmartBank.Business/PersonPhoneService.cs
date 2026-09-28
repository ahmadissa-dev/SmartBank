using SmartBank.DataAccess;
using SmartBank.Infrastructure.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartBank.Business
{
    public enum PersonPhoneValidationError
    {
        None,
        Required,
        PhoneCountryRequired,
        InvalidFormat,
        AlreadyExists,
        ValidationFailed
    }
    public class PersonPhoneService
    {
        private readonly PersonPhoneRepository _personPhoneRepository = new PersonPhoneRepository();

        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber, int phoneCountryID)
        {
            return await _personPhoneRepository.
                         PhoneNumberExistsAsync(phoneNumber, phoneCountryID);
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

        public async Task<PersonPhoneValidationError> ValidatePhoneNumberAsync(string phoneNumber, int? phoneCountryID)
        {

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return PersonPhoneValidationError.Required;
            }

            if (!phoneCountryID.HasValue)
            {
                return PersonPhoneValidationError.PhoneCountryRequired;
            }

            if (!IsPhoneNumberValid(phoneNumber))
            {
                return PersonPhoneValidationError.InvalidFormat;
            }

            try
            {
                bool exists = await PhoneNumberExistsAsync(phoneNumber, phoneCountryID.Value);

                if (exists)
                {
                    return PersonPhoneValidationError.AlreadyExists;
                }
            }
            catch (Exception ex)
            {
                EventViewerLogger.LogError(
                ex,
                "Failed to check phone number uniqueness during phone number validation."
                );

                return PersonPhoneValidationError.ValidationFailed;
            }

            return PersonPhoneValidationError.None;
        }
    }
}
