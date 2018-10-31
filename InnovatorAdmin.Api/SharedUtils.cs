using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public static class SharedUtils
  {
    public static Task<IList<T>> TaskPool<T>(int maxConcurrent, params Func<Task<T>>[] taskFactory)
    {
      return TaskPool(maxConcurrent, null, taskFactory);
    }

    public static async Task<IList<T>> TaskPool<T>(int maxConcurrent, Action<int, string> progress, params Func<Task<T>>[] taskFactory)
    {
      var queue = new Queue<Func<Task<T>>>(Math.Max(taskFactory.Length - maxConcurrent, 0));
      var runningTasks = new List<Task<T>>(maxConcurrent);
      var results = new List<T>(taskFactory.Length);
      for (var i = 0; i < taskFactory.Length; i++)
      {
        if (i < maxConcurrent)
          runningTasks.Add(taskFactory[i].Invoke());
        else
          queue.Enqueue(taskFactory[i]);
      }

      while (runningTasks.Count > 0)
      {
        var task = await Task.WhenAny(runningTasks);
        runningTasks.Remove(task);
        results.Add(task.Result);
        progress?.Invoke(results.Count * 100 / taskFactory.Length, "");
        if (queue.Count > 0)
          runningTasks.Add(queue.Dequeue().Invoke());
      }

      return results;
    }
  }
}
