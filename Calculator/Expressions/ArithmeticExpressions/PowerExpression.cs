namespace Calculator.Expressions.ArithmeticExpressions;

public sealed class PowerExpression(Expression operand1, Expression operand2) : ArithmeticExpression("^", operand1, operand2, 2)
{
	public override double Solve() => Math.Pow(Operand1.Solve(), Operand2.Solve());
}