using System.Threading.Tasks;

namespace xLiAd.SqlEx.Core.Core.Interfaces
{
    public interface IInsert<T>
    {
        int Insert(T entity);
        Task<int> InsertAsync(T entity);
    }
}
