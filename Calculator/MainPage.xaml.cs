﻿namespace Calculator
{
	public partial class MainPage : ContentPage
	{
		private static readonly List<Operator> _operators = [
			new Operator('÷', (a, b) => a / b),
			new Operator('×', (a, b) => a * b),
			new Operator('-', (a, b) => a - b),
			new Operator('+', (a, b) => a + b)
		];

		private int? KeepingValue
		{
			get
			{
				if (string.IsNullOrEmpty(keepingValueField.Text) || !int.TryParse(keepingValueField.Text, out var value))
					return null;

				return value;
			}
			set => keepingValueField.Text = value?.ToString();
		}

		private Operator? Operator
		{
			get
			{
				if (operation.BindingContext is not Operator op)
					return null;

				return op;
			}
			set
			{
				operation.BindingContext = value;
			}
		}

		private int? CurrentValue
		{
			get
			{
				if (string.IsNullOrEmpty(currentValueField.Text) || !int.TryParse(currentValueField.Text, out var value))
					return null;

				return value;
			}
			set => currentValueField.Text = value?.ToString();
		}

		public MainPage()
		{
			InitializeComponent();
		}

		private void OnPageLoaded(object sender, EventArgs e)
		{
			DisplayAlert("Предупреждение", "Этот калькулятор работает только с целыми числами в диапазоне [-2^31; 2^31 - 1]. За попытки работы с другими значениями автор не ручается", "Жаль");
		}

		private void OnDigitButtonClicked(object sender, EventArgs e)
		{
			if (sender is not Button button || !int.TryParse(button.Text[..1], out var digit))
				return;

			CurrentValue = (CurrentValue ?? 0) * 10 + digit;
		}

		private void OnOperatorButtonClicked(object sender, EventArgs e)
		{
			if (sender is not Button button)
				return;

			try
			{
				if (Operator != null)
				{
					CurrentValue = Operator.Execute(KeepingValue ?? 0, CurrentValue ?? 0);
					KeepingValue = null;
				}

				Operator = _operators.FirstOrDefault(op => op.Char.ToString() == button.Text);

				KeepingValue = CurrentValue;
				CurrentValue = null;
			}
			catch (DivideByZeroException) { DisplayAlert("Ошибка", "Деление на ноль", "Жаль"); }
			catch { DisplayAlert("Ошибка", "Неожиданно и неприятно", "Жаль"); }
		}

		private void OnSolveButtonClicked(object sender, EventArgs e)
		{
			if (Operator == null)
			{
				CurrentValue ??= KeepingValue;
				return;
			}

			try
			{
				CurrentValue = Operator.Execute(KeepingValue ?? 0, CurrentValue ?? 0);
				Operator = null;
				KeepingValue = null;
			}
			catch (DivideByZeroException) { DisplayAlert("Ошибка", "Деление на ноль", "Жаль"); }
			catch { DisplayAlert("Ошибка", "Неожиданно и неприятно", "Жаль"); }
		}

		private void OnClearButtonClicked(object sender, EventArgs e)
		{
			KeepingValue = null;
			Operator = null;
			CurrentValue = null;
		}
	}

}
