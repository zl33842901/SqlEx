using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using xLiAd.SqlEx.Core.Core.Dialect;
using xLiAd.SqlEx.Core.Helper;

namespace xLiAd.SqlEx.Core.Core.Expression
{
    public class FieldAnyExpression<T, TField> : ExpressionVisitor, IFieldAnyExpression
    {
        readonly ISqlDialect Dialect;
        public Expression<Func<T, IList<TField>>> Field { get; }
        public Expression<Func<TField, bool>> Any { get; }
        public FieldAnyExpression(Expression<Func<T, IList<TField>>> field, Expression<Func<TField, bool>> any, ISqlDialect dialect)
        {
            this.Field = field;
            this.Any = any;
            this.Dialect = dialect;
            var w = new WhereExpression(any, "FieldAny_Param_", new PostgreSqlJsonColumnDialect(), false, true);
            this.WhereClause = w.SqlCmd;
            this.WhereParam = w.Param;
            Visit(field);
        }
        public string WhereClause { get; }
        public Dictionary<string, object> WhereParam { get; }
        public string ListFieldName { get; private set; }
        protected override System.Linq.Expressions.Expression VisitMember(MemberExpression node)
        {
            this.ListFieldName = node.Member.GetColumnAttributeName(Dialect);
            return node;
        }
    }


    public interface IFieldAnyExpression
    {
        string WhereClause { get; }
        Dictionary<string, object> WhereParam { get; }
        string ListFieldName { get; }
    }
}
