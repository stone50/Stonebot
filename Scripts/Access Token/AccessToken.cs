namespace StoneBot.Scripts.Access_Token {
    using Godot;
    using System;
    using System.Threading.Tasks;

    internal abstract class AccessToken {
        protected string AccessTokenString;
        protected int ExpiresIn;

        protected DateTime LastRefreshDate = DateTime.Now;
        protected int ExpirationBufferMillis = 1000;

        public bool IsAboutToExpire => (DateTime.Now - LastRefreshDate).TotalMilliseconds + ExpirationBufferMillis > ExpiresIn;

        public AccessToken(string accessToken, int expiresIn) {
            AccessTokenString = accessToken;
            ExpiresIn = expiresIn;
        }

        public async Task<string?> GetString() {
            if (IsAboutToExpire && !await Refresh()) {
                GD.PushWarning("Cannot get access token string because refreshing the expired token failed.");
                return null;
            }

            return AccessTokenString;
        }

        public abstract Task<bool> Refresh();
    }
}
