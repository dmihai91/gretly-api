using Firebase.Auth;

namespace GretlyStudio.Utils
{
    public class FirebaseClient
    {
        private static FirebaseAuthProvider firebaseClient = null;
        private static string API_KEY = null;

        public static void SetApiKey(string apiKey)
        {
            API_KEY = apiKey;
        }

        public static string GetApiKey()
        {
            return API_KEY;
        }

        public static FirebaseAuthProvider GetClient()
        {
            if (firebaseClient == null)
            {
                firebaseClient = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));
            }
            return firebaseClient;
        }
    }
}