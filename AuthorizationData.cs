namespace Stonebot {
    internal class AuthorizationData : IDisposable {
        public readonly string UserId;
        public readonly string UserLogin;
        public readonly AccessToken AccessToken;

        public static AuthorizationData Create(string clientId, string clientSecret, string refreshToken, CancellationToken cancellationToken) {
            var accessToken = AccessToken.Create(clientId, clientSecret, refreshToken, cancellationToken);
            return Create(accessToken, cancellationToken);
        }

        public static AuthorizationData Create(string clientId, string clientSecret, string[] scopes, CancellationToken cancellationToken) {
            var accessToken = AccessToken.Create(clientId, clientSecret, scopes, cancellationToken);
            return Create(accessToken, cancellationToken);
        }

        public void Dispose() => AccessToken.Dispose();

        private AuthorizationData(string userId, string userLogin, AccessToken accessToken) {
            UserId = userId;
            UserLogin = userLogin;
            AccessToken = accessToken;
        }

        private static AuthorizationData Create(AccessToken accessToken, CancellationToken cancellationToken) {
            var userData = Twitch.User.GetUser(accessToken, cancellationToken);
            return new(userData.Id, userData.Login, accessToken);
        }
    }
}
