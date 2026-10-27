using System;
using System.Collections.Generic;
using System.Linq;
using HostUtility.Commands.Arguments;

namespace HostUtility.Commands;

public class EndMeetingCommand : ChatCommand
{
    public override string Command => "endmeeting";

    public override string Description => "Ends the current meeting.";

    public override List<CommandArgument> Arguments => [
        new BoolArgument("ShouldSkipEjection")
    ];

    public override void Execute(string[] args)
    {
        if (MeetingHud.Instance == null) return;
        var skipEject = bool.Parse(args[0]);
        if (skipEject)
        {
            MeetingHud.Instance.RpcClose();
            return;
        }
        bool tie;
        var max = MeetingHud.Instance.CalculateVotes().MaxPair(out tie);
        NetworkedPlayerInfo exiled = GameData.Instance.AllPlayers.ToArray().FirstOrDefault<NetworkedPlayerInfo>((Func<NetworkedPlayerInfo, bool>) (v => !tie && v.PlayerId == max.Key));
        bool wasOverruled = false;
        ushort overruleNonce = 0;
        JudgeOverrule winningOverrule;
        NetworkedPlayerInfo judgePlayerInfo;
        NetworkedPlayerInfo overruledPlayerInfo;
        if (MeetingHud.Instance.TryGetWinningOverrule(out winningOverrule, out judgePlayerInfo, out overruledPlayerInfo))
        {
            wasOverruled = true;
            overruleNonce = winningOverrule.OverruleNonce;
            exiled = overruledPlayerInfo.Role.TeamType != RoleTeamTypes.Impostor ? judgePlayerInfo : GameData.Instance.GetPlayerById((byte) winningOverrule.OverruledPlayerId);
        }
        MeetingHud.VoterState[] states = new MeetingHud.VoterState[MeetingHud.Instance.playerStates.Length];
        for (int index = 0; index < MeetingHud.Instance.playerStates.Length; ++index)
        {
            PlayerVoteArea playerState = MeetingHud.Instance.playerStates[index];
            states[index] = new MeetingHud.VoterState()
            {
                VoterId = (byte) playerState.PlayerId,
                VotedForId = (byte) playerState.VotedForId
            };
        }
        MeetingHud.Instance.RpcVotingComplete(states, exiled, tie, wasOverruled, overruleNonce);
    }
}