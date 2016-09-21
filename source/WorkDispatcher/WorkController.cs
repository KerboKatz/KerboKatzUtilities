using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace KerboKatzUtilities.WorkDispatcher
{
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public class WorkController : MonoBehaviour
  {
    public enum CPULoad
    {
      Single = 0,
      Low = 1,
      Medium = 2,
      High = 3
    }

    private static WorkController _instance;

    public static WorkController instance
    {
      get
      {
        if (_instance == null)
        {
          Debug.LogError("WorkController is null! This shouldn't have happend!");
          _instance = new WorkController();
        }
        return _instance;
      }
    }

    public static AutoResetEvent waitHandle = new AutoResetEvent(false);

    public static bool run = false;
    public CPULoad cpuLoad = CPULoad.High;
    internal Queue<Action> tasks = new Queue<Action>();

    public int threadCount;
    private Worker[] threads;

    public static bool HasWork
    {
      get
      {
        if (_instance != null && instance.tasks.Count > 0)
          return true;
        return false;
      }
    }

    private void Awake()
    {
      DontDestroyOnLoad(this);
      _instance = this;
      run = true;
      switch (cpuLoad)
      {
        case CPULoad.Single:
          threadCount = 1;
          break;

        case CPULoad.Low:
          threadCount = Math.Max(1, Environment.ProcessorCount / 2);
          break;

        case CPULoad.Medium:
          threadCount = Math.Max(1, Environment.ProcessorCount - 1);
          break;

        case CPULoad.High:
          threadCount = Environment.ProcessorCount;
          break;
      }
      StartThread();
    }

    internal static Action GetWork()
    {
      Action work;
      if (instance.TryDequeue(out work))
      {
        return work;
      }
      return null;
    }

    private bool TryDequeue(out Action work)
    {
      lock (tasks)
      {
        if (tasks.Count > 0)
        {
          work = tasks.Dequeue();
          return true;
        }
      }
      work = null;
      return false;
    }

    public static void AddWork(Action task)
    {
      lock (instance.tasks)
      {
        instance.tasks.Enqueue(task);
      }
      waitHandle.Set();
    }

    private void StartThread()
    {
      List<Worker> threadsList = new List<Worker>();
      for (var i = 0; i < threadCount; i++)
      {
        var threadClass = new Worker();
        threadClass.threadID = i + 1;
        var thread = new Thread(new ThreadStart(threadClass.Work));
        thread.Priority = System.Threading.ThreadPriority.Lowest;
        thread.IsBackground = true;
        thread.Start();
        threadsList.Add(threadClass);
      }
      threads = threadsList.ToArray();
    }

    private void OnDestroy()
    {
      run = false;
    }
  }
}