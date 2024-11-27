namespace Spindle.Plugins;

public interface ISpindlePlugin
{
    UniTask StartAsync(CancellationToken token);
    UniTask EndAsync(CancellationToken token);
}