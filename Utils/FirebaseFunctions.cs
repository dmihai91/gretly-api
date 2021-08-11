using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GretlyStudio.Dto;
using Newtonsoft.Json;

namespace GretlyStudio.Utils
{
    public class FirebaseFunctions
    {
        private static FirebaseFunctions instance = null;

        private const string API_URL = "https://us-central1-project-zero-a37a9.cloudfunctions.net/api";
        private static HttpClient client = new HttpClient();

        public static FirebaseFunctions GetInstance()
        {
            if (instance == null)
            {
                client.BaseAddress = new System.Uri(API_URL);
                instance = new FirebaseFunctions();
            }
            return instance;
        }

        public async Task<VatNumberValidatorResponse> CheckVatNumber(string country, string vatNumber)
        {
            try
            {
                var json = JsonConvert.SerializeObject(new { country = country, vatNumber = vatNumber });
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/checkVatNumber", data);
                var responseContent = await response.Content.ReadAsStringAsync();
                var deserializedContent = JsonConvert.DeserializeObject<VatNumberValidatorResponse>(responseContent);
                return deserializedContent;
            }
            catch (System.Exception error)
            {
                throw error;
            }
        }
    }
}