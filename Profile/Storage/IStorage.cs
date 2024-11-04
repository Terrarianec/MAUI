namespace Profile.Storage
{
  public  interface IStorage
    {
        bool ContainsKey(string key);

        T Get<T>(string key, T defaultValue);

        void Set<T>(string key, T value);
    }
}
