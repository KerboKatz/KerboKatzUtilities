using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace KerboKatz
{
  [Serializable]
  public class SettingsBase<T> where T : SettingsBase<T>, new()
  {
    public bool debug;
    public float uiFadeSpeed = 0.25f;
    public List<UIData> uiElements = new List<UIData>();

    private string settingsPath;
    private string settingsFile;

    public static T LoadSettings(string path, string file)
    {
      var settingsPath = KSPUtil.ApplicationRootPath + "GameData/KerboKatz/" + path + "/PluginData/";
      if (!Directory.Exists(settingsPath))
      {
        Directory.CreateDirectory(settingsPath);
      }
      var settingsFile = settingsPath + file + ".xml";
      var newSettings = Load(settingsFile);
      if (newSettings == null)
      {
        newSettings = new T();
      }
      newSettings.settingsFile = settingsFile;
      newSettings.settingsPath = settingsPath;
      newSettings.OnLoaded();
      return newSettings;
    }

    protected virtual void OnLoaded()
    {
    }

    public void Save()
    {
      if (!Directory.Exists(settingsPath))
      {
        Directory.CreateDirectory(settingsPath);
      }
      OnSave();
      var serializer = new XmlSerializer(typeof(T));
      using (var stream = new FileStream(settingsFile, FileMode.Create))
      {
        serializer.Serialize(stream, this);
      }
    }

    protected virtual void OnSave()
    {
    }

    private static T Load(string settingsFile)
    {
      if (!File.Exists(settingsFile))
        return null;
      var serializer = new XmlSerializer(typeof(T));
      using (var stream = new FileStream(settingsFile, FileMode.Open))
      {
        return serializer.Deserialize(stream) as T;
      }
    }
  }
}