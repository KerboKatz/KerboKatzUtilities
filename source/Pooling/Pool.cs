using System;
using System.Collections.Generic;

namespace KerboKatz
{
  public class Pool<T>
  {
    private Stack<T> _objects;
    private Func<T> _objectGenerator;
    private Action<T> _objectReset;

    public Func<T> Generator
    {
      set
      {
        _objectGenerator = value;
      }
    }

    public Action<T> Reseter
    {
      set
      {
        _objectReset = value;
      }
    }

    public Pool()
    {
      _objects = new Stack<T>();
    }

    public T GetObject()
    {
      if (_objects.Count > 0)
        return _objects.Pop();
      return _objectGenerator();
    }

    public void PutObject(T item)
    {
      _objectReset(item);
      _objects.Push(item);
    }
  }
}