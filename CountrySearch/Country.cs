using System;

namespace CountrySearch
{
    public class Country
    {
        public string id { get; set; }
        public string name { get; set; }
        public DateTimeOffset  independence { get; set; }
        public long population { get; set; }
        public string continent { get; set; }
    }
}
