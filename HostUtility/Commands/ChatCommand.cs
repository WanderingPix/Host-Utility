using System;
using System.Collections.Generic;

namespace HostUtility.Commands;

public abstract class ChatCommand
{
    public abstract string Command { get; }
    public abstract string Description { get; }
    public abstract List<CommandArgument> Arguments { get; }
    public abstract void Execute(string[] args);

    public virtual void HandleExecute(string[] args, out bool success)
    {
        if (args.Length == 0 && Arguments.Count != 0)
        {
            HudManager.Instance.Chat.AddChatWarning(GetInfoText());
            success = true;
            return;
        }
        try
        {
            Execute(args);
        }
        catch (Exception e)
        {
            success = false;
            return;
        }
        success = true;
    }

    public virtual string GetInfoText()
    {
        return "Something went wrong, make sure you're using proper syntax for this command!";
    }
}
public class CommandArgument(CommandArgument.ArgumentType type, string name)
{
    public ArgumentType Type = type;
    public string Name = name.ToLower();
    public enum ArgumentType
    {
        String,
        Int,
        Bool,
        Player,
        Color
    }
}