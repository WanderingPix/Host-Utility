using System;
using System.Collections.Generic;
using HostUtility.Commands.Arguments;

namespace HostUtility.Commands;

public abstract class ChatCommand
{
    public abstract string Command { get; }
    public abstract string Description { get; }
    public virtual bool HostOnly => false;
    public virtual bool AllowMidGame => false;
    public abstract List<CommandArgument> Arguments { get; }
    public abstract void Execute(string[] args);

    public virtual void HandleExecute(string[] args, out bool success)
    {
        if (args.Length == 0 && Arguments.Count != 0)
        {
            HudManager.Instance.Notifier.AddDisconnectMessage(GetInfoText());
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