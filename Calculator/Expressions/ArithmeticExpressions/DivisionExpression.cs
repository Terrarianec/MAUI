namespace Calculator.Expressions.ArithmeticExpressions;

public sealed class DivisionExpression(Expression operand1, Expression operand2) : ArithmeticExpression("÷", operand1, operand2, 1)
{
	public override double Solve()
	{
		var operand1 = Operand1.Solve();
		var operand2 = Operand2.Solve();

		if (operand2 == 0)
			throw new DivideByZeroException($"{this}: неопределённость при делении на ноль");

		return operand1 / operand2;
	}
}