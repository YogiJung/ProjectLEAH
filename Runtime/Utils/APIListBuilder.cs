using System.Collections.Generic;
using System.Linq;
using ProjectLeah.Runtime.TypeReference;

namespace ProjectLeah.Runtime.Utils
{
    public class APIListBuilder
    {
        public class Builder
        {
            private List<API> APIList = new List<API>();

            public Builder Build(string API_Name, string API_KEY, string Protocol, int order)
            {
                API api = new API
                {
                    API_Name = API_Name,
                    API_KEY = API_KEY,
                    Protocol = Protocol,
                    order = order
                };

                APIList.Add(api);

                return this;
            }
            
            public List<API> GetAPIList()
            {
                return APIList;
            }
        }
    }
}