namespace StoneBot.Scripts.Bot_Core.Http.Access_Token_Client {
    using Access_Token;

    internal class HttpAppAccessTokenClient : HttpAccessTokenClient {
        public HttpAppAccessTokenClient(AppAccessToken accessToken) : base(accessToken) { }
    }
}
