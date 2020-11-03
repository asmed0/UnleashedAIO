using TinyCsvParser.Mapping;

namespace UnleashedAIO.JSON
{
    public class Tasks
    {

        public string Store { get; set; }
        public string SKU { get; set; }
        public string Size { get; set; }
        public string Mode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string email { get; set; }
        public string PhoneNumber { get; set; }
        public string Adress { get; set; }
        public string Adress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string CVV { get; set; }
    }

    public class CsvTasksMapping : CsvMapping<Tasks>
    {
        public CsvTasksMapping()
            : base()
        {

            // TaskInfo Mapping
            MapProperty(0, x => x.Store);
            MapProperty(1, x => x.Mode);
            MapProperty(2, x => x.SKU);
            MapProperty(3, x => x.Size);

            // ContactInfo Mapping
            MapProperty(4, x => x.FirstName);
            MapProperty(5, x => x.LastName);
            MapProperty(6, x => x.email);
            MapProperty(7, x => x.PhoneNumber);

            // Shipping Mapping
            MapProperty(8, x => x.Adress);
            MapProperty(9, x => x.Adress2);
            MapProperty(10, x => x.City);
            MapProperty(11, x => x.State);
            MapProperty(12, x => x.ZipCode);
            MapProperty(13, x => x.Country);

            // CardInfo Mapping
            MapProperty(14, x => x.CardNumber);
            MapProperty(15, x => x.ExpiryMonth);
            MapProperty(16, x => x.ExpiryYear);
            MapProperty(17, x => x.CVV);



        }
    }

}
