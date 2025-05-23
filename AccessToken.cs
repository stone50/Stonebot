namespace Stonebot {
    using Models.Data;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Twitch;

    internal sealed class AccessToken : IDisposable {
        public readonly string ClientId;
        public readonly string ClientSecret;
        public string[] Scopes { get; private set; }
        public string RefreshToken { get; private set; }
        public DateTime ExpirationDate { get; private set; }

        public bool ShouldRefresh => IsExpired || IsAboutToExpire;
        public bool IsExpired => DateTime.UtcNow >= ExpirationDate;
        public bool IsAboutToExpire => DateTime.UtcNow.AddMilliseconds(Config.AccessTokenExpirationMarginMillis) >= ExpirationDate;

        public static async Task<AccessToken> CreateAsync(string clientId, string clientSecret, string refreshToken, CancellationToken cancellationToken) {
            var accessTokenData = await Auth.RefreshAccessTokenAsync(clientId, clientSecret, refreshToken, cancellationToken).ConfigureAwait(false);
            return FromData(accessTokenData, clientId, clientSecret);
        }

        public static async Task<AccessToken> CreateAsync(string clientId, string clientSecret, string[] scopes, CancellationToken cancellationToken) {
            var accessTokenData = await Auth.GetAccessTokenAsync(clientId, clientSecret, scopes, cancellationToken).ConfigureAwait(false);
            return FromData(accessTokenData, clientId, clientSecret);
        }

        public async Task<string> GetValueAsync(CancellationToken cancellationToken) {
            if (ShouldRefresh) {
                await RefreshAsync(cancellationToken).ConfigureAwait(false);
            }

            return rawValue;
        }

        public async Task RefreshAsync(CancellationToken cancellationToken) {
            var accessTokenData = await Auth.RefreshAccessTokenAsync(ClientId, ClientSecret, RefreshToken, cancellationToken).ConfigureAwait(false);
            ExpirationDate = DateTime.UtcNow.AddSeconds(accessTokenData.ExpiresIn);
            rawValue = accessTokenData.AccessToken;
            Scopes = accessTokenData.Scope;
            RefreshToken = new(accessTokenData.RefreshToken);
        }

        public async ValueTask<HttpClient> GetHttpClientAsync(CancellationToken cancellationToken) {
            if (!ShouldRefresh) {
                if (cachedClient is not null) {
                    return cachedClient;
                }

                cachedClient = new HttpClient();
                cachedClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {rawValue}");
                cachedClient.DefaultRequestHeaders.Add("Client-Id", ClientId);
                return cachedClient;
            }

            await RefreshAsync(cancellationToken).ConfigureAwait(false);
            if (cachedClient is null) {
                cachedClient = new HttpClient();
                cachedClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {rawValue}");
                cachedClient.DefaultRequestHeaders.Add("Client-Id", ClientId);
            } else {
                _ = cachedClient.DefaultRequestHeaders.Remove("Authorization");
                cachedClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {rawValue}");
            }

            return cachedClient;
        }

        public void Dispose() => cachedClient?.Dispose();

        private string rawValue;
        private HttpClient? cachedClient;

        private AccessToken(string rawValue, string clientId, string clientSecret, string[] scopes, string refreshToken, DateTime expirationDate) {
            this.rawValue = rawValue;
            ClientId = clientId;
            ClientSecret = clientSecret;
            Scopes = scopes;
            RefreshToken = refreshToken;
            ExpirationDate = expirationDate;
        }

        private static AccessToken FromData(AccessTokenData accessTokenData, string clientId, string clientSecret) => new(
            accessTokenData.AccessToken,
            clientId,
            clientSecret,
            accessTokenData.Scope,
            accessTokenData.RefreshToken,
            DateTime.UtcNow.AddSeconds(accessTokenData.ExpiresIn)
        );
    }
}
