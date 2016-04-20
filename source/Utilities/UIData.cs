using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace KerboKatz
{
  [Serializable]
  public class UIData
  {
    [XmlIgnore]
    public GameObject gameObject;
    [XmlIgnore]
    public CanvasGroup canvasGroup;
    public string name;
    public Vector3 position;
    public bool active;
    [XmlIgnore]
    public GameObject prefab;
  }
}
