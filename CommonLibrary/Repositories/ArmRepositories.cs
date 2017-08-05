#region using
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
#endregion

namespace CommonLibrary.Repositories
{
    /// <summary>
    /// Tools to request Azure
    /// </summary>
    public class ArmRepositories : IArmRepositories
    {
        #region GetArmRequest
        /// <summary>
        /// Run an HTTP request to Azure Resource Management Service
        /// </summary>
        /// <param name="urlPath">Url's path to process. Should end with '/'</param>
        /// <param name="armToken">Arm token</param>
        /// <returns>A JSON response or InvalidOperationException</returns>
        public async Task<string> GetArmRequest(string urlPath, string armToken)
        {
            using (HttpClient client = GetClient(Constantes.Endpoints.ArmEndpoint, armToken))
            {
                HttpResponseMessage message = await client.GetAsync(urlPath + $"?api-version={Constantes.ServiceVersion.Arm}");
                if (message.IsSuccessStatusCode)
                {
                    string json = await message.Content.ReadAsStringAsync();
                    return json;
                }

                throw new InvalidOperationException(message.ReasonPhrase);
            }
        }
        #endregion

        #region GetClient (private)
        /// <summary>
        /// Get an HttpClient
        /// </summary>
        /// <param name="host">Url's host</param>
        /// <param name="token">authorization token</param>
        /// <returns></returns>
        private HttpClient GetClient(string host, string token)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(host);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        } 
        #endregion
    }
}
