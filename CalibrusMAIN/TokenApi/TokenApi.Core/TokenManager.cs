using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TokenApi.Core.Models;
using System.ServiceModel;





namespace TokenApi.Core
{

    public class ConsumerData
    {
        public string UserIpaddress { get; set; }
        public string BrowserAgent { get; set; }
        public string ApiAddressRequested { get; set; }

    }

    public class TokenManager
    {
        
        public ConsumerData GetUserInformation()
        {

            var consumerData = new ConsumerData();
            consumerData.UserIpaddress = HttpContext.Current.Request.UserHostAddress;
            if (consumerData.UserIpaddress == "::1")
            {
                consumerData.UserIpaddress = "localhost";
            }
            
            consumerData.BrowserAgent = HttpContext.Current.Request.UserAgent;
            consumerData.ApiAddressRequested = HttpContext.Current.Request.Url.OriginalString;
            

            return consumerData;
            
        }

        /// <summary>
        /// 1.  (get)(Select) lookup the incoming token id in token table 
        /// 2. (get)(Select) lookup the api in the apiaccess table  need to parse the url to get to  api/request/
        /// 3. (post)(Insert) write to apilog table 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool CheckToken(string token)
        {

            try
            { 
                
                bool authorized = false;
                
                using (var calibrusContext = new CalibrusContext())
                {

                    var queryTokenStore = calibrusContext.TokenStores
                        .Where(t => t.Token == token)
                        .AsEnumerable()
                        .Select(t => new TokenStore()
                        {
                            ClientName = t.ClientName,
                            Domain = t.Domain,
                            TokenId = t.TokenId
                        }).FirstOrDefault();
                       
                    
                    var consumer = new ConsumerData();
                    consumer = GetUserInformation();

                    var uri = HttpContext.Current.Request.Url;
                    var serverHost = new Uri(uri.Scheme + "://" + uri.Authority);
                    var absoluteUri = HttpContext.Current.Request.Url.AbsoluteUri;
                    string finalSearch = "";


                    if (queryTokenStore != null)
                    {







                        if (absoluteUri.Contains("api"))
                        {

                            var apiHost = new Uri(serverHost.AbsoluteUri);
                            var apiTemplate = new UriTemplate("{api}/{*params}", true);
                            var match = apiTemplate.Match(apiHost, new Uri(absoluteUri));
                            var apiMethodRoute = match.RelativePathSegments[1];
                            finalSearch = "api/" + apiMethodRoute;






                            //string prefixSearched = "api";
                            //string parsedUrl =
                            //    consumer.ApiAddressRequested.Substring(
                            //        consumer.ApiAddressRequested.IndexOf(prefixSearched, StringComparison.Ordinal) +
                            //        prefixSearched.Length);
                            //finalSearch = prefixSearched + parsedUrl;

                        }
                        else
                        {

                            //todo  change this asap 
                            var host = new Uri(serverHost.AbsoluteUri);

                            var apiTemplate = new UriTemplate("{api}/{*params}", true);

                            //var match = apiTemplate.Match(host, new Uri("http://localhost:29001/GetQAByDateTime/date/2-15-2017/time/11"));

                            var match = apiTemplate.Match(host, new Uri(absoluteUri));

                            finalSearch = match.BoundVariables["api"];
                            string parameters = match.BoundVariables["params"];
                            //api/books/localoptional

                            //Console.WriteLine(match.BoundVariables["api"]);     // GetQAByDateTime
                            //Console.WriteLine(match.BoundVariables["params"]);  // date/2-15-2017/time/11
                        }





                        //1. Validate the Domain Host Name
                        if (serverHost.AbsoluteUri == queryTokenStore.Domain ||
                            serverHost.OriginalString == queryTokenStore.Domain)
                        {

                            if (queryTokenStore.ClientName == "Calibrus")
                            {
                                authorized = true;
                            }
                            else
                            {

                                //3. Validate the api request 
                                var apiAccessQuery = (from s in calibrusContext.ApiAccesses
                                    where s.ApiAddress.Equals(finalSearch) && s.TokenId == queryTokenStore.TokenId
                                    select s).ToList();

                                authorized = apiAccessQuery.Any();

                            }

                        }
                        else
                        {
                            if (queryTokenStore.ClientName == "Calibrus")
                            {
                                authorized = true;
                            }
                        }

                    }

                    // # 4 logging here 

                    var apiLog = new ApiLog
                    {
                        Header = consumer.BrowserAgent,
                        IpAddress = consumer.UserIpaddress,
                        RequestUrl = consumer.ApiAddressRequested,
                        Token = token,
                        AccessGranted = authorized
                    };

                    calibrusContext.ApiLogs.Add(apiLog);
                    calibrusContext.SaveChanges();


                }

                

                return authorized;


            }
            catch (Exception e)
            {
                //NLOG write to database or to Mongodb 

                //Console.WriteLine(e);
                throw;
            }
        }





    }
}
