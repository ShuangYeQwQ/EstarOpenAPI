using System.Threading.Tasks;
using FireStoreModel;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System.Configuration;
using Newtonsoft.Json;
using Google.Apis.Auth;
using FirebaseAdmin;
using Grpc.Net.Client;
namespace EStarGoogleCloud
{

    public class GoogleFirestoreDBHelper
    {
        private static string firestoreuri = ConfigurationManager.AppSettings["firestoreuri"] + "";
        //path   chat/chatone/firstchat/firstchatone
        public static async Task<FireStoreDataResult> GetFirestoreData(string path)
        {
            FireStoreDataResult fireStoreDataResult = new FireStoreDataResult();
            fireStoreDataResult.code = 1;
            fireStoreDataResult.message = "fail";
            try
            {
                GoogleCredential credential = GoogleCredential.GetApplicationDefault();
                if (credential.IsCreateScopedRequired)
                {
                    credential = credential.CreateScoped(new[]
                    {
                    "https://www.googleapis.com/auth/cloud-platform"
                });
                }

                HttpClient httpClient = new Google.Apis.Http.HttpClientFactory()
                    .CreateHttpClient(new Google.Apis.Http.CreateHttpClientArgs()
                    {
                        ApplicationName = "Google Cloud Platform Firestore Sample",
                        GZipEnabled = true,
                        Initializers = { credential },
                    });
                string uriString = $"{firestoreuri}/{path}";
                UriBuilder uri = new UriBuilder(uriString);
                //HttpResponseMessage response = await httpClient.GetAsync(uri.Uri);
                //response.EnsureSuccessStatusCode(); // Throw if the response status code is not success

                // string resultText = await response.Content.ReadAsStringAsync();
                string resultText = httpClient.GetAsync(uri.Uri).Result.Content.ReadAsStringAsync().Result;
                FireStoreData result = JsonConvert.DeserializeObject<FireStoreData>(resultText);
                if (!string.IsNullOrEmpty(result.fields + ""))
                {
                    fireStoreDataResult.code = 0;
                    fireStoreDataResult.message = "success";
                    fireStoreDataResult.fireStoreData = result;
                }
                else
                {
                    fireStoreDataResult.code = 1;
                    fireStoreDataResult.message = "fail";
                    fireStoreDataResult.fireStoreData = result;
                }
            }
            catch (HttpRequestException e)
            {
                fireStoreDataResult.message = e.Message + "";
            }
            catch (JsonSerializationException e)
            {
                fireStoreDataResult.message = e.Message + "";
            }
            catch (Exception e)
            {
                fireStoreDataResult.message = e.Message + "";
            }
            return fireStoreDataResult;
        }

        public static async Task GetGoogleFirestore()
        {
            //string path = "C:\\work\\EStarOpenAPI\\EStarGoogleCloudDAL\\semiotic.json";
            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            FirestoreDb db = FirestoreDb.Create("semiotic-art-418621"); //GetFirestoreClient();
            // [START firestore_transaction_document_update]
            DocumentReference cityRef = db.Collection("chat").Document("chatone").Collection("firstchat").Document("firstchatone");
            await db.RunTransactionAsync(async transaction =>
            {
                DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(cityRef);
                long newPopulation = snapshot.GetValue<long>("Population") + 1;
                Dictionary<string, object> updates = new Dictionary<string, object>
                {
                    { "Population", newPopulation}
                };
            });
        }

        static FirestoreDb GetFirestoreClient() =>
        new FirestoreDbBuilder { ProjectId = "semiotic-art-418621", DatabaseId = "estarfile" }.Build();

        //    public static async Task GetGoogleFirestore()
        //    {
        //        GoogleCredential credential =
        //            GoogleCredential.GetApplicationDefault();
        //                //Inject the Cloud Platform scope if required.
        //                if (credential.IsCreateScopedRequired)
        //                {
        //                    credential = credential.CreateScoped(new[]
        //                    {
        //                    "https://www.googleapis.com/auth/cloud-platform"
        //                });
        //                }
        //HttpClient http = new Google.Apis.Http.HttpClientFactory()
        //    .CreateHttpClient(
        //    new Google.Apis.Http.CreateHttpClientArgs()
        //    {
        //        ApplicationName = "Google Cloud Platform Firestore Sample",
        //        GZipEnabled = true,
        //        Initializers = { credential },
        //    });

        //UriBuilder uri = new UriBuilder(httpurl);
        //var resultText = http.GetAsync(uri.Uri).Result.Content
        //    .ReadAsStringAsync().Result;
        //return Newtonsoft.Json.JsonConvert
        //    .DeserializeObject<FireStoreDataResult>(resultText);

        //        string path = "C:\\work\\EStarOpenAPI\\EStarGoogleCloudDAL\\semiotic.json";
        //        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
        //        FirestoreDb db = GetFirestoreClient();
        //        CollectionReference citiesRef = db.Collection("chat");
        //        Query query = citiesRef.WhereEqualTo("Content", true);

        //        try
        //        {
        //            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

        //            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        //            {
        //                if (documentSnapshot.Exists)
        //                {
        //                    Console.WriteLine($"Document data: {documentSnapshot.ToDictionary()}");
        //                }
        //                else
        //                {
        //                    Console.WriteLine("No matching documents.");
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine($"Query error: {e.Message}");
        //        }
        //    }



        //    static FirestoreDb GetFirestoreClient() =>
        //new FirestoreDbBuilder { ProjectId = "semiotic-art-418621", DatabaseId = "estarfile" }.Build();

        //    static async Task GetGoogleFirestore(FirestoreDb db)
        //    {


        //        //FirestoreDb.Create("semiotic-art-418621/estarfile");

        //        //Test test = new Test();
        //        //DocumentReference docRef = db.Collection("chat").Document("chatone").Collection("firstchat").Document("firstchatone");
        //        //var snapshot = await docRef.GetSnapshotAsync();
        //        //if (snapshot.Exists)
        //        //{
        //        //    test = snapshot.ConvertTo<Test>();
        //        //}
        //        //else
        //        //{
        //        //    Console.WriteLine("Document {0} does not exist!", snapshot.Id);

        //        //}
        //        //return test;
        //    }
    }



}

