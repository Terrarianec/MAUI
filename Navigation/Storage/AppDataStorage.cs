namespace Navigation.Storage
{
	public class AppDataStorage(string directory) : IStorage
	{
		public bool ContainsKey(string key)
		{
			return File.Exists(Path.Join(directory, key));
		}

		public void Set<T>(string key, T value)
		{
			var methods = GetType().GetMethods().Where(m => !m.IsGenericMethod && m.Name == nameof(Set));

			var method = methods.FirstOrDefault(m => m.GetParameters().Any(p => p.ParameterType == typeof(T))) ?? throw new NotImplementedException();

			method.Invoke(this, [key, value]);
		}

		public void Set(string key, string value) => Write(key, value);

		public void Set(string key, int value) => Write(key, value.ToString());

		public void Set(string key, long value) => Write(key, value.ToString());

		public void Set(string key, bool value) => Write(key, value.ToString());

		public void Set(string key, DateTime value)
		{
			Set(key, value.ToBinary());
		}

		public T Get<T>(string key, T defaultValue)
		{
			var methods = GetType().GetMethods().Where(m => !m.IsGenericMethod && m.Name == nameof(Get));

			var method = methods.FirstOrDefault(m => m.GetParameters().Any(p => p.ParameterType == typeof(T))) ?? throw new NotImplementedException();

			return (T)method.Invoke(this, [key, defaultValue])!;
		}

		public string Get(string key, string defaultValue)
		{
			return Read(key) ?? defaultValue;
		}

		public int Get(string key, int defaultValue)
		{
			var stored = Read(key);

			if (stored == null || !int.TryParse(stored, out var value))
				return defaultValue;

			return value;
		}

		public long Get(string key, long defaultValue)
		{
			var stored = Read(key);

			if (stored == null || !long.TryParse(stored, out var value))
				return defaultValue;

			return value;
		}

		public bool Get(string key, bool defaultValue)
		{
			var stored = Read(key);

			if (stored == null || !bool.TryParse(stored, out var value))
				return defaultValue;

			return value;

		}

		public DateTime Get(string key, DateTime defaultValue)
		{
			var stored = Read(key);

			if (stored == null || !long.TryParse(stored, out long value))
				return defaultValue;
			try
			{
				return DateTime.FromBinary(value);
			}
			catch
			{
				return defaultValue;
			}
		}

		private void Write(string filename, string data)
		{
			using var stream = new StreamWriter(Path.Join(directory, filename), false);
			stream.Write(data);
		}

		private string? Read(string filename)
		{
			if (!File.Exists(Path.Join(directory, filename)))
				return null;

			using var stream = new StreamReader(Path.Join(directory, filename));

			return stream.ReadToEnd();
		}
	}
}
