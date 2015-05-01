namespace KerboKatz.Extensions
{
  public static partial class configNodeExtension
  {
    public static float getFloatValue(this ConfigNode cfgNode, string name)
    {
      float value;
      float.TryParse(cfgNode.GetValue(name), out value);
      return value;
    }
  }
}