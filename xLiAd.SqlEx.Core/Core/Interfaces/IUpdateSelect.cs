using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace xLiAd.SqlEx.Core.Core.Interfaces
{
    public interface IUpdateSelect<T>
    {
        List<T> UpdateSelect(Expression<Func<T, T>> where);
    }
}
