﻿using System;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using xLiAd.SqlEx.Core.Core.Interfaces;

namespace xLiAd.SqlEx.Core.Core.SetQ
{
    /// <summary>
    /// 聚合查询器 带有的参数和 Order<T>一致
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Aggregation<T> : Order<T>, IAggregation
    {
        protected Aggregation(IDbConnection conn, SqlProvider<T> sqlProvider, IDbTransaction dbTransaction = null, bool throws = true) : base(conn, sqlProvider, dbTransaction, throws)
        {

        }
        #region Count
        public async Task<int> CountAsync()
        {
            SqlProvider.FormatCount();
            SetSql();
            return await DbCon.ExecuteScalarAsync<int>(SqlProvider.SqlString, SqlProvider.Params);
        }
        public int Count()
        {
            SqlProvider.FormatCount();
            SetSql();
            return DbCon.ExecuteScalar<int>(SqlProvider.SqlString, SqlProvider.Params);
        }
        #endregion
        #region Sum
        public async Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> sumExpression)
        {
            SqlProvider.FormatSum(sumExpression);
            SetSql();
            return await DbCon.ExecuteScalarAsync<TResult>(SqlProvider.SqlString, SqlProvider.Params);
        }
        public TResult Sum<TResult>(Expression<Func<T, TResult>> sumExpression)
        {
            SqlProvider.FormatSum(sumExpression);
            SetSql();
            return DbCon.ExecuteScalar<TResult>(SqlProvider.SqlString, SqlProvider.Params);
        }
        #endregion
        #region Exists
        public async Task<bool> ExistsAsync()
        {
            SqlProvider.FormatExists();
            SetSql();
            return await DbCon.ExecuteScalarAsync<int>(SqlProvider.SqlString, SqlProvider.Params) == 1;
        }
        public bool Exists()
        {
            SqlProvider.FormatExists();
            SetSql();
            return DbCon.ExecuteScalar<int>(SqlProvider.SqlString, SqlProvider.Params) == 1;
        }
        #endregion
    }
}
