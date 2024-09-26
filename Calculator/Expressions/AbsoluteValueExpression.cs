namespace Calculator.Expressions;

public class AbsoluteValueExpression(Expression value) : Expression
{
	public override string Beautify() => $"|{value}|";
	public override double Solve() => Math.Abs(value.Solve());
}