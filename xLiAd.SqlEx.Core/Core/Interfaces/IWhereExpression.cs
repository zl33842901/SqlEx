
using System;
using System.Collections.Generic;
using System.Text;

namespace xLiAd.SqlEx.Core.Core.Interfaces
{
    public interface IWhereExpression
    {
        string SqlCmd { get; }
        Dictionary<string, object> Param { get; }
    }
}
