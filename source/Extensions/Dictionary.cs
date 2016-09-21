using System.Collections.Generic;

namespace KerboKatz.Extensions
{
  public static class Dictionary
  {
    public static V TryGetOrAdd<K, V>(this Dictionary<K, V> dictionary, K key) where V : class, new()
    {
      V value;
      dictionary.TryGetOrAdd(key, out value);
      return value;
    }

    public static bool TryGetOrAdd<K, V>(this Dictionary<K, V> dictionary, K key, out V value) where V : class, new()
    {
      if (!dictionary.TryGetValue(key, out value))
      {
        value = new V();
        dictionary.Add(key, value);
        return false;
      }
      return true;
    }
  }
}