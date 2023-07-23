using System.Data.SqlClient;
using System.Reflection;
using IsoLevelsAdoNet;
using IsoLevelsAdoNet.Repos;
using IsoLevelsAdoNet.Phenomena;
using Microsoft.Extensions.Configuration;
using System.Data;


// Dirty Read
{
    var phenomenon = new DirtyReadPhenomenon(BuildRepository());
    phenomenon.Demo(IsolationLevel.ReadUncommitted); // reproduce the problem
    // phenomenon.Demo(IsolationLevel.ReadCommitted); // `ReadCommited` solves the problem
}

// // Non Repeatable Read
// {
//     var phenomenon = new NonRepeatableReadPhenomenon(BuildRepository());
//     phenomenon.Demo(IsolationLevel.ReadCommitted); // reproduce the problem
//     // phenomenon.Demo(IsolationLevel.RepeatableRead); // `RepeatableRead` solves the problem
// }

// // Phantom Read
// {
//     var phenomenon = new PhantomReadPhenomenon(BuildRepository());
//     phenomenon.Demo(IsolationLevel.RepeatableRead); // reporoduce `Phantom Read`
//     // phenomenon.Demo(IsolationLevel.Serializable); // `Serializable` solves the problem
// }


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
