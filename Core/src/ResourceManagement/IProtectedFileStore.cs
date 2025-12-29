namespace StonebotCore.ResourceManagement {
    internal interface IProtectedFileStore {
        void Save(string data, string filePath);

        string Load(string filePath);
    }
}
