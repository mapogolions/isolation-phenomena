using System.Data;
using IsoLevelsAdoNet.Models;

namespace IsoLevelsAdoNet;

public partial class RepeatableRead
{
    public void PhantomRead()
    {
        using var cts = new CancellationTokenSource();
        var readSyncEvent = new ManualResetEvent(false);
        var writeAsyncEvent = new ManualResetEvent(false);

        var t1 = new Thread(() =>
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            _repo.TransactionScope(async (transaction, cancellation) =>
            {
                var total1 = await _repo.TotalCostAsync(transaction, cancellation);
                Console.WriteLine($"[{threadId}] Total Cost: {total1}");

                readSyncEvent.Set();
                writeAsyncEvent.WaitOne();

                var total2 = await _repo.TotalCostAsync(transaction, cancellation); // Phantom
                Console.WriteLine($"[{threadId}] Total Cost: {total2}");
            }, IsolationLevel.RepeatableRead, cts.Token).GetAwaiter().GetResult();
        });

        var t2 = new Thread(() =>
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            _repo.TransactionScope(async (transaction, cancellation) =>
            {
                readSyncEvent.WaitOne();

                var album = new Album { Title = "Phantom", Artist = "Phantom", Price = 200.01m };
                var added = await _repo.AddAsync(album, transaction, cancellation);
                Console.WriteLine($"[{threadId}] {added} Album added");

                await transaction.CommitAsync(cancellation);
                Console.WriteLine($"[{threadId}] transaction committed");

                writeAsyncEvent.Set(); // send signal
            }, IsolationLevel.RepeatableRead, cts.Token).GetAwaiter().GetResult();
        });

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();
    }
}
