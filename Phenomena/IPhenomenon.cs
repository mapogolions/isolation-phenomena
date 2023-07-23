using System.Data;

namespace IsolationPhenomena.Phenomena;

public interface IPhenomenon
{
    void Demo(IsolationLevel iso);
}
