using System.Data;

namespace IsoLevelsAdoNet;

public partial class RepeatableRead
{
    public void DeadLock()
    {
        using var cts = new CancellationTokenSource();
        var read1SyncEvent = new ManualResetEvent(false);
        var read2SyncEvent = new ManualResetEvent(false);
        var writeSyncEvent = new ManualResetEvent(false);

        var t1 = new Thread(() =>
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var t = _repo.TransactionScope(async (transaction, cancellation) =>
            {
                // read
                var album = await _repo.GetAsync(1, transaction, cancellation);
                Console.WriteLine($"[{threadId}] {album}");

                read1SyncEvent.Set();
                read2SyncEvent.WaitOne();

                // update
                album!.Price += 0.01m;
                var updated = await _repo.UpdateAsync(album, transaction, cancellation);
                Console.WriteLine($"[{threadId}] {updated} Album updated");

                writeSyncEvent.Set();

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
                // read
                read1SyncEvent.WaitOne();

                var album = (await _repo.GetAsync(1, transaction, cancellation))!;
                Console.WriteLine($"[{threadId}] {album}");

                read2SyncEvent.Set();

                // repetable read
                writeSyncEvent.WaitOne();
                var repetableAlbum = await _repo.GetAsync(1, transaction, cancellation);
                Console.WriteLine(repetableAlbum);
            }, IsolationLevel.RepeatableRead, cts.Token);

            t.GetAwaiter().GetResult();
        });

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();
    }
}
