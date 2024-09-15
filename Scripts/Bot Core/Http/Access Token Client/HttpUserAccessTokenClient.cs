namespace StoneBot.Scripts.Bot_Core.Http.Access_Token_Client {
    using Access_Token;

    internal class HttpUserAccessTokenClient : HttpAccessTokenClient {
        public HttpUserAccessTokenClient(UserAccessToken accessToken) : base(accessToken) { }
    }
}
