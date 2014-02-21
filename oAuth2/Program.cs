using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace oAuth2
{
    class Program
    {
        static private String access_control_service_token_endpoint = "https://localhost/acs/issue/oauth2/token";
        static private String client_id = "testclient";
        static private String client_secret = "R+L/JkHn8ZayazKO/EvfOZnOb+6/ctOY/yk0f7f1zQQ=";
        static private String scope = "urn:testrp";
        static private String username = "<<YOURUSER>>";
        static private String password = "<<YOURPASSWORD>>";

        public static bool Validator(object sender, X509Certificate certificate, X509Chain chain,
                                      SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        static void Main(string[] args)
        {
            PasswordFlow();   
        }

        /// <summary>
        /// This method demonstrates the oAuth2 password flow against a thinktecture identity server. This flow should be 
        /// used when you are a server based application which is executing the login in behalf of a user who logs into
        /// your system.
        /// </summary>
        static void PasswordFlow()
        {
            // To make this examples working we just trust not valid SSL connection. This should not be done in production
            ServicePointManager.ServerCertificateValidationCallback = Validator;

            // First of all generate a request which contains a basic authentication header. The authentication against 
            // the server needs to be performed as the specific client which is configured as oAuth2 client in the 
            // Thinktecture server. The credentials for this are the client id and the client secret. At this point 
            // the systems knows which web server application tries to perform authorization in behalf of an user
            var request = WebRequest.Create(access_control_service_token_endpoint);
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(String.Format("{0}:{1}", client_id, client_secret)));
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            // In the second step the content needs to be generated. The content of the request contains the username and 
            // password of the user the application wants to get an access token for. In addition to this the scopename of 
            // the relying party which is repsonsible needs to be added as well.
            var content = String.Format("grant_type=password&username={0}&password={1}&scope={2}", username, password, scope);
            using (var contentWriter = new StreamWriter(request.GetRequestStream()))
            {
                contentWriter.Write(content);
            }

            // Now send the request and wait for the response which should contain a JSON structure including
            // the access token and a refresh token. 
            try
            {
                var response = request.GetResponse();
                var responseJson = new StreamReader(response.GetResponseStream()).ReadToEnd();
                Console.WriteLine(responseJson);
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
            }         
        }
    }
}
