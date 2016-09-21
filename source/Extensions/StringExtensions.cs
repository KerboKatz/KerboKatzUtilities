using System;

namespace KerboKatz.Extensions
{
  public static class StringExtensions
  {
    public static float ToFloat(this string String)
    {
      float f;
      if (float.TryParse(String, out f))
      {
        return f;
      }
      return 0;
    }

    public static int ToInt(this string String)
    {
      int f;
      if (int.TryParse(String, out f))
      {
        return f;
      }
      return 0;
    }

    #region http://stackoverflow.com/a/444818

    public static bool Contains(this string source, string toCheck, StringComparison comp = StringComparison.OrdinalIgnoreCase)
    {
      return source.IndexOf(toCheck, comp) >= 0;
    }

    #endregion http://stackoverflow.com/a/444818
    /*#region http://stackoverflow.com/a/6535516
    public static bool IsNullOrWhiteSpace(this string value)
    {
      if (value == null) return true;
      return string.IsNullOrEmpty(value.Trim());
    }
    #endregion http://stackoverflow.com/a/6535516*/
  }
}