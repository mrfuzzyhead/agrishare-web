using Agrishare.Core.Entities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrishare.Core.Utils
{
    public class Google
    {
        public static string Key => Config.Find(Key: "Google Maps API Key")?.Value ?? string.Empty;

        public static Address ReverseGeoCode(decimal Latitude, decimal Longitude)
        {
            var address = new Address();

            var client = new RestClient($"https://maps.googleapis.com/maps/api/geocode/json?key={Key}&latlng={Latitude},{Longitude}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            var response = client.Execute<ReverseGeoCodeResult>(request);

            if (response.Data != null && response.Data.results != null)
            {
                foreach (var result in response.Data.results)
                {
                    if (result.address_components != null)
                    {
                        foreach (var component in result.address_components)
                        {
                            if (component.types.Contains("sublocality"))
                                address.Sublocality = component.long_name;
                            if (component.types.Contains("locality"))
                                address.Locality = component.long_name;
                            if (component.types.Contains("colloquial_area"))
                                address.ColloquialArea = component.long_name;
                            if (component.types.Contains("administrative_area_level_1"))
                                address.AdministrativeAreaLevel1 = component.long_name;
                            if (component.types.Contains("country"))
                                address.Country = component.long_name;
                        }
                    }
                }
            }

            return address;            
        }

        public class Address
        {
            public string Sublocality { get; set; }
            public string Locality { get; set; }
            public string ColloquialArea { get; set; }
            public string AdministrativeAreaLevel1 { get; set; }
            public string Country { get; set; }
        }

        public class ReverseGeoCodeResult
        {
            public PlusCode plus_code { get; set; }
            public List<Result> results { get; set; }
            public string status { get; set; }
                        
            public class PlusCode
            {
                public string compound_code { get; set; }
                public string global_code { get; set; }
            }

            public class AddressComponent
            {
                public string long_name { get; set; }
                public string short_name { get; set; }
                public List<string> types { get; set; }
            }

            public class Location
            {
                public double lat { get; set; }
                public double lng { get; set; }
            }

            public class Northeast
            {
                public double lat { get; set; }
                public double lng { get; set; }
            }

            public class Southwest
            {
                public double lat { get; set; }
                public double lng { get; set; }
            }

            public class Viewport
            {
                public Northeast northeast { get; set; }
                public Southwest southwest { get; set; }
            }

            public class Bounds
            {
                public Northeast northeast { get; set; }
                public Southwest southwest { get; set; }
            }

            public class Geometry
            {
                public Location location { get; set; }
                public string location_type { get; set; }
                public Viewport viewport { get; set; }
                public Bounds bounds { get; set; }
            }

            public class Result
            {
                public List<AddressComponent> address_components { get; set; }
                public string formatted_address { get; set; }
                public Geometry geometry { get; set; }
                public string place_id { get; set; }
                public PlusCode plus_code { get; set; }
                public List<string> types { get; set; }
                public List<string> postcode_localities { get; set; }
            }

        }


    }
}
