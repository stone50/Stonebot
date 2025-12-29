namespace StonebotCore.ResourceManagement {
    internal static class ProtectedFileStore {
        internal static IProtectedFileStore Create() =>
#if WINDOWS
            new WindowsDpapiFileStore();
#else
            new DataProtectionFileStore();
#endif
    }
}
