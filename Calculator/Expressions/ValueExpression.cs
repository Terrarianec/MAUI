namespace Calculator.Expressions;

public class ValueExpression(double value) : Expression
{
	protected double Value = value;

	public override string Beautify() => Value.ToString();
	public override double Solve() => Value;
}