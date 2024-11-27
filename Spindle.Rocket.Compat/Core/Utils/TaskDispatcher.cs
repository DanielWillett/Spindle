using SDG.Framework.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Rocket.Core.Utils;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public class TaskDispatcher : MonoBehaviour
{
    private static readonly ConcurrentQueue<Action> InstantQueue = new ConcurrentQueue<Action>();
    private static readonly List<DelayedQueueItem> DelayedQueue = new List<DelayedQueueItem>();

    public static void QueueOnMainThread(Action action)
    {
        InstantQueue.Enqueue(action);
    }

    [Obsolete("Return value is not used and is always null. Use RunTask instead.")]
    public static Thread RunAsync(Action a)
    {
        RunTask(a);
        return null;
    }

    public static Task RunTask(Action a)
    {
        return Task.Run(() =>
        {
            try
            {
                a();
            }
            catch (Exception ex)
            {
                QueueOnMainThread(() => Logger.LogException(ex, "Error while running action"));
            }
        });
    }

    public static void QueueOnMainThread(Action action, float time)
    {
        if (time > 0f)
        {
            lock (DelayedQueue)
            {
                DelayedQueue.Add(new DelayedQueueItem
                {
                    time = Time.realtimeSinceStartup + time,
                    action = action
                });
            }
        }
        else if (Thread.CurrentThread.IsGameThread())
        {
            RunQueuedAction(action);
        }
        else
        {
            InstantQueue.Enqueue(action);
        }
    }

    private static void RunQueuedAction(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "Error while running action");
        }
    }

    public void Update()
    {
        while (InstantQueue.TryDequeue(out Action action))
        {
            RunQueuedAction(action);
        }

        // ReSharper disable once InconsistentlySynchronizedField
        if (DelayedQueue.Count == 0)
            return;

        // theoretically an action could modify the list
        List<Action> items = ListPool<Action>.claim();
        lock (DelayedQueue)
        {
            float rt = Time.realtimeSinceStartup;
            for (int i = DelayedQueue.Count - 1; i >= 0; --i)
            {
                DelayedQueueItem item = DelayedQueue[i];
                if (item.time > rt)
                    continue;

                DelayedQueue.RemoveAt(i);
                items.Add(item.action);
            }
        }

        foreach (Action action in items)
        {
            RunQueuedAction(action);
        }

        ListPool<Action>.release(items);
    }

    [TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
    public struct DelayedQueueItem
    {
        // ReSharper disable InconsistentNaming
        public float time;
        public Action action;
        // ReSharper restore InconsistentNaming
    }
}
