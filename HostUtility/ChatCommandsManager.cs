using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HostUtility.Commands;

namespace HostUtility;

public class ChatCommandsManager
{
    public static List<ChatCommand> Commands = new();

    public static void Initialize()
    {
        foreach (var type in Assembly.GetCallingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(ChatCommand)) && !x.IsAbstract))
        {
            Commands.Add(Activator.CreateInstance(type) as ChatCommand);
        }
    }
}