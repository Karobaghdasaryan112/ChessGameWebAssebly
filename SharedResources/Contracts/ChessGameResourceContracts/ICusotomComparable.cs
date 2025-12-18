using SharedResources.ChessGameResource.Models;

namespace SharedResources.Contracts.ChessGameResourceContracts
{
    public interface ICusotomComparable
    {
        public List<Block> CompareTo(Board other);
    }
}
