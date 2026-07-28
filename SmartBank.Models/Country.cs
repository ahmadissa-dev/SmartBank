namespace SmartBank.Models
{
    public class Country
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }

        public Country(int countryID, string countryName)
        {
            CountryID = countryID;
            CountryName = countryName;
        }
    }
}
