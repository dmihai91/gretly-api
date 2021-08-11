using System.Collections.Generic;
using System.Threading.Tasks;
using FaunaDB.Types;
using GretlyStudio.Models;
using Newtonsoft.Json;

using static FaunaDB.Query.Language;

namespace GretlyStudio.Utils
{
    public class DBHelpers
    {
        public async static Task<FaunaResultDto[]> SearchIndex(string index, string term)
        {
            var response = await FaunaDbClient.QueryDb(
                   Map(
                       Paginate(Match(Index(index), term)),
                       Lambda("ref", Get(Var("ref")))
                   )
               );
            FaunaResultDto[] data = Decoder.Decode<FaunaResultDto[]>(response.At("data"));
            return data;
        }

        public async static Task<T> FindTermInIndex<T>(string index, string term) where T : FaunaEntity
        {
            var result = await SearchIndex(index, term);
            if (result.Length == 1)
            {
                var value = result[0].Data.To<T>().Value;
                value.Id = result[0].Ref.Id;
                return value;
            }
            return default(T);
        }

        public static T ConvertObjectVToType<T>(ObjectV data)
        {
            var output = JsonConvert.SerializeObject(data);
            return JsonConvert.DeserializeObject<T>(output);
        }

        public static async Task<List<T>> GetAllDocumentsFromCollection<T>(string collection) where T : FaunaEntity
        {
            var value = await FaunaDbClient.GetDocuments(collection);
            var result = value.At("data").To<FaunaResultDto[]>().Value;
            var list = new List<T>();
            foreach (var res in result)
            {
                var data = res.Data.To<T>().Value;
                data.Id = res.Ref.Id;
                list.Add(data);
            }
            return list;
        }
    }
}