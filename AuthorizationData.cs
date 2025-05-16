namespace Stonebot {
    internal class AuthorizationData : IDisposable {
        public readonly string UserId;
        public readonly string UserLogin;
        public readonly AccessToken AccessToken;

        public static async Task<AuthorizationData> CreateAsync(string clientId, string clientSecret, string refreshToken, CancellationToken cancellationToken) {
            var accessToken = await AccessToken.CreateAsync(clientId, clientSecret, refreshToken, cancellationToken).ConfigureAwait(false);
            return await CreateAsync(accessToken, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<AuthorizationData> CreateAsync(string clientId, string clientSecret, string[] scopes, CancellationToken cancellationToken) {
            var accessToken = await AccessToken.CreateAsync(clientId, clientSecret, scopes, cancellationToken).ConfigureAwait(false);
            return await CreateAsync(accessToken, cancellationToken).ConfigureAwait(false);
        }

        public void Dispose() => AccessToken.Dispose();

        private AuthorizationData(string userId, string userLogin, AccessToken accessToken) {
            UserId = userId;
            UserLogin = userLogin;
            AccessToken = accessToken;
        }

        private static async Task<AuthorizationData> CreateAsync(AccessToken accessToken, CancellationToken cancellationToken) {
            var userData = await Twitch.User.GetUserAsync(accessToken, cancellationToken).ConfigureAwait(false);
            return new(userData.Id, userData.Login, accessToken);
        }
    }
}
