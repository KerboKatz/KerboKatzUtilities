using System;

namespace KerboKatzUtilities.WorkDispatcher
{
  public class Worker : WorkerBase
  {
    private Action currentWork;

    public override void Update()
    {
      if (!WorkController.run)
      {
        run = false;
        return;
      }
      while (WorkController.HasWork && WorkController.run)
      {
        currentWork = WorkController.GetWork();
        currentWork?.Invoke();
      }
    }
  }
}