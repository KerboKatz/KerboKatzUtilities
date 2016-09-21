using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerboKatz.Extensions
{
  public static class GameObjectExtensions
  {
    public static void SortChildrenByName(this GameObject gameObject, bool ascending = true)
    {
      gameObject.transform.SortChildrenByName(ascending);
    }

    public static void SortChildrenByName(this Transform transform, bool ascending = true)
    {
      transform.SortChildren((x1, x2) =>
      {
        if (ascending)
          return CompareNames(x1, x2);
        else
          return CompareNames(x2, x1);
      });
    }

    public static void SortChildren(this GameObject GameObject, Comparison<Transform> comparison)
    {
      GameObject.transform.SortChildren(CompareNames);
    }

    public static void SortChildren(this Transform transform, Comparison<Transform> comparison)
    {
      var children = new List<Transform>(transform.childCount);
      for (var i = 0; i < transform.childCount; i++)
      {
        children.Add(transform.GetChild(i));
      }
      children.Sort(comparison);
      for (int i = 0; i < children.Count; i++)
        children[i].SetSiblingIndex(i);
    }

    private static int CompareNames(Transform lhs, Transform rhs)
    {
      //return lhs.gameObject.name.CompareTo(rhs.gameObject.name);
      return Utilities.Compare(lhs.gameObject.name, rhs.gameObject.name);
    }
  }
}