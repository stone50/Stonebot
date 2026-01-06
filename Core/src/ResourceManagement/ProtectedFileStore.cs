namespace StonebotCore.ResourceManagement {
    using System;

    internal static class ProtectedFileStore {
        private static readonly Lazy<IProtectedFileStore> _instance = new(() =>
#if WINDOWS
            new WindowsDpapiFileStore()
#else
            new DataProtectionFileStore()
#endif
        );

        internal static IProtectedFileStore Instance => _instance.Value;
    }
}
