namespace SmartBank.Models
{
    public class Country
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public string CallingCode { get; set; }
        public bool IsActive { get; set; }


        public Country(int countryID, string countryName, string callingCode, bool isActive)
        {
            CountryID = countryID;
            CountryName = countryName;
            CallingCode = callingCode;
            IsActive = isActive;
        }
    }
}
