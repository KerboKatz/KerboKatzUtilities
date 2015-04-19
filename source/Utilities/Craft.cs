using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace KerboKatz
{
  public static partial class Utilities
  {
    public static partial class Craft
    {
      public static string getPartAndStageString(int count, string p, bool zeroNumber = true)
      {
        if (count > 1)
        {
          return count + " " + p + "s";
        }
        else if (count == 1)
        {
          return "1 " + p;
        }
        else if (count == 0 && zeroNumber)
        {
          return "0 " + p + "s";
        }
        else
        {
          return "No " + p + "s";
        }
      }

      public static void getCraftCostAndStages(ConfigNode nodes, ConfigNode[] partList, out int craftStages, out float craftCost, out bool completeCraft, out string[] craftCategories)
      {
        craftCost = 0;
        craftStages = 0;
        completeCraft = true;
        craftCategories = nodes.GetValues("category");
        foreach (ConfigNode part in partList)
        {
          if (part.HasValue("istg"))
          {
            var partStage = toInt(part.GetValue("istg"));
            if (partStage > craftStages)
              craftStages = partStage;
          }
          float partCost = 0;
          if (getPartCost(part, out partCost))
          {
            craftCost += partCost;
          }
          else
          {
            completeCraft = false;
          }
        }
      }

      private static bool getPartCost(ConfigNode part, out float total, bool includeFuel = true)
      {
        string name = getPartName(part);
        float dryCost, fuelCost;
        total = 0;
        var aP = getAvailablePart(name);
        if (aP == null)
        {
          return false;
        }
        total = ShipConstruction.GetPartCosts(part, getAvailablePart(name), out dryCost, out fuelCost);
        if (!includeFuel)
          total = dryCost;
        return true;
      }

      private static string getPartName(ConfigNode part)
      {
        string name = part.GetValue("part");
        if (name != null)
          return name.Split('_')[0];
        else
          return part.GetValue("name");
      }

      private static Dictionary<string, AvailablePart> availablePartList;
      private static AvailablePart getAvailablePart(string partName)
      {
        if (availablePartList == null)
        {
          availablePartList = new Dictionary<string, AvailablePart>();
          foreach (AvailablePart part in PartLoader.LoadedPartsList)
          {
            if (!availablePartList.ContainsKey(part.name))
              availablePartList.Add(part.name, part);
          }
        }
        if (availablePartList.ContainsKey(partName))
        {
          return availablePartList[partName];
        }
        return null;
      }

      public static string[] getCraftCategories(string file)
      {
        return ConfigNode.Load(file).GetValues("category");
      }
    }
    #region deprecated
    [Obsolete("Use Utilities.Craft instead")]
    public static string[] getCraftCategories(string file)
    {
      return Craft.getCraftCategories(file);
    }
    [Obsolete("Use Utilities.Craft instead")]
    public static string getPartAndStageString(int count, string p, bool zeroNumber = true)
    {
      return Craft.getPartAndStageString(count, p, zeroNumber);
    }
    [Obsolete("Use Utilities.Craft instead")]
    public static void getCraftCostAndStages(ConfigNode nodes, ConfigNode[] partList, out int craftStages, out float craftCost, out bool completeCraft, out string[] craftCategories)
    {
      Craft.getCraftCostAndStages(nodes, partList, out craftStages, out craftCost, out completeCraft, out craftCategories);
    }
    #endregion
  }
}
