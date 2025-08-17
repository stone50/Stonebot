namespace Stonebot.Twitch {
    using Models.Responses;
    using Resources;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;

    internal static class Auth {
        public static GetAccessTokenResponse GetAccessToken() {
            var state = GetState();
            StartAuthProcess(state);
            var authCode = GetAuthCode(state);
            var accessToken = GetAccessToken(authCode);
            return accessToken;
        }

        public static GetAccessTokenResponse GetRefreshedAccessToken(string refreshToken) {
            var url = Utils.GetUrl("https://id.twitch.tv/oauth2/token", new() {
                { "client_id", Config.ClientId },
                { "client_secret", Config.ClientSecret },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
            });
            return Utils.SendUnauthorizedPostRequest(url, JsonContext.Default.GetAccessTokenResponse);
        }

        private static string GetState() {
            var stateChars = new char[Constants.AuthStateLength];
            for (var i = 0; i < stateChars.Length; ++i) {
                stateChars[i] = Constants.AuthStateAllowedChars[RandomNumberGenerator.GetInt32(Constants.AuthStateAllowedChars.Length)];
            }

            return new(stateChars);
        }

        private static void StartAuthProcess(string state) {
            var authUrl = Utils.GetUrl("https://id.twitch.tv/oauth2/authorize", new() {
                { "client_id", Config.ClientId },
                { "force_verify", "true" },
                { "redirect_uri", Constants.AuthRedirectUri },
                { "response_type", "code" },
                { "scope", Constants.AuthScopes },
                { "state", state },
            });
            _ = Process.Start(new ProcessStartInfo { FileName = authUrl, UseShellExecute = true });
        }

        private static string GetAuthCode(string state) {
            using var listener = new HttpListener();
            listener.Prefixes.Add(Constants.AuthRedirectUri);
            listener.Start();

            var context = listener.GetContext();

            void respond(string response) {
                var buffer = Encoding.UTF8.GetBytes(response);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
                listener.Stop();
            }

            void respondFail(string reason) {
                respond(Embedded.AuthFailHtml.Replace("<error-message />", reason));
                throw new Exception(reason);
            }

            if (context.Request.QueryString["state"] != state) {
                respondFail("The received state does not match the sent state.");
            }

            var authCode = context.Request.QueryString["code"];
            if (string.IsNullOrEmpty(authCode)) {
                respondFail("The received code is null or empty.");
            }

            respond(Embedded.AuthSuccessHtml);
            return authCode!;
        }

        private static GetAccessTokenResponse GetAccessToken(string authCode) {
            var url = Utils.GetUrl("https://id.twitch.tv/oauth2/token", new() {
                { "client_id", Config.ClientId },
                { "client_secret", Config.ClientSecret },
                { "code", authCode },
                { "grant_type", "authorization_code" },
                { "redirect_uri", Constants.AuthRedirectUri }
            });
            return Utils.SendUnauthorizedPostRequest(url, JsonContext.Default.GetAccessTokenResponse);
        }
    }
}
