using System.Threading.Tasks;
using FaunaDB.Client;
using FaunaDB.Types;

using static FaunaDB.Query.Language;

namespace Gretly.Utils
{
    public class FaunaDbClient
    {
        private static string ENDPOINT = null;
        private static string SECRET = null;
        private static FaunaClient client = null;

        public static void SetConfig(string endpoint, string secret)
        {
            ENDPOINT = endpoint;
            SECRET = secret;
        }

        public static FaunaClient GetClient()
        {
            if (client == null)
            {
                client = new FaunaClient(endpoint: ENDPOINT, secret: SECRET);
            }
            return client;
        }

        public static async Task<Value> CreateDocument(string collection, object data)
        {
            var client = GetClient();
            return await client.Query(
                Create(
                    Collection(collection),
                    Obj("data", Encoder.Encode(data))
                )
            );
        }

        public static async Task<Value> GetDocument(string collection, string documentId)
        {
            var client = GetClient();
            return await client.Query(
                Get(
                    Ref(
                        Collection(collection),
                        documentId
                    )
                )
            );
        }

        public static async Task<Value> GetDocuments(string collection)
        {
            var client = GetClient();
            return await client.Query(Map(
                Paginate(Documents(Collection(collection))),
                Lambda(x => Get(x)))
            );
        }

        public static async Task<Value> GetDocumentByIndex(string indexId, string value)
        {
            var client = GetClient();
            return await client.Query(Get(Match(Index(indexId), value)));
        }

        public static async Task<Value> UpdateDocument(string collection, string documentId, object data)
        {
            var client = GetClient();
            return await client.Query(
                Update(
                    Ref(
                        Collection(collection),
                        documentId
                    ),
                    Obj("data", Encoder.Encode(data))
                )
            );
        }

        public static async Task<Value> DeleteDocument(string collection, string documentId)
        {
            var client = GetClient();
            return await client.Query(
                Delete(
                    Ref(
                        Collection(collection),
                        documentId
                    )
                ));
        }

        public static async Task<Value> QueryDb(FaunaDB.Query.Expr query)
        {
            var client = GetClient();
            return await client.Query(query);
        }

        public static async Task<RefV> GetRef(string collection, string id)
        {
            var client = GetClient();
            var value = await GetDocument(collection, id);
            var result = value.To<FaunaResultDto>().Value;
            return result.Ref;
        }
    }
}