
using System;
using System.Collections.Generic;
using System.Text;

namespace xLiAd.SqlEx.Core.Core.Interfaces
{
    public interface ISql
    {
        Dictionary<string, object> Params { get; }
        string SqlString { get; }
    }
}
