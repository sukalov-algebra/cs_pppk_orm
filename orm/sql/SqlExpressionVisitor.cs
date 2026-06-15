using System;
using System.Linq.Expressions;
using System.Text;

namespace orm.sql
{
    public class SqlExpressionVisitor : ExpressionVisitor
    {
        private readonly StringBuilder _sb = new();

        public string Translate(Expression exp)
        {
            Visit(exp);
            return _sb.ToString();
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Visit(node.Left);

            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    _sb.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    _sb.Append(" <> ");
                    break;
                case ExpressionType.GreaterThan:
                    _sb.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _sb.Append(" >= ");
                    break;
                case ExpressionType.LessThan:
                    _sb.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    _sb.Append(" <= ");
                    break;
                case ExpressionType.AndAlso:
                    _sb.Append(" AND ");
                    break;
                case ExpressionType.OrElse:
                    _sb.Append(" OR ");
                    break;
                default:
                    throw new NotSupportedException($"Unsupported node type: {node.NodeType}");
            }

            Visit(node.Right);
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (IsColumn(node))
            {
                _sb.Append('\"');
                _sb.Append(node.Member.Name);
                _sb.Append('\"');
                return node;
            }

            _sb.Append(SqlFormatHelper.GetSqlValue(Evaluate(node)));
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _sb.Append(SqlFormatHelper.GetSqlValue(node.Value));
            return node;
        }

        private static bool IsColumn(Expression? node) => node switch
        {
            ParameterExpression => true,
            MemberExpression m => IsColumn(m.Expression),
            _ => false
        };

        private static object? Evaluate(Expression node) =>
            Expression.Lambda(node).Compile().DynamicInvoke();
    }
}
