using System.Collections.Generic;

namespace HostUtility.Commands;

public class EndMeetingCommand : ChatCommand
{
    public override string Command => "endmeeting";

    public override string Description => "Ends the current meeting.";

    public override List<CommandArgument> Arguments => [];

    public override void Execute(string[] args)
    {
        if (MeetingHud.Instance == null) return;
        MeetingHud.Instance.RpcClose();
    }
}