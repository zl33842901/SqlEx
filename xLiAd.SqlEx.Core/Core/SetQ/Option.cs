﻿using System;
using System.Data;
using System.Linq.Expressions;

namespace xLiAd.SqlEx.Core.Core.SetQ
{
    /// <summary>
    /// 具有Select 参数和Top 参数的查询器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Option<T> : Query<T>
    {
        protected Option(IDbConnection conn, SqlProvider<T> sqlProvider, IDbTransaction dbTransaction = null, bool throws = true) : base(conn, sqlProvider, dbTransaction, throws)
        {

        }

        internal int? TopNum { get; set; }

        internal LambdaExpression SelectExpression { get; set; }

        //之前的这个Select 方法不灵活
        //public virtual Query<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        //{
        //    SelectExpression = selector;

        //    var thisObject = (QuerySet<T>)this;

        //    return new QuerySet<TResult>(DbCon, new SqlProvider<TResult>(), typeof(T), thisObject.WhereExpression, thisObject.SelectExpression, thisObject.TopNum, thisObject.OrderbyExpressionList, DbTransaction);
        //}
        public virtual Query<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            SelectExpression = selector;

            var thisObject = (QuerySet<T>)this;

            var rst = new QuerySet<TResult, T>(DbCon, new SqlProvider<TResult>(SqlProvider.Dialect), typeof(T), thisObject.WhereExpression, selector, thisObject.TopNum, thisObject.OrderbyExpressionList, DbTransaction, this.Throws);
            //考虑一下  这块要改成  return this; 可以避免很多问题。
            rst.ResetErrorHandler(thisObject);
            if (HasSerializer)
                rst.SetSerializeFunc(this.Serializer, this.Deserializer);
            rst.FieldAnyExpression = this.FieldAnyExpression;
            return rst;
        }

        public virtual Option<T> Top(int num)
        {
            TopNum = num;
            return this;
        }
    }
}
