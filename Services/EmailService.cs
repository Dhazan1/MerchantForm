using MerchantForm.Model.ViewModel;
using MerchantForm.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using System.Threading.Tasks;
using System.Net.Security;
using RestSharp;
using AutoMapper;

namespace MerchantForm.Services
{
    public class EmailService : IEmailService
       
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public async Task<bool> SendEmail(SendMailModel data)
        {
            string EmailApiBaseUrl = _config.GetSection("ExternalUrl").GetSection("EmailApiBaseUrl").Value;
            string SendEmail = _config.GetSection("ExternalUrl").GetSection("SendEmail").Value;
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            RestClient restClient = new RestClient(EmailApiBaseUrl); // TODO
            restClient.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
     
            RestRequest request = new RestRequest(SendEmail, Method.POST);
            request.AddHeader("Accept", "application/json");
            string jsonObject = JsonConvert.SerializeObject(data, Formatting.Indented, jsonSerializerSettings);
            request.AddParameter("application/json", jsonObject, ParameterType.RequestBody);
            TaskCompletionSource<IRestResponse> taskCompletion = new TaskCompletionSource<IRestResponse>();
            RestRequestAsyncHandle handle = restClient.ExecuteAsync(request, r => taskCompletion.SetResult(r));
            RestResponse response = (RestResponse)(await taskCompletion.Task);
            if (response.IsSuccessful)
            {
                //var result = JsonConvert.DeserializeObject<string>(response.Content, new ExpandoObjectConverter());
                //var r = Mapper.Map<string>(result);
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
