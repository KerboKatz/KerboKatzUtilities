using System;
using System.Collections.Generic;
using System.IO;

namespace KerboKatz
{
  public class settings
  {
    private ConfigNode settingsContainer;
    private Dictionary<string, string> currentSettings = new Dictionary<string, string>();
    private string settingsPath;
    private string settingsFile;
    private string settingsName;

    public void load(string path, string file, string name)
    {
      settingsPath = KSPUtil.ApplicationRootPath + "GameData/KerboKatz/" + path + "/PluginData/";
      settingsFile = settingsPath + file + ".cfg";
      settingsName = name;
      settingsContainer = ConfigNode.Load(settingsFile);
      if (settingsContainer != null)
      {//if there was a settings file load the values from the file into the dictionary
        loadSettings();
      }
      settingsContainer = new ConfigNode(settingsName);
    }

    public bool isSet(string key)
    {
      return currentSettings.ContainsKey(key);
    }

    public void setDefault(string key, string value)
    {
      if (!isSet(key))
      {
        add(key, value);
      }
    }

    #region add, modify and get string
    /**
     * methods to add modify and get string values
     */
    public void add(string index, string value)
    {
      currentSettings.Add(index, value);
    }

    public void set(string index, string value)
    {
      if (currentSettings.ContainsKey(index))
      {
        currentSettings[index] = value;
      }
      else
      {
        add(index, value);
      }
    }

    public string getString(string index)
    {
      string result;
      if (currentSettings.TryGetValue(index, out result))
      {
        return result;
      }
      else
      {
        return "";
      }
    }

    public string getStringNumbers(string index)
    {
      return Utilities.getOnlyNumbers(getString(index));
    }

    #endregion add, modify and get string
    #region add, modify and get double
    /**
     * methods to add modify and get string double
     */
    public void add(string index, double value)
    {
      currentSettings.Add(index, value.ToString());
    }

    public void set(string index, double value)
    {
      if (currentSettings.ContainsKey(index))
      {
        currentSettings[index] = value.ToString();
      }
      else
      {
        add(index, value);
      }
    }

    public double getDouble(string index)
    {
      double result;
      string resultString;
      if (currentSettings.TryGetValue(index, out resultString) && double.TryParse(resultString, out result))
      {
        return result;
      }
      else
      {
        return 0;
      }
    }

    #endregion add, modify and get double
    #region add, modify and get float
    /**
     * methods to add modify and get string double
     */
    public void add(string index, float value)
    {
      currentSettings.Add(index, value.ToString());
    }

    public void set(string index, float value)
    {
      if (currentSettings.ContainsKey(index))
      {
        currentSettings[index] = value.ToString();
      }
      else
      {
        add(index, value);
      }
    }

    public float getFloat(string index)
    {
      float result;
      string resultString;
      if (currentSettings.TryGetValue(index, out resultString) && float.TryParse(resultString, out result))
      {
        return result;
      }
      else
      {
        return 0;
      }
    }

    #endregion add, modify and get float
    #region add, modify and get int
    /**
     * methods to add modify and get string double
     */
    public void add(string index, int value)
    {
      currentSettings.Add(index, value.ToString());
    }

    public void set(string index, int value)
    {
      if (currentSettings.ContainsKey(index))
      {
        currentSettings[index] = value.ToString();
      }
      else
      {
        add(index, value);
      }
    }

    public int getInt(string index)
    {
      int result;
      string resultString;
      if (currentSettings.TryGetValue(index, out resultString) && int.TryParse(resultString, out result))
      {
        return result;
      }
      else
      {
        return 0;
      }
    }

    #endregion add, modify and get int
    #region add, modify and get bool
    /**
     * methods to add modify and get bool values
     */
    public void add(string index, bool value)
    {
      currentSettings.Add(index, value.ToString());
    }

    public void set(string index, bool value)
    {
      if (currentSettings.ContainsKey(index))
      {
        currentSettings[index] = value.ToString();
      }
      else
      {
        add(index, value);
      }
    }

    public bool getBool(string index)
    {
      bool result;
      string resultString;
      if (currentSettings.TryGetValue(index, out resultString) && bool.TryParse(resultString, out result))
      {
        return result;
      }
      else
      {
        return false;
      }
    }

    #endregion add, modify and get bool

    /**
     *
     **/
    public void save()
    {
      ConfigNode savenode = new ConfigNode();
      foreach (KeyValuePair<string, string> entry in currentSettings)
      {
        if (settingsContainer.HasValue(entry.Key))
        {
          settingsContainer.SetValue(entry.Key, entry.Value);
        }
        else
        {
          settingsContainer.AddValue(entry.Key, entry.Value);
        }
      }
      savenode.AddNode(settingsContainer);
      Directory.CreateDirectory(settingsPath);//create the directory if it doesnt exist
      savenode.Save(settingsFile);//save the file \o/
    }

    private void loadSettings()
    {
      try
      {
        foreach (ConfigNode node in settingsContainer.GetNodes())
        {
          for (int i = 0; i < node.CountValues; i++)
          {
            add(node.values[i].name, node.values[i].value);
          }
        }
      }
      catch (Exception)
      {
      }
    }
  }
}