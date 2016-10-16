namespace KerboKatz.Extensions
{
  public static partial class ConfigNodeExtension
  {
    public static float GetFloatValue(this ConfigNode cfgNode, string name)
    {
      float value;
      float.TryParse(cfgNode.GetValue(name), out value);
      return value;
    }
    public static int GetIntValue(this ConfigNode cfgNode, string name)
    {
      int value;
      int.TryParse(cfgNode.GetValue(name), out value);
      return value;
    }
    public static bool GetBoolValue(this ConfigNode cfgNode, string name)
    {
      bool value;
      bool.TryParse(cfgNode.GetValue(name), out value);
      return value;
    }
  }
}