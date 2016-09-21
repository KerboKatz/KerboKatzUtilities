using System.Collections.Generic;

namespace KerboKatz
{
  public class ListPool<T>
  {
    private static Pool<List<T>> pool;

    public static List<T> GetList()
    {
      CheckPoolExistence();
      return pool.GetObject();
    }

    public static void ReturnList(List<T> list)
    {
      CheckPoolExistence();
      pool.PutObject(list);
    }

    private static void CheckPoolExistence()
    {
      if (pool == null)
      {
        pool = new Pool<List<T>>();
        pool.Reseter = ResetList;
        pool.Generator = GenerateList;
      }
    }

    private static List<T> GenerateList()
    {
      return new List<T>();
    }

    private static void ResetList(List<T> obj)
    {
      obj.Clear();
    }
  }
}