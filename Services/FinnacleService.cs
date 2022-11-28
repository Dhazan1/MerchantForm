using Dapper;
using Mapster;
using MerchantForm.Model;
using MerchantForm.Models.WebMarchant;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MerchantForm.Services.Interfaces
{
    public class FinnacleService : IFinnacleService
    {
        private readonly IConfiguration _config;
        public FinnacleService(IConfiguration config)
        {
            _config = config;
        }

        public async Task <string> Encryption()
        {

            var IV = _config.GetValue<string>("ConnectionStrings:IV");
            var conString = _config.GetValue<string>("ConnectionStrings:FinnacleDb");
            var secretKey = _config.GetValue<string>("ConnectionStrings:SecretKey");


            var url = "http://172.27.4.17/EncryptionServiceAPI/api/Encryption/decrypt?planValue=PLAINTEXT&secretKey=SECRETVALUE&iv=IVVALUE";
            url = url.Replace("PLAINTEXT", conString)
                .Replace("SECRETVALUE", secretKey)
                .Replace("IVVALUE", IV);

            var http = new HttpClient();
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(url);
            request.Method = HttpMethod.Post;
            var response = await http.SendAsync(request);         
            if (!response.IsSuccessStatusCode) return null;
            var constring = response.Content.ReadAsStringAsync().Result;
            return constring;
        }
        public async Task <object> GetBranch(string branch)
        {

            var branchList = new List<string>();
            var constring = Encryption().Result==null?"EncyptionService Unavailable": Encryption().Result;
            OracleConnection fibConn = new OracleConnection(constring);
            var sql = @"select sol_desc from TBAADM.SOL WHERE STATE_CODE= (select REF_CODE from tbaadm.rct where REF_REC_TYPE = '02' and REF_DESC = '{0}' and rownum=1) and del_flg = 'N'";
            var newSql = string.Format(sql, branch.ToUpper());
            try
            {
                fibConn.Open();
                var branches = await fibConn.QueryAsync<string>(newSql);
                branchList = branches.ToList();
                if (branchList.Count == 0) return "No branch was Found";
                return branchList;
            }
            catch (Exception ex) { return ex.Message; }
        }
        public async Task<object> GetTransaction(TransactionModel model)
        {

            var conString = Encryption().Result == null ? "EncyptionService Unavailable" : Encryption().Result;

            model.TransDate.Replace("/", "-");
                  
         try
            {
            var trans = new char[9];      
            Array.ConstrainedCopy(model.TransId.ToCharArray(), 0, trans, 9 - model.TransId.Length, model.TransId.Length);

            if(model.TransId.Length < 9)
            {
            for (int i = 0; i < (9-model.TransId.Length); i++)
                {
                    trans[i] =' ';
                }
             }
            var transQ = new String(trans);          
            var querytemplate = _config.GetValue<string>("FinnacleQueries:NewTransaction");
                var query = querytemplate
                 .Replace("EntityDate", model.TransDate)
                 .Replace("EntityId", transQ);                 
           
                OracleConnection fibConn = new OracleConnection(conString);         
                await fibConn.OpenAsync();           
                var transacts = await fibConn.QueryAsync<object>(query);

                //  var transactions=transacts.AsList<object>();
                if (transacts.Count() == 0)
                {
                    return "Nil";
                }

                //var result = transactions.Adapt<List<GetTransactionResponse>>();
                //var index = 0;
                //foreach (var e in result)
                //{
                //    index = e.ENTITY_DATE.IndexOf(" ");
                //    e.ENTITY_DATE.Replace('-', '/');
                //    e.ENTITY_DATE = e.ENTITY_DATE.Remove(index);
                //}
                //return result;
                return transacts;

            }
            catch (Exception ex)
            {
                return new { statusCode = 400, message = ex.Message };
            }
        }


    }
}
