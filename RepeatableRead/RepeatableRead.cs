using System.Data;
using IsoLevelsAdoNet.Repos;

namespace IsoLevelsAdoNet;

public partial class RepeatableRead
{
    private readonly IAlbumRepository _repo;

    public RepeatableRead(IAlbumRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public void PreventNonRepetableRead()
    {
        using var cts = new CancellationTokenSource();
        var readSyncEvent = new ManualResetEvent(false);

        var t1 = new Thread(() =>
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var t = _repo.TransactionScope(async (transaction, cancellation) =>
            {
                readSyncEvent.WaitOne();

                // read
                var album = await _repo.GetAsync(1, transaction, cancellation);
                Console.WriteLine($"[{threadId}] {album}");

                // update
                album!.Price += 0.01m;
                var updated = await _repo.UpdateAsync(album, transaction, cancellation);
                Console.WriteLine($"[{threadId}] {updated} Album updated");

                // commit
                await transaction.CommitAsync(cancellation);
                Console.WriteLine($"[{threadId}] transaction committed");
            }, IsolationLevel.RepeatableRead, cts.Token);

            t.GetAwaiter().GetResult();
        });

        var t2 = new Thread(() =>
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var t = _repo.TransactionScope(async (transaction, cancellation) =>
            {
                var album1 = await _repo.GetAsync(1, transaction, cancellation);
                Console.WriteLine($"[{threadId}] {album1}");

                readSyncEvent.Set();

                await Task.Delay(TimeSpan.FromSeconds(4));
                var album2 = await _repo.GetAsync(1, transaction, cancellation);
                Console.WriteLine($"[{threadId}] {album2}");

                await Task.Delay(TimeSpan.FromSeconds(10));
            }, IsolationLevel.RepeatableRead, cts.Token);

            t.GetAwaiter().GetResult();
        });

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();
    }
}
