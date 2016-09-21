using KerboKatz.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KerboKatz
{
  public class CraftDataEvent : UnityEvent<CraftData> { }

  public class CraftData
  {
    public string name;
    public bool completeCraft;
    public int partCount;
    public int stageCount;
    public float cost;

    //public HashSet<string> categories = new HashSet<string>();
    public DateTime lastSave;

    public bool isDone;
    public Texture2D thumbnail;
    public CraftDataEvent OnSaveRequest = new CraftDataEvent();
    public ConfigNode[] partNodes;

    public ConfigNode configNode;

    //public GameObject UIObject;
    public FileInfo fileInfo;

    public DirectoryInfo directoryInfo;

    private string _path;
    public InputField nameInputField;
    public GameObject gameObject;
    public UI.ContextMenu contextMenu;
    public ContextMenuOption contextMenuCopy;
    public bool thumbnailLoaded;

    public string path
    {
      get
      {
        return _path;
      }
      set
      {
        _path = Path.GetFullPath(value);
      }
    }

    /*public void RemoveCategory(string categoryName)
    {
      if (categories.Remove(categoryName))
      {
        OnSaveRequest.Invoke(this);
      }
    }
    public void RenameCategory(string oldName, string newName)
    {
      if (categories.Remove(oldName))
      {
        if (categories.Add(newName))
        {
          OnSaveRequest.Invoke(this);
        }
      }
    }

    public void ToggleCategory(string categoryName, bool isOn)
    {
      if (isOn)
      {
        if (categories.Add(categoryName))
        {
          OnSaveRequest.Invoke(this);
        }
      }
      else
      {
        if (categories.Remove(categoryName))
        {
          OnSaveRequest.Invoke(this);
        }
      }
    }*/

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append(name);
      sb.Append("_");
      sb.Append(lastSave.ToString());
      sb.Append("_");
      sb.Append(completeCraft);
      sb.Append("_");
      sb.Append(partCount);
      sb.Append("_");
      sb.Append(stageCount);
      sb.Append("_");
      sb.Append(cost);
      /*foreach (var category in categories)
      {
        sb.Append("_");
        sb.Append(category);
      }*/
      return sb.ToString();
    }
  }

  public static partial class Utilities
  {
    public static partial class Craft
    {
      public static string GetPartAndStageString(int count, string p, bool zeroNumber = true)
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

      public static CraftData GetCraftInfo(FileInfo file)
      {
        //FileInfo fileInfo = //new FileInfo(file);
        var path = file.FullName;
        var craftData = new CraftData();
        craftData.fileInfo = file;
        craftData.directoryInfo = file.Directory;
        craftData.configNode = ConfigNode.Load(path);
        craftData.partNodes = craftData.configNode.GetNodes("PART");
        craftData.path = path;
        craftData.partCount = craftData.partNodes.Length;
        craftData.cost = 0;
        craftData.stageCount = 0;
        craftData.completeCraft = true;
        //craftData.categories = new HashSet<string>(craftData.configNode.GetValues("category"));
        craftData.name = craftData.configNode.GetValue("ship");//gets the name
        craftData.lastSave = file.LastWriteTime;
        foreach (ConfigNode part in craftData.partNodes)
        {
          if (part.HasValue("istg"))
          {
            var partStage = toInt(part.GetValue("istg"));
            if (partStage > craftData.stageCount)
              craftData.stageCount = partStage;
          }
          float partCost = 0;
          if (!GetPartCost(part, out partCost))
          {
            craftData.completeCraft = false;
          }
          craftData.cost += partCost;
        }
        craftData.isDone = true;
        return craftData;
      }

      private static bool GetPartCost(ConfigNode part, out float total, bool includeFuel = true)
      {
        string name = GetPartName(part);
        float dryCost, fuelCost;
        total = 0;
        var aP = GetAvailablePart(name);
        if (aP == null)
        {
          return false;
        }
        total = ShipConstruction.GetPartCosts(part, aP, out dryCost, out fuelCost);
        if (!includeFuel)
          total = dryCost;
        return ResearchAndDevelopment.PartTechAvailable(aP);
      }

      private static string GetPartName(ConfigNode part)
      {
        string name = part.GetValue("part");
        if (name != null)
          return name.Split('_')[0];
        else
          return part.GetValue("name");
      }

      private static Dictionary<string, AvailablePart> availablePartList;

      private static AvailablePart GetAvailablePart(string partName)
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

      public static string[] GetCraftCategories(string file)
      {
        return ConfigNode.Load(file).GetValues("category");
      }
    }
  }
}