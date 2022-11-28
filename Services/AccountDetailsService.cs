using AutoMapper;
using Mapster;
using MerchantForm.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MerchantForm.Services
{
    public class AccountDetailsService : IAccountDetailsService
    {
        IConfiguration _config;
        IMapper _mapper;
        public AccountDetailsService(IConfiguration config, IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
        }

        public async Task<object> GetAcctDetails(string accNum)
        {
            try
            {
                if (accNum.Length != 10) { return "Invalid"; }
                var baseAddress = _config.GetValue<string>("AcctDetailsEndpoint");
                var path = _config.GetValue<string>("AcctDetailspath");
                var client = new HttpClient()
                {
                    BaseAddress = new Uri(baseAddress)
                };
                var appID = "ALAT";
                var url = baseAddress + path + $"acctnum={accNum}&appID={appID}";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var Jsondetails = await response.Content.ReadAsStringAsync();
                    var details = JsonConvert.DeserializeObject<AcctResponse>(Jsondetails);
                    // var data = _mapper.Map<AcctResponseRequired>(details);
                    var data = details.Adapt<AcctResponseRequired>();
                    data.statusCode = response.StatusCode.ToString();
                    return data;
                }
                else return "BadRequest";
            }
            catch (Exception ex) { return ex.Message; }
        }

    }
}
