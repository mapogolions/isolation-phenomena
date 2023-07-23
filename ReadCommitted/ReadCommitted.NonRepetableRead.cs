using System.Data;

namespace IsoLevelsAdoNet;

public partial class ReadCommitted
{
    public void NonRepeatableRead()
    {
        using var cts = new CancellationTokenSource();
        var readSyncEvent = new ManualResetEvent(false);

        var t1 = new Thread(() =>
        {
            var containerId = Thread.CurrentThread.ManagedThreadId;

            var t = _repo.TransactionScope(async (transaction, cancellation) =>
            {
                var album = await _repo.GetAsync(1, transaction, cts.Token);
                Console.WriteLine($"[{containerId}] Read {album}");
                album!.Price += 0.01m;
                var updated = await _repo.UpdateAsync(album, transaction, cts.Token);
                Console.WriteLine($"[{containerId}] {updated} Album update");
                await transaction.CommitAsync(cts.Token);
                Console.WriteLine($"[{containerId}] transaction committed");

                readSyncEvent.Set();
            }, IsolationLevel.ReadCommitted, cts.Token);

            t.GetAwaiter().GetResult();
        });

        var t2 = new Thread(() =>
        {
            var containerId = Thread.CurrentThread.ManagedThreadId;
            var t = _repo.TransactionScope(async (transaction, cancellation) =>
            {
                var album1 = await _repo.GetAsync(1, transaction, cts.Token); // Result 1: NON REPEATABLE READ
                Console.WriteLine($"[{containerId}] Read {album1}");

                readSyncEvent.WaitOne();

                var album2 = await _repo.GetAsync(1, transaction, cts.Token); // Result 2: Result 1 != Result2
                Console.WriteLine($"[{containerId}] Read {album2}");
            }, IsolationLevel.ReadCommitted, cts.Token);

            t.GetAwaiter().GetResult();
        });

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();
    }
}
