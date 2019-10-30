using System.Data;
using xLiAd.SqlEx.Core.Core.SetC;
using xLiAd.SqlEx.Core.Core.SetQ;

namespace xLiAd.SqlEx.Core.Core.Interfaces
{
    public interface IDatabase
    {
        QuerySet<T> QuerySet<T>();

        CommandSet<T> CommandSet<T>();

        IDbConnection GetConnection();
    }
}
