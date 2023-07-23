using System.Data;
using IsoLevelsAdoNet.Repos;

namespace IsoLevelsAdoNet.Phenomena;

public class DirtyReadPhenomen : IPhenomen
{
    private readonly IAlbumRepository _repo;

    public DirtyReadPhenomen(IAlbumRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));;
    }

    public void Demo(IsolationLevel iso = IsolationLevel.ReadUncommitted)
    {
        using var cts = new CancellationTokenSource();
        var readSyncEvent = new ManualResetEvent(false);

        var t1 = new Thread(() =>
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var t = _repo.TransactionScope(async (transaction, cancellation) =>
            {
                var album = await _repo.GetAsync(1, transaction, cancellation);
                Console.WriteLine($"[{threadId}] {album}");
                album!.Price += 0.01m;
                var updated = await _repo.UpdateAsync(album, transaction, cancellation);
                Console.WriteLine($"[{threadId}] {updated} Album updated");

                readSyncEvent.Set();

                // DO NOT commit transaction to cause DIRTY READ
                await Task.Delay(TimeSpan.FromSeconds(2));

            }, iso, cts.Token);

            t.GetAwaiter().GetResult();
        });

        var t2 = new Thread(() =>
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var t = _repo.TransactionScope(async (transaction, cancellation) => {
                readSyncEvent.WaitOne();

                var album = await _repo.GetAsync(1, transaction, cancellation); // Dirty Read
                Console.WriteLine($"[{threadId}] DIRTY READ {album}");

            }, iso, cts.Token);

            t.GetAwaiter().GetResult();
        });

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();
    }
}
