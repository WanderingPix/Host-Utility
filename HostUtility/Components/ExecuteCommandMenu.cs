using System;
using System.Collections;
using System.Collections.Generic;
using HostUtility.Commands;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HostUtility.Components
{
    [RegisterInIl2Cpp]
    public class ExecuteCommandMenu(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public Il2CppReferenceField<Toggle> togglePrefab;
        public Il2CppReferenceField<TMP_Dropdown> dropdownPrefab;
        public Il2CppReferenceField<Transform> container;
        public Il2CppReferenceField<TextMeshProUGUI> text;
        public ChatCommand command;
        public List<string> parameters;
        public void Execute()
        {
            command.HandleExecute(parameters.ToArray(), out _);
            gameObject.Destroy();
        }

        private void SetUp(ChatCommand cmd)
        {
            command = cmd;
            text.Value.text = cmd.Command;
            parameters = new List<string>(cmd.Arguments.Count);
            for (int index = 0; index < cmd.Arguments.Count; index++)
            {
                var arg = cmd.Arguments[index];
                int capturedIndex = index;

                switch (arg.Type)
                {
                    case CommandArgument.ArgumentType.Bool:
                        var toggle = Object.Instantiate(togglePrefab.Value, container.Value);
                        toggle.onValueChanged.AddListener(new System.Action<bool>(value =>
                        {
                            parameters[capturedIndex] = value.ToString();
                        }));
                        parameters.Add("False");
                        break;
                    case CommandArgument.ArgumentType.Player:
                        var playerDropdown = Object.Instantiate(dropdownPrefab.Value, container.Value);
                        playerDropdown.ClearOptions();
                        var playerNames = new Il2CppSystem.Collections.Generic.List<string>();
                        foreach (var player in PlayerControl.AllPlayerControls) playerNames.Add(player.Data.PlayerName);
                        playerDropdown.AddOptions(playerNames);
                        playerDropdown.onValueChanged.AddListener(new System.Action<int>(value =>
                        {
                            parameters[capturedIndex] = PlayerControl.AllPlayerControls[value].Data.PlayerName;
                        }));
                        parameters.Add(PlayerControl.AllPlayerControls[0].Data.PlayerName);
                        break;
                    case CommandArgument.ArgumentType.Color:
                        var colorDropdown = Object.Instantiate(dropdownPrefab.Value, container.Value);
                        colorDropdown.ClearOptions();
                        var colorNames = new Il2CppSystem.Collections.Generic.List<string>();
                        foreach (var color in Palette.ColorNames) colorNames.Add(TranslationController.Instance.GetString(color));
                        colorDropdown.AddOptions(colorNames);
                        colorDropdown.onValueChanged.AddListener(new System.Action<int>(value =>
                        {
                            parameters[capturedIndex] = colorNames[value].ToLower();
                        }));
                        parameters.Add(colorNames[0].ToLower());
                        break;
                }
            }
        }
        private void OnDestroy()
        {
            ControllerManager.Instance.CloseOverlayMenu("ExecuteCommandMenu");
        }

        private void OnDisable()
        {
            gameObject.Destroy();
        }

        private void Start()
        {
            
            if (OperatingSystem.IsAndroid()) GetComponent<CanvasScaler>().scaleFactor *= 0.8f;
        }

        public static void CreateAndShow(ChatCommand command)
        {
            var menu = Object.Instantiate(HostUtilityPlugin.Bundle.LoadAsset<GameObject>("ExecuteCommandMenu"));
            menu.GetComponent<ExecuteCommandMenu>().SetUp(command);
            ControllerManager.Instance.OpenOverlayMenu("ExecuteCommandMenu", null);
        }
    }
}