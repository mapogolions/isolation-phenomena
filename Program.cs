using System.Data.SqlClient;
using System.Reflection;
using IsoLevelsAdoNet;
using IsoLevelsAdoNet.Repos;
using IsoLevelsAdoNet.Phenomena;
using Microsoft.Extensions.Configuration;
using System.Data;


// // ReadUncommmitted (Dirty Read)
// {
//     var sample = new ReadUncommitted(BuildRepository());
//     sample.DirtyRead();
// }

// // ReadCommmited (Non-Repeatable Read)
// {
//     var sample = new ReadCommitted(BuildRepository());
//     sample.PreventDirtyRead();
//     // sample.NonRepeatableRead();
// }

// // RepetableRead ()
// {
//     var sample = new RepeatableRead(BuildRepository());
//     // sample.DeadLock();
//     // sample.PreventNonRepetableRead();
//     sample.PhantomRead();
// }


// // Dirty Read
// {
//     var phenomen = new DirtyReadPhenomen(BuildRepository());
//     phenomen.Demo(); // reproduce the problem
//     // phenomen.Demo(System.Data.IsolationLevel.ReadCommitted); // how ReadCommited level solves the problem
// }

// // Non Repeatable Read
// {
//     var phenomen = new NonRepeatableReadPhenomen(BuildRepository());
//     phenomen.Demo(); // reproduce the problem
//     // phenomen.Demo(System.Data.IsolationLevel.RepeatableRead); // how RepeatableRead solves the problem
// }

// Phantom Read
{
    var phenomen = new PhantomReadPhenomen(BuildRepository());
    phenomen.Demo(IsolationLevel.RepeatableRead); // reporoduce `Phantom Read`
    // phenomen.Demo(IsolationLevel.Serializable);
}


IAlbumRepository BuildRepository()
{
    var configuration = BuildConfiguration();
    DbConnectionFactoryDelegate factory =
        () => new SqlConnection(configuration.GetConnectionString("IsoLevelsAdoNet"));
    return new AlbumRepository(factory);
}

IConfiguration BuildConfiguration()
{
    return new ConfigurationBuilder()
        .AddUserSecrets(Assembly.GetExecutingAssembly())
        .Build();
}
