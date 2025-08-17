namespace Stonebot {
    using System;
    using Twitch;

    internal class AccessToken {
        public string Value { get; private set; }
        public string RefreshValue { get; private set; }
        public DateTime ExpirationDate { get; private set; }

        public AccessToken() {
            var accessToken = Auth.GetAccessToken();
            Value = accessToken.AccessToken;
            RefreshValue = accessToken.RefreshToken;
            ExpirationDate = DateTime.UtcNow.AddSeconds(accessToken.ExpiresIn);
        }

        public void Refresh() {
            var newAccessToken = Auth.GetRefreshedAccessToken(RefreshValue);
            Value = newAccessToken.AccessToken;
            RefreshValue = newAccessToken.RefreshToken;
            ExpirationDate = DateTime.UtcNow.AddSeconds(newAccessToken.ExpiresIn);
        }
    }
}
