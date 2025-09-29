using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace MainUi
{
    public static class FacebookAuthService
    {
        private const string FacebookAppId = "1471778957279642"; // thay bằng AppId thật
        private const string RedirectUri = "https://www.facebook.com/connect/login_success.html";
        private const string Scope = "email,public_profile";

        public static async Task<string> LoginAndGetTokenAsync()
        {
            string facebookUrl =
                $"https://www.facebook.com/v23/dialog/oauth?client_id={FacebookAppId}" +
                $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}&scope={Scope}&response_type=token";

            WebAuthenticationResult result =
                await WebAuthenticationBroker.AuthenticateAsync(
                    WebAuthenticationOptions.None,
                    new Uri(facebookUrl),
                    new Uri(RedirectUri));
            
            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                // Kết quả: https://www.facebook.com/connect/login_success.html#access_token=XXX&expires_in=5183085
                var responseUri = new Uri(result.ResponseData);
                var fragment = responseUri.Fragment; // "#access_token=XXX&expires_in=..."
                var accessToken = fragment.Split('&')[0].Split('=')[1];
                return accessToken;
            }
            else if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                Debug.WriteLine("HTTP Error: " + result.ResponseErrorDetail);
            }
            else
            {
                Debug.WriteLine("Authentication failed: " + result.ResponseStatus);
            }
            return string.Empty;
        }
    }
}