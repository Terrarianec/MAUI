
namespace AuthorizeHere
{
	public partial class MainPage : ContentPage
	{
		private readonly Dictionary<string, string> users = new Dictionary<string, string>(comparer: StringComparer.InvariantCultureIgnoreCase)
		{
			{"Админ", "123" },
			{"Пользователь", "123" }
		};

		public MainPage()
		{
			InitializeComponent();
		}

		private async void OnPageLoaded(object sender, EventArgs e)
		{
			bool isCorrectUsername = false;
			string? answer = string.Empty;

			do
			{
				answer = (await DisplayPromptAsync("Авторизация", "Введите имя пользователя", accept: "Авторизоваться", cancel: "Выход", placeholder: "Я", initialValue: answer))?.Trim();

				if (answer is null)
				{
					Application.Current?.Quit();
					return;
				}

				isCorrectUsername = users.ContainsKey(answer);

				if (!isCorrectUsername)
					await DisplayAlert("Проблема", "Нет такого пользователя", "Жаль");
			} while (!isCorrectUsername);

			var password = users[answer];

			while (await DisplayPromptAsync("Авторизация", $"Введите пароль, {answer}", accept: "Войти", placeholder: "1234567890", keyboard: Keyboard.Numeric) != password)
				await DisplayAlert("Проблема", "Неверный пароль", "Жаль");

			MainArea.IsVisible = true;
		}

		private void OnButtonClicked(object sender, EventArgs e)
		{
			DisplayAlert(
			"Задание",
			"1. Создайте проект мобильного приложения.\n2. Создайте вертикальный дизайн приложения, состоящий из трех основных элементов, первый элемент – блок авторизация, второй элемент – блок элементов, размещенный согласно варианту задания, третий элемент – кнопка закрыть.\n3. Блок авторизации должен содержать четыре элемента – заголовок авторизация, заполненный фоновым цветом, элементы для ввода пользователя и пароля (Пользователи - Админ и Пользователь, пароль - 123), кнопка Авторизоваться.\n4. Блок элементов по заданию расположить на каком-либо фоне и без авторизации он не должен отображаться.",
			"Понятно");
		}

		private void OnSquakButtonClicked(object sender, EventArgs e)
		{
			DisplayAlert("Ква", "Ква", "Ква");
		}

		private async void OnQuitButtonClicked(object sender, EventArgs e)
		{
			var result = await DisplayAlert("Выход", "Вы точно хотите выйти?", "Да", "Нет");

			if (result == true)
				Application.Current?.Quit();
		}
	}

}
