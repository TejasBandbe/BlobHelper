using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobFunctions.Helpers
{
    public class KeyVaultHelper
    {
        public static string BlobConnectionString { get; set; }
        public static string GetSecret(string key)
        {
            try
            {
                string strVaultURL = Environment.GetEnvironmentVariable("KeyVaultUrl").ToString();
                var client = new SecretClient(new Uri(strVaultURL), new DefaultAzureCredential());

                KeyVaultSecret secret = client.GetSecret(key);

                return secret.Value;
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        public static void FetchKeyvalutSecrets()
        {
            try
            {
                string blobConnectionStringKey = Environment.GetEnvironmentVariable("BlobConnectionString").ToString();
                if (string.IsNullOrEmpty(blobConnectionStringKey))
                {
                    throw new Exception("BlobConnectionString missing in App Settings");
                }
                KeyVaultHelper.BlobConnectionString = KeyVaultHelper.GetSecret(blobConnectionStringKey);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
