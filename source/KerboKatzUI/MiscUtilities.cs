namespace KerboKatz.UI
{
  public static class MiscUtilities
  {
    public static bool IsNullOrWhiteSpace(this string value)
    {
      if (value == null) return true;
      return string.IsNullOrEmpty(value.Trim());
    }
  }
}