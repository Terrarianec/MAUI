namespace Calculator.Expressions;

public class NegativeExpression(Expression expression) : Expression
{
	protected Expression Expression = expression;

	public override string Beautify()
	{
		return Expression switch
		{
			ValueExpression or ArithmeticExpression or NegativeExpression => $"-({Expression})",
			_ => $"-{Expression}",
		};
	}

	public override double Solve() => -Expression.Solve();
}