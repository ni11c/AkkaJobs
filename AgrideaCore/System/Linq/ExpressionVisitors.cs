

namespace System.Linq.Expressions
{
    public class ReplaceVisitor : ExpressionVisitor
    {
        private readonly Expression from_, to_;

        public ReplaceVisitor(Expression from, Expression to)
        {
            from_ = from;
            to_ = to;
        }

        public override Expression Visit(Expression node)
        {
            return node == from_ ? to_ : base.Visit(node);
        }
    }

    public static class ExpressionVisitorsHelper
    {
        public static Expression Replace(this Expression expression,
                                         Expression searchEx, Expression replaceEx)
        {
            return new ReplaceVisitor(searchEx, replaceEx).Visit(expression);
        }
    }
}
