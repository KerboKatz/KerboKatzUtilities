using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KerboKatz.ConfigNodeExtension
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
