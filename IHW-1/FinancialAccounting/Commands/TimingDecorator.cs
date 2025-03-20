using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class TimingDecorator : ICommand
{
    private readonly ICommand _inner;

    public TimingDecorator(ICommand command) => _inner = command;

    public async Task ExecuteAsync()
    {
        var sw = Stopwatch.StartNew();
        await _inner.ExecuteAsync();
        sw.Stop();
        Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds} ms");
    }
}
