namespace Directory_Scanner.Model.Threads;

public class TaskQueue
{
    private List<Thread> _threads;
    private Queue<Action?> _tasks;
    private int _waitingCount;
    public CancellationTokenSource cts;

    public TaskQueue(int threadCount)
    {
        cts = new CancellationTokenSource();
        _waitingCount = 0;
        _tasks = new Queue<Action?>();
        _threads = new List<Thread>();
        for (int i = 0; i < threadCount; i++)
        {
            var thread = new Thread(DoThreadWork);
            thread.IsBackground = true;
            _threads.Add(thread);
            thread.Start();
        }
    }

    public void EnqueueTask(Action? task)
    {
        lock (_tasks)
        {
            _tasks.Enqueue(task);
            Monitor.Pulse(_tasks);
        }
    }
    private Action? DequeueTask()
    {
        lock (_tasks)
        {
            while (_tasks.Count == 0 && !cts.Token.IsCancellationRequested)
            {
                _waitingCount++;
                Monitor.Wait(_tasks);
                _waitingCount--;
            }
            return _tasks.Dequeue();
        }
    }

    private void DoThreadWork()
    {
        while (!cts.Token.IsCancellationRequested)
        {
            Action? task = DequeueTask();
            if (task != null)
            {
                try
                {
                    task();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
                break;
        }
    }

    public void Close()
    {
        cts.Cancel();
        for (int i = 0; i < _waitingCount; i++)
            EnqueueTask(null);
        foreach (Thread t in _threads)
            t.Join();
        cts.Dispose();
    }


}
