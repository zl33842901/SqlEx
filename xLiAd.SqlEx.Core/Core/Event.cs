using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xLiAd.SqlEx.Core.Helper;

namespace xLiAd.SqlEx.Core.Core
{
    public class SqlExEventArgs : EventArgs
    {
        public string Sql { get; }
        public Dictionary<string, object> Params { get; }
        public string ExtMessage { get; }
        public SqlExEventArgs(string sql, Dictionary<string, object> param, string extMessage) : base()
        {
            Sql = sql;
            Params = param;
            ExtMessage = extMessage;
        }
        public override string ToString()
        {
            return $"sql:{this.Sql} params:{this.Params.FormatString()} message:{this.ExtMessage}";
        }
    }

    public delegate void SqlExExceptionHandler(object sender, SqlExEventArgs e);
}
