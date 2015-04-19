using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerboKatz.Classes
{
  public class alignedTooltip
  {
    public string name;
    public string value;
    public float nameSize;
    public float valueSize;
    public float totalSize;
    public alignedTooltip(string name, string value, float nameSize, float valueSize, float thisSize)
    {
      this.name = name;
      this.value = value;
      this.nameSize = nameSize;
      this.valueSize = valueSize;
      this.totalSize = thisSize;
    }
  }
}
