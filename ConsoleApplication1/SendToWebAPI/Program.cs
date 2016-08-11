using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SendToWebAPI.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SendToWebAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new HttpClient())
            {
                // New code:
                client.BaseAddress = new Uri("http://localhost:5000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                var newUsageReport = new NewUsageData { TimeStamp = DateTime.Now, ProcessorUsage =  40, MemoryUsage = 30 };

                var json = JsonConvert.SerializeObject(newUsageReport);

                var content = new StringContent(json);

                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

                var response = client.PostAsync("api/virtualmachines/1/usagereports", content);
                
                var result = response.Result;
            }

        }
    }
}
