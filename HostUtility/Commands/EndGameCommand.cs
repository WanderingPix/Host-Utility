using System.Collections.Generic;

namespace HostUtility.Commands;

public class EndGameCommand : ChatCommand
{
    public override string Command => "endgame";

    public override string Description => "Ends the game manually.";

    public override List<CommandArgument> Arguments => [];

    public override void Execute(string[] args)
    {
        if (MeetingHud.Instance == null) return;
        GameManager.Instance.RpcEndGame(GameOverReason.ImpostorDisconnect, false);
    }
}