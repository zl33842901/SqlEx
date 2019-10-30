using System;
using System.Data;
using System.Data.SqlClient;
using xLiAd.SqlEx.Core.Core.Dialect;
using xLiAd.SqlEx.Repository;

namespace xLiAd.SqlEx.RepositoryMsSql
{
    public class RepositoryMsSql<T> : RepositoryBase<T>, IRepository<T>
    {
        public RepositoryMsSql(string connectionString, RepoXmlProvider repoXmlProvider = null, SqlEx.Core.Core.SqlExExceptionHandler exceptionHandler = null, bool throws = true)
            : base(new SqlConnection(connectionString), repoXmlProvider, exceptionHandler, throws)
        {

        }
        public RepositoryMsSql(IDbConnection _con, RepoXmlProvider repoXmlProvider = null, SqlEx.Core.Core.SqlExExceptionHandler exceptionHandler = null, bool throws = true)
            : base(_con, repoXmlProvider, exceptionHandler, throws)
        {

        }
        public RepositoryMsSql(IDbTransaction _tran, RepoXmlProvider repoXmlProvider = null, SqlEx.Core.Core.SqlExExceptionHandler exceptionHandler = null, bool throws = true)
            : base(_tran.Connection, repoXmlProvider, exceptionHandler, throws, _tran)
        {

        }

        protected override ISqlDialect Dialect => new SqlServerDialect();

        /// <summary>
        /// 获取事务提供
        /// </summary>
        /// <returns></returns>
        public override TransactionProviderBase GetTransaction()
        {
            if (DbTransaction != null)
                throw new Exception("已有事务实例的仓储不允许执行此操作。");
            else
                return new TransactionProvider(con, this.RepoXmlProvider, ExceptionHandler, Throws);
        }
    }
}
