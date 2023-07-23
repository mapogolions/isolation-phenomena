using System.Data;

namespace IsoLevelsAdoNet;

public partial class ReadCommitted
{
    public void PreventDirtyRead(IsolationLevel iso = IsolationLevel.ReadCommitted)
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
                Console.WriteLine($"[{threadId}] {updated} album updated");

                readSyncEvent.Set();
                await Task.Delay(TimeSpan.FromSeconds(3));

                await transaction.CommitAsync(cts.Token);
                Console.WriteLine($"[{threadId}] transaction committed");
            }, iso, cts.Token);

            t.GetAwaiter().GetResult();
        });

        var t2 = new Thread(() =>
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            var t = _repo.TransactionScope(async (transaction, cancellation) =>
            {
                readSyncEvent.WaitOne();
                var album = await _repo.GetAsync(1, transaction, cancellation);
                Console.WriteLine($"[{threadId}] {album}");
            }, iso, cts.Token);

            t.GetAwaiter().GetResult();
        });

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();
    }
}
