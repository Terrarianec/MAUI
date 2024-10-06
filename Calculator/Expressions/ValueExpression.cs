namespace Calculator.Expressions;

public class ValueExpression(double value = 0) : Expression
{
	protected double Value = value;

	public override string Beautify() => Value.ToString();
	public override double Solve() => Value;
}