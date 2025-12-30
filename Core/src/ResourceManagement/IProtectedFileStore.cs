namespace StonebotCore.ResourceManagement {
    using System.Threading;
    using System.Threading.Tasks;

    internal interface IProtectedFileStore {
        Task SaveAsync(
            string filePath,
            string data,
            CancellationToken cancellationToken
        );

        Task<string> LoadAsync(
            string filePath,
            CancellationToken cancellationToken
        );
    }
}
