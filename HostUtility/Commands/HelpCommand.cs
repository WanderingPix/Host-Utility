using System.Collections.Generic;

namespace HostUtility.Commands;

public class HelpCommand : ChatCommand
{
    public override string Command => "help";

    public override string Description => "Opens this helpful menu!";

    public override List<CommandArgument> Arguments => [];

    public override void Execute(string[] args)
    {
        string allCommands = string.Empty;
        foreach (var command in ChatCommandsManager.Commands)
        {
            allCommands += $"<b>/{command.Command}</b> : {command.Description}.\n<size=70%>";
            foreach (var arg in command.Arguments)
            {
                allCommands += $"- {arg.Name} ({arg.Type.ToString()})\n";
            }
            allCommands += "</size>\n";
        }
        HudManager.Instance.Chat.AddChatWarning("<color=black>" + allCommands + "</color>");
    }
}