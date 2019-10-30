using System.Collections.Generic;
using System.Threading.Tasks;
using xLiAd.SqlEx.Core.Model;

namespace xLiAd.SqlEx.Core.Core.Interfaces
{
    public interface IQuery<T>
    {
        T Get();
        Task<T> GetAsync();

        List<T> ToList();
        Task<List<T>> ToListAsync();

        PageList<T> PageList(int pageIndex, int pageSize);
        Task<PageList<T>> PageListAsync(int pageIndex, int pageSize);
    }
}
