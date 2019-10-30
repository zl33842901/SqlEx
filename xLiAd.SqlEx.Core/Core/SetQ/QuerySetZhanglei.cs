
using xLiAd.SqlEx.Core.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using xLiAd.SqlEx.Core.Helper;

namespace xLiAd.SqlEx.Core.Core.SetQ
{
    /// <summary>
    /// 具有不同类型的查询类和结果类的查询器
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <typeparam name="TSource">查询类型</typeparam>
    public class QuerySet<TResult,TSource> : QuerySet<TResult>
    {
        protected readonly SqlProvider<TSource> SqlProviderSource;
        internal QuerySet(IDbConnection conn, SqlProvider<TResult> sqlProvider, Type tableType, LambdaExpression whereExpression, LambdaExpression selectExpression, int? topNum, List<(EOrderBy Key, LambdaExpression Value)> orderbyExpressionList, IDbTransaction dbTransaction, bool throws = true)
            : base(conn, sqlProvider, tableType, whereExpression,selectExpression,topNum,orderbyExpressionList, dbTransaction, throws) { }

        #region ToList
        public override async Task<List<TResult>> ToListAsync()
        {
            SqlProvider.FormatToListZhanglei(typeof(TSource), this.FieldAnyExpression);
            SetSql();
            try
            {
                var results = await QueryDatabaseAsync<TSource>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
                var l = results.ToList();
                return l.Select(((Expression<Func<TSource, TResult>>)SelectExpression).Compile()).ToList();
            }
            catch (Exception e)
            {
                CallEvent(SqlProvider.SqlString, SqlProvider.Params, e.Message);
                if (Throws)
                    throw new Exception($"{e.Message} sql:{SqlProvider.SqlString} params:{SqlProvider.Params}", e);
                else
                    return new List<TResult>();
            }
        }
        public override List<TResult> ToList()
        {
            SqlProvider.FormatToListZhanglei(typeof(TSource), this.FieldAnyExpression);
            SetSql();
            try
            {
                var results = QueryDatabase<TSource>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
                var l = results.ToList();
                return l.Select(((Expression<Func<TSource, TResult>>)SelectExpression).Compile()).ToList();
            }
            catch (Exception e)
            {
                CallEvent(SqlProvider.SqlString, SqlProvider.Params, e.Message);
                if (Throws)
                    throw new Exception($"{e.Message} sql:{SqlProvider.SqlString} params:{SqlProvider.Params}", e);
                else
                    return new List<TResult>();
            }
        }
        #endregion
        protected override Type GetSourceType()
        {
            return typeof(TSource);
        }
        #region ProcessPageList
        protected override List<TResult> ProcessPageList(IDataReader reader, PropertyInfo[] ps)
        {
            var Parser = TypeConvert.GetSerializer<TSource>(TypeMapper, reader);
            List<TSource> lrst = new List<TSource>();
            while (reader.Read())
            {
                object rst = Parser(reader);
                if (ps.Length > 0 && HasSerializer)
                {
                    foreach (var p in ps)
                    {
                        var col = reader.GetOrdinal($"{p.Name}{ResolveExpression.JsonColumnNameSuffix}");
                        var s = reader.GetString(col);
                        var pv = Deserializer(s, p.PropertyType);
                        p.SetValue(rst, pv);
                    }
                }
                lrst.Add((TSource)rst);
            }
            return lrst.Select(((Expression<Func<TSource, TResult>>)SelectExpression).Compile()).ToList();
        }
        #endregion
    }
}
