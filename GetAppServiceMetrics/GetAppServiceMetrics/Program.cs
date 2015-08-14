using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Web;
using System.Configuration;
using System.Net.Http;

namespace GetAppServiceMetrics
{
    class Program
    {

        private const string apiVersion = "2015-04-01";
        private const string serviceUrlFormat = "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Web/sites/{2}/metrics?api-version={3}";
        private static string subscriptionId = ConfigurationManager.AppSettings["subscriptionId"] ?? "";
        private static string resourceGroup = ConfigurationManager.AppSettings["resourceGroup"] ?? "";
        private static string siteName = ConfigurationManager.AppSettings["siteName"] ?? "";
        private static string tenantId = ConfigurationManager.AppSettings["tenantId"] ?? "";
        private static string applicationId = ConfigurationManager.AppSettings["applicationId"] ?? "";
        private static string redirectUri = ConfigurationManager.AppSettings["redirectUri"] ?? "";

        static void Main(string[] args)
        {
            string serviceUrl = string.Format(serviceUrlFormat,
                        subscriptionId,resourceGroup,siteName,apiVersion);

            var dateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
            List<string> metricNames = new List<string>();
            metricNames.Add("CpuTime");
            metricNames.Add("MemoryWorkingSet");
            metricNames.Add("AverageResponseTime");
            string timeGrain = "PT1H";  // every min - ”PT1M”、every 5min - ”PT5M”, every hour - "PT1H” 
            DateTime startTime = new DateTime(2015, 8, 1);
            DateTime endTime = new DateTime(2015, 8, 30);

            string filter = "";
            if (metricNames != null && metricNames.Count > 0)
            {
                if (metricNames.Count == 1)
                {
                    filter = "name.value eq '" + metricNames[0] + "' and ";
                }
                else
                {
                    filter = "(name.value eq '"
                             + String.Join("' or name.value eq '", metricNames) + "') and ";
                }
            }
        
            filter += String.Format("startTime eq {0} and endTime eq {1} and timeGrain eq duration'{2}'",
                startTime.ToString(dateTimeFormat), endTime.ToString(dateTimeFormat), timeGrain);
            serviceUrl += "&details=true&$filter="
                        + HttpUtility.UrlEncode(filter).Replace("+", "%20");
            Console.WriteLine("url:" + serviceUrl);

            try {

                string token = GetAToken(tenantId, applicationId, redirectUri);
                Console.WriteLine("authtoken:" + token);

                string response = ExecuteGetRequest(serviceUrl, token);
                Console.WriteLine("output:" + response);

                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Unhandled exception caught:");

                while (e != null)
                {
                    Console.WriteLine("\t{0}", e.Message);
                    e = e.InnerException;
                }
            }
        }

        public static string GetAToken(string tenantId, string applicationId, string redirectUri)
        {
            var authenticationContext =
                    new AuthenticationContext("https://login.windows.net/" + tenantId);
            var result = authenticationContext.AcquireToken("https://management.azure.com/",
                                    applicationId, new Uri(redirectUri));

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }
            return result.AccessToken;
        }

        static string ExecuteGetRequest(string requestUri, string token )
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage result = client.GetAsync(requestUri).Result; 

                return result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
