using IsoLevelsAdoNet.Repos;

namespace IsoLevelsAdoNet;

public partial class ReadUncommitted
{
    private readonly IAlbumRepository _repo;

    public ReadUncommitted(IAlbumRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }
}
