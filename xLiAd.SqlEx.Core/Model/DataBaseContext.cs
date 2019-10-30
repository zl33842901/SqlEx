using xLiAd.SqlEx.Core.Core.Interfaces;
using xLiAd.SqlEx.Core.Core.SetC;
using xLiAd.SqlEx.Core.Core.SetQ;

namespace xLiAd.SqlEx.Core.Model
{
    public class DataBaseContext<T>
    {
        public QuerySet<T> QuerySet => (QuerySet<T>)Set;

        public CommandSet<T> CommandSet => (CommandSet<T>)Set;

        public ISet<T> Set { get; internal set; }

        internal EOperateType OperateType { get; set; }
    }
}
