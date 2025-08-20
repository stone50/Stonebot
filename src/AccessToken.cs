namespace Stonebot {
    using System;

    internal struct AccessToken(string tokenValue, DateTime expirationDate) {
        public readonly string Value = tokenValue;
        public readonly DateTime ExpirationDate = expirationDate;
    }
}
