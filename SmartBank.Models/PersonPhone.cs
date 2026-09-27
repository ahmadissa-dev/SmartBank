using System;

namespace SmartBank.Models
{
    public class PersonPhone
    {
        public int PersonPhoneID { get; private set; }
        public int PersonID { get; set; }
        public int CountryID { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        // Creates a new phone number before it is saved to the database.
        public PersonPhone(int personID, int countryID, string phoneNumber, bool isPrimary)
        {
            PersonID = personID;
            CountryID = countryID;
            PhoneNumber = phoneNumber;
            IsPrimary = isPrimary;
        }

        // Reconstructs an existing phone number from the database.
        public PersonPhone(int personPhoneID, int personID, int countryID,
                           string phoneNumber, bool isPrimary, DateTime createdAt,
                           DateTime? updatedAt, bool isActive)
            : this(personID, countryID, phoneNumber, isPrimary)
        {
            PersonPhoneID = personPhoneID;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsActive = isActive;
        }
    }
}