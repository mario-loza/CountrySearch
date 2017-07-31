using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;

namespace CountrySearch
{
    class Program
    {
        #region CONSTANTS
        const string azureSearchServiceName = "mini-search";
        const string azureSearchServiceAdminApiKey = "8DAE9BD6BAF2D9104D35D77CD09D9121";
        const string azureSearchServiceQueryApiKey = "C14627B2D007D6B8C67A85BFEBDD2144";
        #endregion

        #region Azure Service
        private static SearchServiceClient CreateSearchServiceClient()
        {
            SearchServiceClient serviceClient = new SearchServiceClient(
                                                    azureSearchServiceName, 
                                                    new SearchCredentials(azureSearchServiceAdminApiKey));
            return serviceClient;
        }
        private static SearchIndexClient CreateSearchIndexClient()
        {
            SearchIndexClient indexClient = new SearchIndexClient(
                                                azureSearchServiceName,
                                                "countries", 
                                                new SearchCredentials(azureSearchServiceQueryApiKey));
            return indexClient;
        }
        #endregion

        #region Sample Queries

        public static void PushNewDocument()
        {
            ISearchIndexClient indexClient = CreateSearchServiceClient().Indexes.GetClient("countries");
            
            var countries = new Country[] {
            new Country{ id= "1", name="Australia",independence= new DateTimeOffset(1901,1,1,0,0,0,new TimeSpan() ) 
                         ,population=24130000,continent="Oceania"},
            new Country{ id= "2", name="Canada",independence= new DateTimeOffset(1867,7,1,0,0,0,new TimeSpan() ) 
                         ,population=36290000,continent="America"},
            new Country{ id= "3", name="Mexico",independence= new DateTimeOffset(1810,9,16,0,0,0,new TimeSpan() ) 
                         ,population=127500000,continent="America"},
            };
            var batch = IndexBatch.Upload(countries);
            try
            {
                indexClient.Documents.Index(batch);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to index the document: {0}", e.Message);
            }
        }

        public static void Samplequeries()
        {
            ISearchIndexClient indexClient = CreateSearchIndexClient();
            var parameters = new SearchParameters();
            var results = new DocumentSearchResult<Country>();
            
            //All Documents indexed
            results = indexClient.Documents.Search<Country>("", parameters);
            PrintResults(results);

            //Search for a word on all searchable fields
            results = indexClient.Documents.Search<Country>("America", parameters);
            PrintResults(results);

            //countries in Oceania
            parameters = new SearchParameters();
            parameters.Filter = "continent eq 'Oceania'";
            results = indexClient.Documents.Search<Country>("", parameters);
            PrintResults(results);

            //countries with less than 100000000 persons
            parameters = new SearchParameters();
            parameters.Filter = "population lt 100000000";
            results = indexClient.Documents.Search<Country>("", parameters);
            PrintResults(results);

        }

        public static void PrintResults(DocumentSearchResult<Country> searchResults)
        {
            Console.WriteLine("");
            foreach (SearchResult<Country> result in searchResults.Results)
            {
                Console.WriteLine("{0} {1} {2} {3}", 
                    result.Document.id,
                    result.Document.name,
                    result.Document.population,
                    result.Document.continent);
            }
        }


        #endregion

        static void Main(string[] args)
        {
            var done = false;
            while (!done)
            {
                Console.WriteLine("Please choose an option:");
                Console.WriteLine("0-> Load Data, 1-> Run Queries, 2->Exit");
                var op = Console.ReadLine();
                switch (op)
                {
                    case "0":
                        PushNewDocument();
                        break;
                    case "1":
                        Samplequeries();
                        break;
                    case "2":
                        done = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
