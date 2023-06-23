// See https://aka.ms/new-console-template for more information
using CyberArk_Password_Retrival;


  VaultDetailsResponse GetConnectionData(string AppId, string Safe, string Folder, string ObjectConfig, string Reason, int ConnectionPort, int ConnectionTimeout)
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
