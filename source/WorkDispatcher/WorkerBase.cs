using System;
using System.Threading;

namespace KerboKatzUtilities.WorkDispatcher
{
  public class WorkerBase
  {
    public int threadID;
    public string workerName;
    public bool run = true;
    public int threadWaitTimer = 1;

    public void Work()
    {
      //UnityEngine.Debug.Log("Starting " + workerName + " worker: " + threadID);
      try
      {
        while (run)
        {
          Update();
          WorkController.waitHandle.WaitOne(32);
          //block.
          //block.
          //Thread.Sleep(threadWaitTimer);
        }
      }
      catch (Exception e)
      {
        //UnityEngine.Debug.Log(workerName + " " + threadID + ": " + e.Message + "_" + e.StackTrace);
        Work();
      }
      //UnityEngine.Debug.Log("Stoping " + workerName + " worker: " + threadID);
    }

    public virtual void Update()
    {
    }
  }
}