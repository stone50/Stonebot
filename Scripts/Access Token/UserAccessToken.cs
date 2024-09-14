namespace StoneBot.Scripts.Access_Token {
    using Models;
    using System.Threading.Tasks;

    internal class UserAccessToken : AccessToken {
        private readonly string RefreshToken;

        public UserAccessToken(UserAccessTokenData data) : base(data.AccessToken, data.ExpiresIn) => RefreshToken = data.RefreshToken;

        public override async Task<bool> Refresh() =>
            // TODO
            false;
    }
}
