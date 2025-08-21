namespace Stonebot {
    using System;

    internal readonly struct AccessToken(string tokenValue, DateTime expirationDate) {
        public readonly string Value = tokenValue;
        public readonly DateTime ExpirationDate = expirationDate;
    }
}
