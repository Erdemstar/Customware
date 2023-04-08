using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Customware
{
    internal class NetworkIO
    {
        public string GetHttpResponse(string url)
        {
            var client = new HttpClient();

            try
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
               
                var responseBody = response.Content.ReadAsStringAsync().Result;
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                //Console.WriteLine($"HTTP hata oluştu. Mesaj: {e.Message}");
                return null;
            }
        }
    }
}
