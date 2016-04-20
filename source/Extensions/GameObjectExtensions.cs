using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerboKatz.Extensions
{
  public static class GameObjectExtensions
  {
    public static void SortChildrenByName(this GameObject gameObject)
    {
      gameObject.transform.SortChildrenByName();
    }
    public static void SortChildrenByName(this Transform transform)
    {
      transform.SortChildren(CompareNames);
    }
    public static void SortChildren(this GameObject transform, Comparison<Transform> comparison)
    {
      transform.gameObject.SortChildren(CompareNames);

    }
    public static void SortChildren(this Transform transform,Comparison<Transform> comparison)
    {
      var children = new List<Transform>(transform.childCount);
      for(var i = 0; i< transform.childCount; i++)
      {
        children.Add(transform.GetChild(i));
      }
      children.Sort(comparison);
      for (int i = 0; i < children.Count; i++)
        children[i].SetSiblingIndex(i);
    }
    private static int CompareNames(Transform lhs, Transform rhs)
    {
      return lhs.gameObject.name.CompareTo(rhs.gameObject.name);
    }
  }
}
