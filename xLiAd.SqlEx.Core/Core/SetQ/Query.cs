﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using xLiAd.SqlEx.Core.Core.Expression;
using xLiAd.SqlEx.Core.Core.Interfaces;
using xLiAd.SqlEx.Core.Helper;
using xLiAd.SqlEx.Core.Model;

namespace xLiAd.SqlEx.Core.Core.SetQ
{
    /// <summary>
    /// 查询器抽象基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Query<T> : IQuery<T>, IUpdateSelect<T>, ISql
    {
        public event SqlExExceptionHandler ErrorHappened;
        protected readonly SqlProvider<T> SqlProvider;
        protected readonly IDbConnection DbCon;
        protected readonly IDbTransaction DbTransaction;
        public static ITypeMapper TypeMapper = new TypeMapper();
        /// <summary>
        /// 是否抛出错误，如果不抛，可以用 event 实现日志功能
        /// </summary>
        public bool Throws { get; }
        /// <summary>
        /// 刚刚执行过的SQL语句（注：由于单例模式时会发生线程问题，本属性只作为调试用，不应该在程序里引用。）
        /// </summary>
        public string SqlString { get; private set; }
        /// <summary>
        /// 刚刚执行过的语句使用的参数（注：由于单例模式时会发生线程问题，本属性只作为调试用，不应该在程序里引用。）
        /// </summary>
        public Dictionary<string, object> Params { get; private set; }
        protected void SetSql()
        {
            if (SqlProvider.SqlString != null)
                this.SqlString = SqlProvider.SqlString;
            if (SqlProvider.Params != null)
                this.Params = SqlProvider.Params;
        }

        protected DataBaseContext<T> SetContext { get; set; }
        /// <summary>
        /// 是否 Distinct
        /// </summary>
        public bool IsDistinct { get; protected set; }
        public Query<T> Distinct()
        {
            this.IsDistinct = true;
            return this;
        }
        /// <summary>
        /// 新建一个查询器
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="sqlProvider">SQL转换器</param>
        /// <param name="dbTransaction">事务</param>
        /// <param name="throws">是否抛出错误</param>
        protected Query(IDbConnection conn, SqlProvider<T> sqlProvider, IDbTransaction dbTransaction = null, bool throws = true)
        {
            SqlProvider = sqlProvider;
            DbCon = conn;
            DbTransaction = dbTransaction;
            Throws = throws;
        }
        #region Get
        public async Task<T> GetAsync()
        {
            SqlProvider.FormatGet(this.FieldAnyExpression);
            SetSql();
            var result = await QueryFirstAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
            return result;
        }
        public T Get()
        {
            SqlProvider.FormatGet(this.FieldAnyExpression);
            SetSql();
            var result = QueryFirst(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
            return result;
        }
        #endregion
        #region Get
        public async Task<T> GetAsync<TKey>(TKey id)
        {
            SqlProvider.FormatGet(id, this.FieldAnyExpression);
            SetSql();
            var result = await QueryFirstAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
            return result;
        }

        public T Get<TKey>(TKey id)
        {
            SqlProvider.FormatGet(id, this.FieldAnyExpression);
            SetSql();
            var result = QueryFirst(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
            return result;
        }
        #endregion
        #region ToList
        public virtual async Task<List<T>> ToListAsync()
        {
            SqlProvider.FormatToList(null, this.FieldAnyExpression);
            SetSql();
            var results = await QueryWithExceptionAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
            return results.ToList();
        }
        public virtual List<T> ToList()
        {
            SqlProvider.FormatToList(null, this.FieldAnyExpression);
            SetSql();
            var results = QueryWithException(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
            return results.ToList();
        }
        #endregion
        #region ToList
        public virtual async Task<List<T>> ToListAsync(LambdaExpression[] selector)
        {
            SqlProvider.FormatToList(selector, this.FieldAnyExpression);
            SetSql();
            var results = await QueryWithExceptionAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
            return results.ToList();
        }
        public virtual List<T> ToList(LambdaExpression[] selector)
        {
            SqlProvider.FormatToList(selector, this.FieldAnyExpression);
            SetSql();
            var results = QueryWithException(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
            return results.ToList();
        }
        #endregion
        protected virtual List<T> ProcessPageList(IDataReader reader, PropertyInfo[] ps)
        {
            var Parser =  TypeConvert.GetSerializer<T>(TypeMapper, reader);
            List<T> lrst = new List<T>();
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
                lrst.Add((T)rst);
            }
            return lrst;
        }
        protected virtual Type GetSourceType() { return typeof(T); }
        
        #region PageList
        public async Task<PageList<T>> PageListAsync(int pageIndex, int pageSize)
        {
            SqlProvider.FormatToPageList(GetSourceType(), pageIndex, pageSize, this.FieldAnyExpression);
            SetSql();
            try {
                var ps = typeof(T).GetJsonColumnProperty();
                
                var Reader = await DbCon.ExecuteReaderAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
                var pageTotal = 0;
                if (Reader.Read())
                {
                    pageTotal = Reader.GetInt32(0);
                }
                Reader.NextResult();
                var lrst = ProcessPageList(Reader, ps);
                Reader.Close();
                return new PageList<T>(pageIndex, pageSize, pageTotal, lrst);
            }
            catch (Exception e)
            {
                CallEvent(SqlProvider.SqlString, SqlProvider.Params, e.Message);
                if (Throws)
                    throw new Exception($"{e.Message} sql:{SqlProvider.SqlString} params:{SqlProvider.Params}", e);
                else
                    return new PageList<T>(0, 0, 0, new List<T>());
            }
        }
        public PageList<T> PageList(int pageIndex, int pageSize)
        {
            SqlProvider.FormatToPageList(GetSourceType(), pageIndex, pageSize, this.FieldAnyExpression);
            SetSql();
            try
            {
                var ps = typeof(T).GetJsonColumnProperty();
                
                var Reader = DbCon.ExecuteReader(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
                var pageTotal = 0;
                if (Reader.Read())
                {
                    pageTotal = Reader.GetInt32(0);
                }
                Reader.NextResult();
                var lrst = ProcessPageList(Reader, ps);
                Reader.Close();
                return new PageList<T>(pageIndex, pageSize, pageTotal, lrst);
            }
            catch (Exception e)
            {
                CallEvent(SqlProvider.SqlString, SqlProvider.Params, e.Message);
                if (Throws)
                    throw new Exception($"{e.Message} sql:{SqlProvider.SqlString} params:{SqlProvider.Params}", e);
                else
                    return new PageList<T>(0, 0, 0, new List<T>());
            }
        }
        #endregion
        #region UpdateSelect
        public async Task<List<T>> UpdateSelectAsync(Expression<Func<T, T>> updator)
        {
            SqlProvider.FormatUpdateSelect(updator);
            SetSql();
            var lresult = await QueryWithExceptionAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
            return lresult.ToList();
        }
        public List<T> UpdateSelect(Expression<Func<T, T>> updator)
        {
            SqlProvider.FormatUpdateSelect(updator);
            SetSql();
            var lresult = QueryWithException(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
            return lresult.ToList();
        }
        #endregion
        #region QueryDatabase
        protected async Task<IEnumerable<TRst>> QueryDatabaseAsync<TRst>(string sqlString, Dictionary<string, object> param, IDbTransaction dbTransaction, int count = 0)
        {
            var ps = typeof(TRst).GetJsonColumnProperty();
            
            var Reader = await DbCon.ExecuteReaderAsync(sqlString, param, dbTransaction);
            var Parser = //Reader.GetRowParser(typeof(TRst));
            TypeConvert.GetSerializer<TRst>(TypeMapper, Reader);
            List<TRst> lrst = new List<TRst>();
            int i = 0;
            while (Reader.Read())
            {
                object rst = Parser(Reader);
                if (ps.Length > 0 && HasSerializer)
                {
                    foreach (var p in ps)
                    {
                        int col;
                        try
                        {
                            col = Reader.GetOrdinal($"{p.Name}{ResolveExpression.JsonColumnNameSuffix}");
                        }
                        catch
                        {
                            continue;
                        }
                        object o = Reader.GetValue(col);
                        if (o == DBNull.Value)
                            continue;
                        var s = Reader.GetString(col);
                        var pv = Deserializer(s, p.PropertyType);
                        p.SetValue(rst, pv);
                    }
                }
                lrst.Add((TRst)rst);
                if (++i >= count && count > 0)
                    break;
            }
            Reader.Close();
            return lrst;
        }
        protected IEnumerable<TRst> QueryDatabase<TRst>(string sqlString, Dictionary<string, object> param, IDbTransaction dbTransaction, int count = 0)
        {
            var ps = typeof(TRst).GetJsonColumnProperty();
            var Reader = DbCon.ExecuteReader(sqlString, param, dbTransaction);
            var Parser = //Reader.GetRowParser(typeof(TRst));
            TypeConvert.GetSerializer<TRst>(TypeMapper, Reader);
            List<TRst> lrst = new List<TRst>();
            int i = 0;
            while (Reader.Read())
            {
                object rst = Parser(Reader);
                if (ps.Length > 0 && HasSerializer)
                {
                    foreach (var p in ps)
                    {
                        int col;
                        try
                        {
                            col = Reader.GetOrdinal($"{p.Name}{ResolveExpression.JsonColumnNameSuffix}");
                        }
                        catch
                        {
                            continue;
                        }
                        object o = Reader.GetValue(col);
                        if (o == DBNull.Value)
                            continue;
                        var s = Reader.GetString(col);
                        var pv = Deserializer(s, p.PropertyType);
                        p.SetValue(rst, pv);
                    }
                }
                lrst.Add((TRst)rst);
                if (++i >= count && count > 0)
                    break;
            }
            Reader.Close();
            return lrst;
        }
        #endregion
        #region QueryWithException
        private async Task<IEnumerable<T>> QueryWithExceptionAsync(string sqlString, Dictionary<string, object> param, IDbTransaction dbTransaction)
        {
            try
            {
                return await QueryDatabaseAsync<T>(sqlString, param, dbTransaction);
            }
            catch (Exception e)
            {
                CallEvent(sqlString, param, e.Message);
                if (Throws)
                    throw new Exception($"{e.Message} sql:{sqlString} params:{param}", e);
                else
                    return new List<T>();
            }
        }
        private IEnumerable<T> QueryWithException(string sqlString, Dictionary<string, object> param, IDbTransaction dbTransaction)
        {
            try
            {
                return QueryDatabase<T>(sqlString, param, dbTransaction);
            }
            catch (Exception e)
            {
                CallEvent(sqlString, param, e.Message);
                if (Throws)
                    throw new Exception($"{e.Message} sql:{sqlString} params:{param}", e);
                else
                    return new List<T>();
            }
        }
        #endregion
        #region QueryFirst
        private async Task<T> QueryFirstAsync(string sqlString, Dictionary<string, object> param, IDbTransaction dbTransaction)
        {
            try
            {
                return (await QueryDatabaseAsync<T>(sqlString, param, dbTransaction, 1)).FirstOrDefault();
            }
            catch (Exception e)
            {
                CallEvent(sqlString, param, e.Message);
                if (Throws)
                    throw new Exception($"{e.Message} sql:{sqlString} params:{param}", e);
                else
                    return default(T);
            }
        }
        private T QueryFirst(string sqlString, Dictionary<string, object> param, IDbTransaction dbTransaction)
        {
            try
            {
                return QueryDatabase<T>(sqlString, param, dbTransaction, 1).FirstOrDefault();
            }
            catch (Exception e)
            {
                CallEvent(sqlString, param, e.Message);
                if (Throws)
                    throw new Exception($"{e.Message} sql:{sqlString} params:{param}", e);
                else
                    return default(T);
            }
        }
        #endregion
        protected void CallEvent(string sqlString, Dictionary<string, object> param, string message)
        {
            try
            {
                var args = new SqlExEventArgs(sqlString, param, message);
                ErrorHappened?.Invoke(this, args);
            }
            catch { }
        }
        internal void ResetErrorHandler<Told>(Query<Told> old)
        {
            this.ErrorHappened = old.ErrorHappened;
        }
        /// <summary>
        /// 是否有JSON序列化器
        /// </summary>
        public bool HasSerializer { get; private set; } = false;
        /// <summary>
        /// 序列化器
        /// </summary>
        public Func<object, string> Serializer { get; private set; }
        /// <summary>
        /// 反序列化器
        /// </summary>
        public Func<string, Type, object> Deserializer { get; private set; }
        /// <summary>
        /// 设置序列化和反序列化器
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="deserializer"></param>
        public void SetSerializeFunc(Func<object, string> serializer, Func<string, Type, object> deserializer)
        {
            if (serializer != null && deserializer != null)
            {
                this.Serializer = serializer;
                this.Deserializer = deserializer;
                HasSerializer = true;
            }
        }
        public IFieldAnyExpression FieldAnyExpression { get; set; }
    }
}
