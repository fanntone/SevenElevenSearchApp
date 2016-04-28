using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace SevenSearch
{
    public class WebApiServices
    {
        private readonly HttpClient _httpClient;
        public WebApiServices()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetDataAsync(string city, string area)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            var apiUri = string.Format("http://emap.pcsc.com.tw/EMapSDK.aspx?commandid=SearchStore&city={0}&town={1}&roadname=&ID=&StoreName=&SpecialStore_Kind=&isDining=False&isParking=False&isLavatory=False&isATM=False&is7WiFi=False&isIce=False&isHotDog=False&isHealthStations=False&isIceCream=False&isOpenStore=False&isFruit=False&isCityCafe=False&isUp=False&isOrganic=False&isCorn=False&isMakeUp=False&isMuji=False&isMD=False&isStarBucks=False&isIbon=False&address=",
                                        city,
                                        area);
            var response = await _httpClient.GetAsync(apiUri);
            var responseAsString = await response.Content.ReadAsStringAsync();
            if(string.IsNullOrWhiteSpace(responseAsString) == false)
            {
                responseAsString = responseAsString.Substring(0, responseAsString.Length);
            }
            return responseAsString;
        }
    }
}