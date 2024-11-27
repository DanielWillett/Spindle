using Rocket.API;
using Rocket.Core.Serialization;
using Rocket.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Rocket.Core.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public class RocketCommandManager : MonoBehaviour
{
    public ReadOnlyCollection<RegisteredRocketCommand> Commands { get; internal set; }
    public event ExecuteCommand OnExecuteCommand;

    public IRocketCommand GetCommand(string command)
    {
        return Commands.FirstOrDefault(c => string.Equals(c.Name, command, StringComparison.InvariantCultureIgnoreCase));
    }

    public bool Register(IRocketCommand command)
    {
        Register(command, null);
        return true;
    }

    public void Register(IRocketCommand command, string alias)
    {
        Register(command, alias, CommandPriority.Normal);
    }

    public void Register(IRocketCommand command, string alias, CommandPriority priority)
    {

    }

    public void DeregisterFromAssembly(Assembly assembly)
    {

    }

    public double GetCooldown(IRocketPlayer player, IRocketCommand command)
    {
        return 0;
    }

    public void SetCooldown(IRocketPlayer player, IRocketCommand command)
    {

    }

    public bool Execute(IRocketPlayer player, string command)
    {
        command = command.TrimStart('/');
        string[] array1 = Regex.Matches(command, "[\\\"](.+?)[\\\"]|([^ ]+)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled).Cast<Match>().Select(m => m.Value.Trim('"').Trim()).ToArray();
        if (array1.Length != 0)
        {
            name = array1[0];
            string[] array2 = array1.Skip(1).ToArray();
            if (player == null)
                player = new ConsolePlayer();
            IRocketCommand command1 = GetCommand(name);
            double cooldown = GetCooldown(player, command1);
            if (command1 != null)
            {
                if (command1.AllowedCaller == AllowedCaller.Player && player is ConsolePlayer)
                {
                    Logger.Log("This command can't be called from console");
                    return false;
                }
                if (command1.AllowedCaller == AllowedCaller.Console && player is not ConsolePlayer)
                {
                    Logger.Log("This command can only be called from console");
                    return false;
                }
                if (cooldown != -1.0)
                {
                    Logger.Log("This command is still on cooldown");
                    return false;
                }
                try
                {
                    bool cancel = false;
                    if (OnExecuteCommand != null)
                    {
                        foreach (ExecuteCommand executeCommand in OnExecuteCommand.GetInvocationList().Cast<ExecuteCommand>())
                        {
                            try
                            {
                                executeCommand(player, command1, ref cancel);
                            }
                            catch (Exception ex)
                            {
                                Logger.LogException(ex);
                            }
                        }
                    }
                    if (!cancel)
                    {
                        try
                        {
                            command1.Execute(player, array2);
                            if (!player.HasPermission("*"))
                                SetCooldown(player, command1);
                        }
                        catch (NoPermissionsForCommandException ex)
                        {
                            Logger.LogWarning(ex.Message);
                        }
                        catch (WrongUsageOfCommandException ex)
                        {
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("An error occured while executing " + command1.Name + " [" + string.Join(", ", array2) + "]: " + ex.ToString());
                }
                return true;
            }
        }
        return false;
    }

    public void RegisterFromAssembly(Assembly assembly)
    {
        foreach (Type type in RocketHelper.GetTypesFromInterface(assembly, "IRocketCommand"))
        {
            if (type.GetConstructor(Type.EmptyTypes) != null)
            {
                IRocketCommand instance = (IRocketCommand)Activator.CreateInstance(type);
                Register(instance);
                foreach (string alias in instance.Aliases)
                    Register(instance, alias);
            }
        }

        Type typeFromAssembly = R.Plugins.GetMainTypeFromAssembly(assembly);
        if (typeFromAssembly == null)
            return;

        foreach (MethodInfo method in typeFromAssembly.GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            RocketCommandAttribute commands = (RocketCommandAttribute)Attribute.GetCustomAttribute(method, typeof(RocketCommandAttribute));
            RocketCommandAliasAttribute[] aliasAttributes = (RocketCommandAliasAttribute[])Attribute.GetCustomAttributes(method, typeof(RocketCommandAliasAttribute));
            RocketCommandPermissionAttribute[] permAttributes = (RocketCommandPermissionAttribute[])Attribute.GetCustomAttributes(method, typeof(RocketCommandPermissionAttribute));
            if (commands != null)
            {
                List<string> permissions = new List<string>();
                List<string> aliases = new List<string>();
                if (aliasAttributes != null)
                {
                    foreach (RocketCommandAliasAttribute commandAliasAttribute in aliasAttributes)
                        aliases.Add(commandAliasAttribute.Name);
                }
                if (permAttributes != null)
                {
                    foreach (RocketCommandPermissionAttribute permissionAttribute in permAttributes)
                        aliases.Add(permissionAttribute.Name);
                }
                //IRocketCommand command = new RocketAttributeCommand(commands.Name, commands.Help, commands.Syntax, commands.AllowedCaller, permissions, aliases, method);
                //Register(command);
                //foreach (string alias in command.Aliases)
                //    Register(command, alias);
            }
        }
    }

    [TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
    public delegate void ExecuteCommand(IRocketPlayer player, IRocketCommand command, ref bool cancel);

    [TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
    public class CommandMappingComparer : IEqualityComparer<CommandMapping>
    {
        public bool Equals(CommandMapping x, CommandMapping y)
        {
            return x.Name.ToLower() == y.Name.ToLower() && x.Class.ToLower() == y.Class.ToLower();
        }

        public int GetHashCode(CommandMapping obj)
        {
            return (obj.Name.ToLower() + obj.Class.ToLower()).GetHashCode();
        }
    }

    private static Type GetCommandType(IRocketCommand command)
    {
        Type t = command.GetType();
        return t.ReflectedType ?? t;
    }

    [TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
    public class RegisteredRocketCommand : IRocketCommand
    {
        public Type Type;
        public IRocketCommand Command;
        public List<string> Aliases => Command.Aliases;
        public AllowedCaller AllowedCaller => Command.AllowedCaller;
        public string Help => Command.Help;
        public string Name { get; }
        public List<string> Permissions => Command.Permissions;
        public string Syntax => Command.Syntax;

        public RegisteredRocketCommand(string name, IRocketCommand command)
        {
            Command = command;
            Name = name;
            Type = GetCommandType(command);
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Command.Execute(caller, command);
        }
    }
}