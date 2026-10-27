using System;
using HostUtility.Components.UI;
using UnityEngine.Bindings;

namespace HostUtility.Commands.Arguments;

public abstract class CommandArgument<T> : CommandArgument
{
    public abstract string Serialize(T value);
    public abstract T Deserialize(string value);
}

public abstract class CommandArgument
{
    public string ArgumentName { get; set; }
    public abstract void CreateOptions(CommandArgumentsMenu menu);
}