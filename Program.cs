using System.Data.SqlClient;
using System.Reflection;
using IsoLevelsAdoNet;
using IsoLevelsAdoNet.Repos;
using IsoLevelsAdoNet.Phenomena;
using Microsoft.Extensions.Configuration;


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


// Dirty Read Phenomena
{
    var phenomen = new DirtyReadPhenomen(BuildRepository());
    phenomen.Demo(); // reproduce problem
    // phenomen.Demo(System.Data.IsolationLevel.ReadCommitted); // how ReadCommited level solves the problem
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
