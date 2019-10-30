using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using xLiAd.SqlEx.Core.Model;
using xLiAd.SqlEx.Repository;

namespace xLiAd.SqlEx.RepositoryMysql
{
    public interface IRepositoryMysql<T> : IRepository<T>
    {
        Task<PageList<T>> PageListBySqlAsync(string sql, int pageIndex, int pageSize, Dictionary<string, string> dic = null);
        PageList<T> PageListBySql(string sql, int pageIndex, int pageSize, Dictionary<string, string> dic = null);
        Task<PageList<TResult>> PageListBySqlAsync<TResult>(string sql, int pageIndex, int pageSize, Dictionary<string, string> dic = null);
        PageList<TResult> PageListBySql<TResult>(string sql, int pageIndex, int pageSize, Dictionary<string, string> dic = null);
    }
}
