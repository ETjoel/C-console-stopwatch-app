using System;
using System.Threading;
using System.Threading.Tasks;

class Stopwatch
{
    private TimeSpan _timeElapsed;
    private bool _isRunning;
    private CancellationTokenSource _cancellationTokenSource;

    public delegate void StopwatchEventHandler(string message);
    public event StopwatchEventHandler OnStarted;
    public event StopwatchEventHandler OnStopped;
    public event StopwatchEventHandler OnReset;

    public Stopwatch()
    {
        _timeElapsed = TimeSpan.Zero;
        _isRunning = false;
    }

    public void Start()
    {
        if (_isRunning)
        {
            OnStarted?.Invoke("Stopwatch is already running.");
            return;
        }

        _isRunning = true;
        _cancellationTokenSource = new CancellationTokenSource();
        OnStarted?.Invoke("Stopwatch Started!");

        Task.Run(() => Tick(_cancellationTokenSource.Token));
    }

    public void Stop()
    {
        if (!_isRunning)
        {
            OnStopped?.Invoke("Stopwatch is not running.");
            return;
        }

        _isRunning = false;
        _cancellationTokenSource.Cancel();
        OnStopped?.Invoke("Stopwatch Stopped!");
    }

    public void Reset()
    {
        Stop();
        _timeElapsed = TimeSpan.Zero;
        OnReset?.Invoke("Stopwatch Reset!");
        Start();
    }

    private async Task Tick(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1000);
            if (_isRunning)
            {
                _timeElapsed = _timeElapsed.Add(TimeSpan.FromSeconds(1));
                Console.WriteLine($"Time Elapsed: {_timeElapsed}");
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var stopwatch = new Stopwatch();

        stopwatch.OnStarted += message => Console.WriteLine(message);
        stopwatch.OnStopped += message => Console.WriteLine(message);
        stopwatch.OnReset += message => Console.WriteLine(message);

        Console.WriteLine("Stopwatch Console Application");
        Console.WriteLine("Commands: S to Start, T to Stop, R to Reset, Q to Quit");

        bool exit = false;

        while (!exit)
        {
            Console.Write("Enter command: ");
            string input = Console.ReadLine().ToUpper();

            switch (input)
            {
                case "S":
                    stopwatch.Start();
                    break;

                case "T":
                    stopwatch.Stop();
                    break;

                case "R":
                    stopwatch.Reset();
                    break;

                case "Q":
                    exit = true;
                    stopwatch.Stop();
                    Console.WriteLine("Exiting Stopwatch Application. Goodbye!");
                    break;

                default:
                    Console.WriteLine("Invalid command. Please use S, T, R, or Q.");
                    break;
            }
        }
    }
}
