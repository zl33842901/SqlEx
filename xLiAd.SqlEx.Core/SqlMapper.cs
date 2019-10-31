using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data.Common;
using System.Threading;

namespace xLiAd.SqlEx.Core
{
    /// <summary>
    /// Extended connection
    /// </summary>
    public static class SqlMapper
    {
        /// <summary>
        /// Type mapper
        /// </summary>
        public static ITypeMapper TypeMapper = new TypeMapper();
        /// <summary>
        /// Does name matching ignore underscores
        /// </summary>
        public static bool MatchNamesWithUnderscores { get; set; }

        /// <summary>
        /// Executes a query, returning the data typed as T.
        /// </summary>
        public static IEnumerable<T> ExecuteQuery<T>(this IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var cmd = connection.CreateCommand())
            {
                Initialize(cmd, transaction, sql, param, commandTimeout, commandType);
                using (var reader = cmd.ExecuteReader())
                {
                    var handler = TypeConvert.GetSerializer<T>(TypeMapper, reader);
                    while (reader.Read())
                    {
                        yield return handler(reader);
                    }
                }
            }
        }
        ///// <summary>
        ///// Executes Multi query, returning the data typed as valueTuple.
        ///// </summary>
        //public static (IEnumerable<T1>, IEnumerable<T2>) ExecuteQuery<T1, T2>(this IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        //{
        //    using (var cmd = connection.CreateCommand())
        //    {
        //        Initialize(cmd, transaction, sql, param, commandTimeout, commandType);
        //        var item1 = new List<T1>();
        //        var item2 = new List<T2>();
        //        var count = 0;
        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            do
        //            {
        //                if (count == 0)
        //                {
        //                    var handler = TypeConvert.GetSerializer<T1>(TypeMapper, reader);
        //                    while (reader.Read())
        //                    {
        //                        item1.Add(handler(reader));
        //                    }
        //                }
        //                if (count == 1)
        //                {
        //                    var handler = TypeConvert.GetSerializer<T2>(TypeMapper, reader);
        //                    while (reader.Read())
        //                    {
        //                        item2.Add(handler(reader));
        //                    }
        //                }
        //                count++;
        //            } while (reader.NextResult());
        //            return (item1, item2);
        //        }
        //    }

        //}
        ///// <summary>
        ///// Execute parameterized SQL
        ///// </summary>
        //public static int ExecuteNonQuery(this IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        //{
        //    using (var cmd = connection.CreateCommand())
        //    {
        //        Initialize(cmd, transaction, sql, param, commandTimeout, commandType);
        //        return cmd.ExecuteNonQuery();
        //    }
        //}
        ///// <summary>
        ///// Execute parameterized SQL that selects a single value.
        ///// </summary>
        //public static T ExecuteScalar<T>(this IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        //{
        //    using (var cmd = connection.CreateCommand())
        //    {
        //        Initialize(cmd, transaction, sql, param, commandTimeout, commandType);
        //        var result = cmd.ExecuteScalar();
        //        if (result is DBNull)
        //        {
        //            return default;
        //        }
        //        return (T)Convert.ChangeType(result, typeof(T));
        //    }
        //}
        /// <summary>
        /// handler command
        /// </summary>      
        private static void Initialize(IDbCommand cmd, IDbTransaction transaction, string sql, object param, int? commandTimeout = null, CommandType? commandType = null)
        {
            var dbParameters = new List<IDbDataParameter>();
            cmd.Transaction = transaction;
            cmd.CommandText = sql;
            if (commandTimeout.HasValue)
            {
                cmd.CommandTimeout = commandTimeout.Value;
            }
            if (commandType.HasValue)
            {
                cmd.CommandType = commandType.Value;
            }
            if (param is IDbDataParameter)
            {
                dbParameters.Add(param as IDbDataParameter);
            }
            else if (param is IEnumerable<IDbDataParameter> parameters)
            {
                dbParameters.AddRange(parameters);
            }
            else if (param is Dictionary<string, object> keyValues)
            {
                foreach (var item in keyValues)
                {
                    var parameter = CreateParameter(cmd, item.Key, item.Value);
                    dbParameters.Add(parameter);
                }
            }
            else if (param != null)
            {
                var handler = TypeConvert.GetDeserializer(param.GetType());
                var values = handler(param);
                foreach (var item in values)
                {
                    var parameter = CreateParameter(cmd,item.Key,item.Value);
                    dbParameters.Add(parameter);
                }
            }
            if (dbParameters.Count > 0)
            {
                foreach (IDataParameter item in dbParameters)
                {
                    item.Value = item.Value ?? DBNull.Value;
                    var pattern = $@"in\s+([\@,\:,\?]?{item.ParameterName})";
                    var options = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline;
                    if (cmd.CommandText.IndexOf("in", StringComparison.OrdinalIgnoreCase) != -1 && Regex.IsMatch(cmd.CommandText, pattern, options))
                    {
                        var name = Regex.Match(cmd.CommandText, pattern, options).Groups[1].Value;
                        var list = new List<object>();
                        if (item.Value is IEnumerable<object> || item.Value is Array)
                        {
                            list = (item.Value as IEnumerable).Cast<object>().Where(a => a != null && a != DBNull.Value).ToList();
                        }
                        else
                        {
                            list.Add(item.Value);
                        }
                        if (list.Count() > 0)
                        {
                            cmd.CommandText = Regex.Replace(cmd.CommandText, name, $"({string.Join(",", list.Select(s => $"{name}{list.IndexOf(s)}"))})");
                            foreach (var iitem in list)
                            {
                                var key = $"{item.ParameterName}{list.IndexOf(iitem)}";
                                var parameter = CreateParameter(cmd, key, iitem);
                                cmd.Parameters.Add(parameter);
                            }
                        }
                        else
                        {
                            cmd.CommandText = Regex.Replace(cmd.CommandText, name, $"(SELECT 1 WHERE 1 = 0)");
                        }
                    }
                    else
                    {
                        cmd.Parameters.Add(item);
                    }
                }
            }
        }
        private static IDbDataParameter CreateParameter(IDbCommand command,string name,object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }
    }
}
