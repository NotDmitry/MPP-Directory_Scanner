namespace Directory_Scanner.Model.Threads;

public class TaskQueue
{
    private List<Thread> _threads;
    private Queue<Action?> _tasks;
    public int WaitingCount { get; private set; }
    public CancellationTokenSource Cts { get; private set };

    public TaskQueue(int threadCount)
    {
        Cts = new CancellationTokenSource();
        WaitingCount = 0;
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
            while (_tasks.Count == 0 && !Cts.Token.IsCancellationRequested)
            {
                WaitingCount++;
                Monitor.Wait(_tasks);
                WaitingCount--;
            }
            return _tasks.Dequeue();
        }
    }

    private void DoThreadWork()
    {
        while (!Cts.Token.IsCancellationRequested)
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
        Cts.Cancel();
        for (int i = 0; i < WaitingCount; i++)
            EnqueueTask(null);
        foreach (Thread t in _threads)
            t.Join();
        Cts.Dispose();
    }

}
