using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace xLiAd.SqlEx.Repository
{
    public interface IConnectionHolder
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        void BeginTransaction();
        void Commit();
        void Rollback();
    }

    public class ConnectionHolder : IConnectionHolder
    {
        public IDbConnection Connection { get; private set; }
        public IDbTransaction Transaction { get; private set; } = null;
        bool beenProcess = false;
        public ConnectionHolder(IDbConnection dbConnection)
        {
            this.Connection = dbConnection;
        }
        public void BeginTransaction()
        {
            if (Connection.State == ConnectionState.Closed)
                Connection.Open();
            Transaction = Connection.BeginTransaction();
        }
        /// <summary>
        /// 尝试提交事务并关闭数据库连接
        /// </summary>
        public void Commit()
        {
            if (!beenProcess)
            {
                try
                {
                    Transaction.Commit();
                }
                catch
                {
                    Transaction.Rollback();
                    throw;
                }
                finally
                {
                    Connection.Close();
                }
                beenProcess = true;
            }
        }
        /// <summary>
        /// 回滚事务  需要在catch 里显示调用，不然直到资源释放才能把表解锁。
        /// </summary>
        public void Rollback()
        {
            if (!beenProcess)
            {
                Transaction.Rollback();
                beenProcess = true;
                Connection.Close();
            }
        }
    }
}
