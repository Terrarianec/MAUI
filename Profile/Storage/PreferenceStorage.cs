namespace Profile.Storage
{
  public  class PreferenceStorage(IPreferences preferences) : IStorage
    {
        public bool ContainsKey(string key) => preferences.ContainsKey(key);

        public T Get<T>(string key, T defaultValue) => preferences.Get(key, defaultValue);

        public void Set<T>(string key, T value) => preferences.Set(key, value);
    }
}
