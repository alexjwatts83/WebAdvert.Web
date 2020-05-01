using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WebAdvert.Web.Models;

namespace WebAdvert.Web.ServiceClients
{
    public class SearchApiClient : ISearchApiClient
    {
        private readonly HttpClient _client;
        private readonly string BaseAddress = string.Empty;
        public SearchApiClient(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            BaseAddress = configuration.GetSection("SearchApi").GetValue<string>("url");
        }

        public async Task<List<AdvertType>> Search(string keyword)
        {
            var result = new List<AdvertType>();
            var callUrl = $"{BaseAddress}/search/v1/{keyword}";
            var response = await _client.GetAsync(new Uri(callUrl)).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //var allAdverts = await httpResponse.Content.ReadAsAsync<List<AdvertType>>().ConfigureAwait(false);
                var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var allAdverts = JsonConvert.DeserializeObject<List<AdvertType>>(responseJson);
                result.AddRange(allAdverts);
            }

            return result;
        }
    }
}
