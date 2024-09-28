using Calculator.Expressions;
using Calculator.Expressions.FunctionExpressions;
using System.Globalization;
using System.Reflection;

namespace Calculator;

public partial class MainPage : ContentPage
{
	private readonly IDispatcherTimer _holdTimer = Application.Current!.Dispatcher.CreateTimer();

	public MainPage()
	{
		InitializeComponent();

		_holdTimer.Interval = TimeSpan.FromSeconds(0.45);
		_holdTimer.IsRepeating = false;
		_holdTimer.Tick += OnHoldTimerLeft;
	}

	private void OnHoldTimerLeft(object? sender, EventArgs e)
	{
		previousValueField.BindingContext = null;
		currentValueField.Text = string.Empty;
	}

	private void OnPageLoaded(object sender, EventArgs e)
	{
		var columns = functionButtons.ColumnDefinitions.Count;
		var rows = functionButtons.RowDefinitions.Count;
		int i = rows * columns - 4;

		var methods = typeof(OneArgumentFunctionExpression).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
			.Where(m => !m.IsSpecialName);

		foreach (var method in methods)
		{
			var button = new Button { Text = $"{method.Name.ToLower()}({string.Join(", ", method.GetParameters().Select(p => p.Name))})" };
			Grid.SetRow(button, i / columns);
			Grid.SetColumn(button, columns - 1 - i % columns);
			button.Clicked += OnOneArgumentFunctionButtonClicked;

			i--;
			functionButtons.Children.Add(button);
		}

		methods = typeof(TwoArgumentsFunctionExpression).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
					.Where(m => !m.IsSpecialName);

		foreach (var method in methods)
		{
			var button = new Button { Text = $"{method.Name.ToLower()}({string.Join(", ", method.GetParameters().Select(p => p.Name))})", BindingContext = method };
			Grid.SetRow(button, i / columns);
			Grid.SetColumn(button, columns - 1 - i % columns);
			button.Clicked += OnTwoArgumentsFunctionExpressionButtonClicked;

			i--;
			functionButtons.Children.Add(button);
		}
	}

	private void OnOneArgumentFunctionButtonClicked(object? sender, EventArgs e)
	{
		if (sender is not Button button)
			return;

		if (currentValueField.SelectionLength == 0)
		{
			Insert(button.Text[..^2]);
			return;
		}

		int cursorPosition = currentValueField.CursorPosition;
		var selected = currentValueField.Text.Substring(cursorPosition, currentValueField.SelectionLength);
		var functionExpression = $"{button.Text.Replace("v", selected)}";
		currentValueField.Text = $"{currentValueField.Text[..cursorPosition]}{functionExpression}{currentValueField.Text[(cursorPosition + currentValueField.SelectionLength)..]}";
		currentValueField.CursorPosition = cursorPosition + functionExpression.Length;
	}

	private void OnTwoArgumentsFunctionExpressionButtonClicked(object? sender, EventArgs e)
	{
		if (sender is not Button button || button.BindingContext is not MethodInfo method)
			return;

		if (currentValueField.SelectionLength == 0)
		{
			Insert($"{method.Name.ToLower()}({string.Join(", ", method.GetParameters().Select(p => p.DefaultValue is double value ? value : double.NegativeZero))})");
			return;
		}

		int cursorPosition = currentValueField.CursorPosition;
		var selected = currentValueField.Text.Substring(cursorPosition, currentValueField.SelectionLength);
		var functionExpression = $"{button.Text.Replace("a", selected).Replace("b", string.Empty)}";
		currentValueField.Text = $"{currentValueField.Text[..cursorPosition]}{functionExpression}{currentValueField.Text[(cursorPosition + currentValueField.SelectionLength)..]}";
		currentValueField.CursorPosition = cursorPosition + functionExpression.Length - 1;
	}

	private void OnDigitButtonClicked(object sender, EventArgs e)
	{
		if (sender is not Button button || !double.TryParse(button.Text[..1], out var digit))
			return;

		Insert(digit.ToString());
	}

	private void OnOperatorButtonClicked(object sender, EventArgs e)
	{
		if (sender is not Button button)
			return;

		Insert(button.Text);
	}

	private void OnSolveButtonClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(currentValueField.Text))
			return;

		var openBrackets = currentValueField.Text.Count(c => c == '(');
		var closeBrackets = currentValueField.Text.Count(c => c == ')');

		if (openBrackets > closeBrackets)
			currentValueField.Text += new string(')', openBrackets - closeBrackets);

		try
		{
			var expression = Expression.Parse(currentValueField.Text);
			currentValueField.Text = expression.Solve().ToString(CultureInfo.InvariantCulture);
			previousValueField.BindingContext = expression;
		}
		catch (ArgumentOutOfRangeException ex) { DisplayAlert("Ошибка", $"{ex.Message}", "Жаль"); }
		catch (ArgumentException) { DisplayAlert("Ошибка", "Некорректное выражение", "Жаль"); }
		catch (OverflowException) { DisplayAlert("Ошибка", "Произошло переполнение", "Жаль"); }
		catch (InvalidOperationException ex) { DisplayAlert("Ошибка", $"'{ex.Message}' не является корректной функцией", "Жаль"); }
		catch (DivideByZeroException ex) { DisplayAlert("Ошибка", ex.Message, "Жаль"); }
		catch (Exception) { DisplayAlert("Ошибка", "Непредвиденная ошибка", "Жаль"); }
	}

	private void OnClearButtonClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(currentValueField.Text)
			|| currentValueField.CursorPosition == 0)
			return;

		if (previousValueField.BindingContext is Expression previousExpression
			&& previousExpression.Solve().ToString() == currentValueField.Text)
		{
			previousValueField.BindingContext = null;
			currentValueField.Text = string.Empty;

			return;
		}

		currentValueField.Text = currentValueField.Text.Remove(currentValueField.CursorPosition - 1, 1);
	}

	private void OnAbsButtonClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(currentValueField.Text))
			return;

		if (currentValueField.SelectionLength != 0)
		{
			int cursorPosition = currentValueField.CursorPosition;
			var selected = currentValueField.Text.Substring(cursorPosition, currentValueField.SelectionLength);
			currentValueField.Text = $"{currentValueField.Text[..cursorPosition]}|{selected}|{currentValueField.Text[(cursorPosition + currentValueField.SelectionLength)..]}";
		}
		else
			currentValueField.Text = $"|{currentValueField.Text}|";
	}

	private void OnClearButtonPressed(object sender, EventArgs e)
	{
		_holdTimer.Start();
	}

	private void OnClearButtonReleased(object sender, EventArgs e)
	{
		_holdTimer.Stop();
	}

	private void OnFunctionsButtonClicked(object sender, EventArgs e)
	{
		functionButtons.IsVisible = !functionButtons.IsVisible;
	}

	private void OnOpenBracketButtonClicked(object sender, EventArgs e)
	{
		if (currentValueField.SelectionLength <= 0)
			Insert("(");
		else
		{
			int cursorPosition = currentValueField.CursorPosition;
			var selected = currentValueField.Text.Substring(cursorPosition, currentValueField.SelectionLength);
			currentValueField.Text = $"{currentValueField.Text[..cursorPosition]}({selected}){currentValueField.Text[(cursorPosition + currentValueField.SelectionLength)..]}";
		}
	}

	private void OnCloseBracketButtonClicked(object sender, EventArgs e)
	{
		Insert(")");
	}

	private void Insert(string value)
	{
		var cursorPosition = currentValueField.CursorPosition;

		currentValueField.Text = (currentValueField.Text ?? string.Empty).Insert(currentValueField.CursorPosition, value);

		currentValueField.CursorPosition = cursorPosition + value.Length;
	}
}
