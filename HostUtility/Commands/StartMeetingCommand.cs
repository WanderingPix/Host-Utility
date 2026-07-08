using System.Collections.Generic;

namespace HostUtility.Commands;

public class StartMeetingCommand : ChatCommand
{
    public override string Command => "startmeeting";

    public override string Description => "Starts a meeting.";

    public override List<CommandArgument> Arguments => [];

    public override void Execute(string[] args)
    {
        if (MeetingHud.Instance == null) return;
        PlayerControl.LocalPlayer.RpcStartMeeting(null);
    }
}