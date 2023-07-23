using System.Data;
using IsoLevelsAdoNet.Repos;

namespace IsoLevelsAdoNet.Phenomena;

public class NonRepeatableReadPhenomenon : IPhenomenon
{
    private readonly IAlbumRepository _repo;

    public NonRepeatableReadPhenomenon(IAlbumRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public void Demo(IsolationLevel iso = IsolationLevel.ReadCommitted)
    {
        using var cts = new CancellationTokenSource();
        var readSyncEvent = new ManualResetEvent(false);

        var t1 = new Thread(() =>
        {
            var containerId = Thread.CurrentThread.ManagedThreadId;

            var t = _repo.TransactionScope(async (transaction, cancellation) =>
            {
                // read
                var album = await _repo.GetAsync(1, transaction, cts.Token);

                // update
                album!.Price += 0.01m;
                var updated = await _repo.UpdateAsync(album, transaction, cts.Token);
                Console.WriteLine($"[{containerId}] {updated} Album update");

                // commit
                await transaction.CommitAsync(cts.Token);
                Console.WriteLine($"[{containerId}] transaction committed");

                readSyncEvent.Set();
            }, iso, cts.Token);

            t.GetAwaiter().GetResult();
        });

        var t2 = new Thread(() =>
        {
            var containerId = Thread.CurrentThread.ManagedThreadId;
            var t = _repo.TransactionScope(async (transaction, cancellation) =>
            {
                // read
                var album1 = await _repo.GetAsync(1, transaction, cts.Token); // Result 1: NON REPEATABLE READ
                Console.WriteLine($"[{containerId}] Non Repeatable Read {album1}");

                // use Timeout to prevent deadlock if iso is RepeableRead
                readSyncEvent.WaitOne(TimeSpan.FromSeconds(5));

                // read again
                var album2 = await _repo.GetAsync(1, transaction, cts.Token); // Result 2: Result 1 != Result2
                Console.WriteLine($"[{containerId}] Read AGAIN {album2}");
            }, iso, cts.Token);

            t.GetAwaiter().GetResult();
        });

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();
    }
}