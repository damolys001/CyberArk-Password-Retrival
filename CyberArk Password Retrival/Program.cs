// See https://aka.ms/new-console-template for more information

using CyberArk.AIM.NetPasswordSDK;
using CyberArk.AIM.NetPasswordSDK.Exceptions;
using System.Configuration;
using System.Threading;
namespace CyberArk_Password_Retrival
{
    public class Program
    {

        public static void Main()
        {
            _ = int.TryParse(ConfigurationManager.AppSettings["conDefaultPort"], out int conDefaultPort);
            _ = int.TryParse(ConfigurationManager.AppSettings["conDefaultTimeout"], out int conDefaultTimeout);
            string conDefaultAppID = ConfigurationManager.AppSettings["conDefaultAppID"];
            string conDefaultSafe = ConfigurationManager.AppSettings["conDefaultSafe"];
            string conDefaultFolder = ConfigurationManager.AppSettings["conDefaultFolder"];
            string conDefaultObject = ConfigurationManager.AppSettings["conDefaultObject"];
            string conDefaultReason = ConfigurationManager.AppSettings["conDefaultReason"];
            string conDefaultTemplate = ConfigurationManager.AppSettings["conDefaultTemplate"];



            VaultDetailsResponse responseDetail = GetConnectionData(conDefaultAppID, conDefaultSafe, conDefaultFolder, conDefaultObject, conDefaultReason, conDefaultPort, conDefaultTimeout);
            if (responseDetail != null)
                conDefaultTemplate.Replace("{IpAddress}", responseDetail.IpAddress)
                                       .Replace("{UserName}", responseDetail.UserName)
                                       .Replace("{Password}", responseDetail.Password);
        }

        public static VaultDetailsResponse GetConnectionData(string AppId, string Safe, string Folder, string ObjectConfig, string Reason, int ConnectionPort, int ConnectionTimeout)
        {
            VaultDetailsResponse vaultDetailsResponseRaw = new VaultDetailsResponse();
            PSDKPassword pSDKPassword = null;
            try
            {
                PSDKPasswordRequest pSDKPasswordRequest = new PSDKPasswordRequest
                {
                    AppID = AppId,
                    Safe = Safe,
                    Folder = Folder,
                    Object = ObjectConfig,
                    Reason = Reason,
                    ConnectionPort = ConnectionPort,
                    ConnectionTimeout = ConnectionTimeout,
                    FailRequestOnPasswordChange = false
                };
                pSDKPasswordRequest.RequiredProperties.Add("PolicyId");
                pSDKPasswordRequest.RequiredProperties.Add("UserName");
                pSDKPasswordRequest.RequiredProperties.Add("Address");

                bool flag = false;
                int millisecondsTimeout = 1500;
                while (!flag)
                {
                    pSDKPassword = PasswordSDK.GetPassword(pSDKPasswordRequest);
                    if (pSDKPassword.GetAttribute("PasswordChangeInProcess").Equals("True"))
                    {
                        Thread.Sleep(millisecondsTimeout);
                    }
                    else
                    {
                        flag = true;
                    }
                }

                string password = new System.Net.NetworkCredential(string.Empty, pSDKPassword.SecureContent).Password;
                vaultDetailsResponseRaw.Password = password;
                vaultDetailsResponseRaw.UserName = pSDKPassword.UserName;
                vaultDetailsResponseRaw.IpAddress = pSDKPassword.Address;


#if DEBUG
                Console.WriteLine("-----------Credential--------------");
                Console.WriteLine("UserName:" + vaultDetailsResponseRaw.UserName);
                Console.WriteLine("Pass:" + vaultDetailsResponseRaw.Password);
                Console.WriteLine("Pass The old way:" + pSDKPassword.Content);
                Console.WriteLine("Server:" + vaultDetailsResponseRaw.Database);
                Console.WriteLine("IP:" + vaultDetailsResponseRaw.IpAddress);

#endif

                return vaultDetailsResponseRaw;
            }
            catch (PSDKException ex)
            {
                throw ex;
            }
            finally
            {
                pSDKPassword?.Dispose();
            }

        }
    }
}