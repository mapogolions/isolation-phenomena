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
            var t = _repo.TransactionScope(async (transaction, cancellation) =>
            {
                var albums1 = await _repo.GetAsync(cancellation);
                Console.WriteLine($"[{threadId}] {string.Join(", ", albums1.Select(x => x.Id))}");

                readSyncEvent.Set();
                writeAsyncEvent.WaitOne();

                var albums2 = await _repo.GetAsync(cancellation); // Phantom read
                Console.WriteLine($"[{threadId}] {string.Join(", ", albums2.Select(x => x.Id))}");
            }, IsolationLevel.RepeatableRead, cts.Token);

            t.GetAwaiter().GetResult();
        });

        var t2 = new Thread(() =>
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            var t = _repo.TransactionScope(async (transaction, cancellation) =>
            {
                readSyncEvent.WaitOne();

                var album = new Album { Title = "Test", Artist = "Test", Price = 0.03m };
                var added = await _repo.AddAsync(album, transaction, cancellation);
                Console.WriteLine($"[{threadId}] {added} Album added");
                await transaction.CommitAsync(cancellation);
                Console.WriteLine($"[{threadId}] transaction committed");

                writeAsyncEvent.Set(); // send signal
            }, IsolationLevel.RepeatableRead, cts.Token);

            t.GetAwaiter().GetResult();
        });

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();
    }
}