using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using xLiAd.SqlEx.Core.Core.Interfaces;
using xLiAd.SqlEx.Core.Helper;
using xLiAd.SqlEx.Core.Model;

namespace xLiAd.SqlEx.Core.Core.SetC
{
    public class CommandSet<T> : Command<T>, Interfaces.ISet<T>, ISql
    {
        internal Type TableType { get; set; }

        internal LambdaExpression WhereExpression { get; set; }

        internal LambdaExpression IfNotExistsExpression { get; set; }


        public CommandSet(IDbConnection conn, SqlProvider<T> sqlProvider, IDbTransaction dbTransaction = null, bool throws = true) : base(conn, sqlProvider, dbTransaction, throws)
        {
            TableType = typeof(T);
            SetContext = new DataBaseContext<T>
            {
                Set = this,
                OperateType = EOperateType.Command
            };

            sqlProvider.Context = SetContext;
        }

        internal CommandSet(IDbConnection conn, SqlProvider<T> sqlProvider, Type tableType, LambdaExpression whereExpression) : base(conn, sqlProvider)
        {
            TableType = tableType;
            WhereExpression = whereExpression;

            SetContext = new DataBaseContext<T>
            {
                Set = this,
                OperateType = EOperateType.Command
            };

            sqlProvider.Context = SetContext;
        }

        public ICommand<T> Where(Expression<Func<T, bool>> predicate)
        {
            WhereExpression = WhereExpression == null ? predicate : ((Expression<Func<T, bool>>)WhereExpression).And(predicate);

            return this;
        }

        public IInsert<T> IfNotExists(Expression<Func<T, bool>> predicate)
        {
            IfNotExistsExpression = IfNotExistsExpression == null ? predicate : ((Expression<Func<T, bool>>)IfNotExistsExpression).And(predicate);

            return this;
        }

        //public void BatchInsert(IEnumerable<T> entities, int timeout = 120)
        //{
        //    SqlHelper.BulkCopy(DbCon, entities);
        //}
    }
}
