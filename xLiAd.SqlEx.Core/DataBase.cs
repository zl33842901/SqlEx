﻿using System;
using System.Data;
using xLiAd.SqlEx.Core.Core.SetC;
using xLiAd.SqlEx.Core.Core.SetQ;

namespace xLiAd.SqlEx.Core
{
    public static class DataBase
    {

        public static void Transaction(this IDbConnection sqlConnection, Action<TransContext> action)
        {
            if (sqlConnection.State == ConnectionState.Closed)
                sqlConnection.Open();

            IDbTransaction transaction = sqlConnection.BeginTransaction();
            try
            {
                action(new TransContext { IDbTransaction = transaction, SqlConnection = sqlConnection });
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }

    public class TransContext
    {
        public IDbConnection SqlConnection { internal get; set; }

        public IDbTransaction IDbTransaction { internal get; set; }

        public QuerySet<T> QuerySet<T>()
        {
            return new QuerySet<T>(SqlConnection, new SqlProvider<T>(), IDbTransaction);
        }

        public CommandSet<T> CommandSet<T>()
        {
            return new CommandSet<T>(SqlConnection, new SqlProvider<T>(), IDbTransaction);
        }
        public CommandSet<T> CommandSet<T>(SqlProvider<T> sqlProvider, bool throws)
        {
            return new CommandSet<T>(SqlConnection, sqlProvider, IDbTransaction, throws);
        }
    }
}
