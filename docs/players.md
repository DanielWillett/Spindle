# Spindle Player

Like RocketMod's `UnturnedPlayer` class, Spindle also has a wrapper.

@Spindle.Players.SpindlePlayer is a struct that can be created for wrapping any of the default player classes.

@Spindle.Players.SpindlePlayer.Create*?text=SpindlePlayer.Create overloads can be used to create the type from many different built-in types representing players.

For most actions you can do for a player, there is a method or property in this struct performing it.

# Player Components

Plugins can create classes implementing @Spindle.Players.IPlayerComponent which will be created for a player when they join.

It is not required that these be Unity components, but they will be handled correctly and added to the Player object if they are.

If a component implements @System.IDisposable, the component will be disposed when the player disconnects.

# [Basic Class](#tab/nocomponent)
```cs
using Spindle.Logging;
using Spindle.Players;

public sealed class PlayerLocationComponent : IPlayerComponent
{
    private readonly ILogger _logger;

    public SpindlePlayer Player { get; private set; }

    public PlayerLocationComponent(ILogger<PlayerLocationComponent> logger)
    {
        _logger = logger;
    }

    void IPlayerComponent.Initialize(SpindlePlayer player)
    {
        Player = player;
    }

    public void PrintLocation()
    {
        _logger.LogInformation("Location of {0}: {1} ({2}°).", Player, Player.Position, Player.Yaw);
    }
}
```

# [Unity Component](#tab/component)
```
using Spindle;
using Spindle.Logging;
using Spindle.Players;

public sealed class PlayerUnityComponent : MonoBehaviour, IPlayerComponent
{
    // dependency injection doesn't work with Unity components
    private ILogger _logger = null!;

    public SpindlePlayer Player { get; private set; }

    void IPlayerComponent.Initialize(SpindlePlayer player)
    {
        _logger = SpindleLauncher.LoggerFactory.CreateLogger<PlayerUnityComponent>();
        
        Player = player;
    }

    public void PrintLocation()
    {
        _logger.LogInformation("Location of {0}: {1} ({2}°).", Player, Player.Position, Player.Yaw);
    }
}
```
---

Usage:
```cs
SpindlePlayer player = SpindlePlayer.Create(/* ... */);

// SpindlePlayer.Component uses static generic types for very effecient type lookup.
// An error would be thrown if the component isn't found
player.Component<PlayerLocationComponent>().PrintLocation();
```

> [!NOTE]
> The player component system can be safely accessed from any thread.