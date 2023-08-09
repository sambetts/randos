using Models.Entities;
using Models.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace Models
{
    public class ApiClient
    {
        #region Constructors

        private HttpClient _client = null;
        private readonly string _bearer;
        private readonly string _apiBaseUrl;

        public ApiClient(string apiBaseUrl) : this(string.Empty, apiBaseUrl)
        {
        }
        public ApiClient(string bearer, string apiBaseUrl)
        {
            this._bearer = bearer;
            this._apiBaseUrl = apiBaseUrl;

            _client = new HttpClient();
            if (!string.IsNullOrEmpty(bearer))
            {
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearer);
            }
        }
        public bool IsAuthenticated => !string.IsNullOrEmpty(_bearer);

        #endregion


        public async Task<HomePageModel> GetHomePageModel()
        {
            var response = await _client.GetAsync($"{_apiBaseUrl}/Debates");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<HomePageModel>(content);

            return responseObject;
        }

        public async Task<DebateUser> EnsureUser()
        {
            var response = await _client.PostAsync($"{_apiBaseUrl}/Users/EnsureUser", null);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DebateUser>(content);
        }

        public async Task<List<ResponseType>> GetResponseTypes()
        {
            var response = await _client.PostAsync($"{_apiBaseUrl}/Debates/GetResponseTypes", null);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ResponseType>>(content);
        }
        public async Task<NoReturnDataAPIResponse> Reply(NewAnswerDTO newAnswer)
        {
            return await Post($"{_apiBaseUrl}/api/AnswersAPI/PostAnswer", newAnswer);
        }
        public async Task<NoReturnDataAPIResponse> Delete(AnswerDataOnlyTreeNode answer)
        {
            return await Delete($"{_apiBaseUrl}/api/AnswersAPI/{answer.ID}");
        }
        public async Task<DebateDetailsPage> GetDebateFromWebSafeName(string webSafeName)
        {
            var response = await _client.GetAsync($"{_apiBaseUrl}/Debates/DetailsByWebsafeName/{webSafeName}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<DebateDetailsPage>(content);

            return responseObject;
        }


        async Task<NoReturnDataAPIResponse> Post(string url, object body)
        {
            var m = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(url, m);

            var content = await response.Content.ReadAsStringAsync();
            return new NoReturnDataAPIResponse { IsSuccessStatusCode = response.IsSuccessStatusCode, ResponseContents = content, ResponseCode = response.StatusCode };
        }
        async Task<NoReturnDataAPIResponse> Delete(string url)
        {
            var response = await _client.DeleteAsync(url);

            var content = await response.Content.ReadAsStringAsync();
            return new NoReturnDataAPIResponse { IsSuccessStatusCode = response.IsSuccessStatusCode, ResponseContents = content, ResponseCode = response.StatusCode };
        }
    }


    #region Return Classes

    public class NoReturnDataAPIResponse
    {
        public HttpStatusCode ResponseCode { get; set; }
        public string ResponseContents { get; set; }
        public bool IsSuccessStatusCode { get; set; }

        public string ToSummary()
        {
            if (string.IsNullOrEmpty(ResponseContents))
            {
                return $"HTTP{(int)ResponseCode} (response body has no content)";
            }
            else
            {
                return $"HTTP{(int)ResponseCode} - {ResponseContents}";
            }
        }
    }

    /// <summary>
    /// A list of an object type of API response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectListAPIResponse<T> : NoReturnDataAPIResponse
    {
        public List<T> ResponseList { get; set; }
    }
    public class SingleObjectAPIResponse<T> : NoReturnDataAPIResponse
    {
        public T ResponseData { get; set; }
    }
    #endregion
}
