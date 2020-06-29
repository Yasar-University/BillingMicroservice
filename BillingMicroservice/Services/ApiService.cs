using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using BillingMicroservice.Services;
using BillingMicroservice;

namespace GCPCalc.Services
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory httpFact;
        private string responses_ofresponse;
        private Stack<KeyValuePair<string, string>> stack;
                

        public ApiService(IHttpClientFactory factory)   
        {
            httpFact = factory;
        }

        public async Task<string> GetBillingComputeData()
        {
            const string JSON_SERVICE_KEY_FILE = "./responsivebilling.json";
            const string BASE_URL = "";//This address will be given from the first microservice when run without any error.

            var accessToken = GetAccessToken(JSON_SERVICE_KEY_FILE, BASE_URL); 

            var client = httpFact.CreateClient();
            client.BaseAddress = new Uri(BASE_URL);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("BillingDoc", accessToken);

            string singlePageResponse = string.Empty;                          
            string nextPageToken = string.Empty;
            responses_ofresponse = string.Empty;
            stack = new Stack<KeyValuePair<string, string>>();
            do
            {                                                                   
               
                var response = await client.GetAsync(BASE_URL+"api/billing");              

                singlePageResponse = await response.Content.ReadAsStringAsync();
                nextPageToken = GetNextPageToken(singlePageResponse);
            } while (nextPageToken != string.Empty);                          
            return responses_ofresponse;
        }
       
       
  
        void EvaluationBlockOfBillingData(JsonTextReader reader)
        {
            string key = reader.Value.ToString();
            switch (key)
            {
                case "Id":
                    PushNextToken(key, reader);
                    break;
                case "Name":
                    PushNextToken(key, reader);
                    break;
                case "Description":
                    PushNextToken(key, reader);
                    break;
                case "StockNumber":
                    PushNextToken(key, reader);
                    reader.Read();
                    if (reader.Value.ToString().CompareTo("Compute") == 0)
                    {
                        reader.Read(); 
                        reader.Read();
                        if (0 == reader.Value.ToString().CompareTo("RAM") ||
                            0 == reader.Value.ToString().CompareTo("CPU"))
                        {
                            PushCurrentToken("StockNumber", reader.Value.ToString());
                            reader.Read(); 
                            reader.Read(); 
                            reader.Read();
                            reader.Read(); 
                            reader.Read();
                            IntoPricingInfo(reader);
                            FinishRead(reader);
                        }
                        else
                        {
                            FinishRead(reader);
                            stack.Clear();
                        }
                    }
                    else
                    {
                        FinishRead(reader);
                        stack.Clear();
                    }
                    break;
                default:
                    break;
            }
        }
        void PushNextToken(string name, JsonTextReader reader)
        {
            reader.Read();
            stack.Push(new KeyValuePair<string, string>(name, reader.Value.ToString()));
        }
        void PushCurrentToken(string name, string value)
        {
            stack.Push(new KeyValuePair<string, string>(name, value));
        }
        public string GetNextPageToken(string stringResponse)
        {
            JObject o = JObject.Parse(stringResponse);
            return (string)o.SelectToken("nextPageToken");
        }

        public string GetAccessToken(string jsonKeyFilePath, params string[] scopes)
        {
            using (var stream = new FileStream(jsonKeyFilePath, FileMode.Open, FileAccess.Read))
            {
                return GoogleCredential
                    .FromStream(stream)
                    .CreateScoped(scopes)
                    .UnderlyingCredential
                    .GetAccessTokenForRequestAsync().Result;
            }
        }
        private void FinishRead(JsonTextReader reader)
        {
            string token = string.Empty;
            do 
            {
                reader.Read();
                if (reader.Value != null)
                {
                    token = reader.Value.ToString();
                }
                else
                {
                    continue;
                }
            } while (0 != token.CompareTo("serviceProviderName"));
            reader.Read(); 
            reader.Read(); 
        }

        void IntoPricingInfo(JsonTextReader reader)
        {

            string token = string.Empty;
            do
            {
                reader.Read();
                if (reader.Value != null)
                {
                    token = reader.Value.ToString();
                }
                else
                {
                    continue;
                }
            } while (0 != token.CompareTo("nanos"));
            reader.Read();
            
            float nanos = Convert.ToSingle(reader.Value.ToString());
            float stockval = nanos / 1000000000.0F;
            string stocknum = stockval.ToString("0.000000");  
            PushCurrentToken("StockNumber", stocknum);
        }

  
    }
}