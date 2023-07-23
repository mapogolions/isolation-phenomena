using IsoLevelsAdoNet.Repos;

namespace IsoLevelsAdoNet;

public partial class ReadCommitted
{
    private readonly IAlbumRepository _repo;

    public ReadCommitted(IAlbumRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }
}
