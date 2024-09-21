﻿namespace StoneBot.Scripts.Bot_Core.Twitch {
    using Godot;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal static partial class TwitchAPI {
        public static Process? Authorize(string clientId, string redirectUri, string[] scope, bool forceVerify = false, string? state = null) {
            var scopeParam = string.Join(" ", scope);

            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = $"https://id.twitch.tv/oauth2/authorize?client_id={clientId}&force_verify={forceVerify}&redirect_uri={redirectUri}&response_type=code&scope={scopeParam}";
            if (state is not null) {
                process.StartInfo.FileName += $"&state={state}";
            }

            try {
                _ = process.Start();
            } catch (Exception e) {
                GD.PushWarning($"Canot authorize because process.Start failed: {e}.");
                return null;
            }

            return process;
        }

        // no access token
        public static async Task<HttpResponseMessage?> GetAccessToken(HttpClient client, string clientId, string clientSecret, string authorizationCode, string redirectUri) {
            try {
                return await client.PostAsync($"https://id.twitch.tv/oauth2/token?&client_id={clientId}&client_secret={clientSecret}&code={authorizationCode}&grant_type=authorization_code&redirect_uri={redirectUri}", null);
            } catch (Exception e) {
                GD.PushWarning($"Cannot get access token because client.PostAsync failed: {e}.");
                return null;
            }
        }

        // no access token
        public static async Task<HttpResponseMessage?> RefreshAccessToken(HttpClient client, string clientId, string clientSecret, string refreshToken) {
            try {
                return await client.PostAsync($"https://id.twitch.tv/oauth2/token?client_id={clientId}&client_secret={clientSecret}&grant_type=refresh_token&refresh_token={refreshToken}", null);
            } catch (Exception e) {
                GD.PushWarning($"Cannot refresh access token because client.PostAsync failed: {e}.");
                return null;
            }
        }
    }
}
