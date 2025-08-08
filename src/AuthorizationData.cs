namespace Stonebot {
    using Twitch;

    internal class AuthorizationData : IDisposable {
        public readonly string UserId;
        public readonly string UserLogin;
        public readonly AccessToken AccessToken;

        public static AuthorizationData CreateFromRefreshToken(string clientId, string clientSecret, string refreshToken, CancellationToken cancellationToken) {
            var accessToken = AccessToken.CreateFromRefreshToken(clientId, clientSecret, refreshToken, cancellationToken);
            return Create(accessToken, cancellationToken);
        }

        public static AuthorizationData CreateFromScopes(string clientId, string clientSecret, string scopes, CancellationToken cancellationToken) {
            var accessToken = AccessToken.CreateFromScopes(clientId, clientSecret, scopes, cancellationToken);
            return Create(accessToken, cancellationToken);
        }

        public void Dispose() => AccessToken.Dispose();

        private AuthorizationData(string userId, string userLogin, AccessToken accessToken) {
            UserId = userId;
            UserLogin = userLogin;
            AccessToken = accessToken;
        }

        private static AuthorizationData Create(AccessToken accessToken, CancellationToken cancellationToken) {
            var userData = User.GetUser(accessToken, cancellationToken);
            return new(userData.Id, userData.Login, accessToken);
        }
    }
}
