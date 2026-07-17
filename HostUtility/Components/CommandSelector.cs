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
    public class CommandSelector(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public Il2CppReferenceField<Button> modButtonPrefab;
        public Il2CppReferenceField<Button> commandButtonPrefab;
        public Il2CppReferenceField<Transform> modButtonsParent;
        public Il2CppReferenceField<Transform> commandButtonsParent;

        private void Start()
        {
            HudManager.Instance.StartCoroutine(Effects.ScaleIn(transform.GetChild(0), 0, 1, 0.2f));
            PopulateMods();
            PopulateCommands(ChatCommandsManager.Commands);
            if (OperatingSystem.IsAndroid()) GetComponent<CanvasScaler>().scaleFactor /= 0.8f;
            transform.GetChild(0).GetChild(0).GetComponent<Button>().m_OnClick.AddListener(new System.Action(() => gameObject.Destroy()));
        }
        
        private void OnDisable()
        {
            gameObject.Destroy();
        }

        void PopulateMods()
        {
            var button = Object.Instantiate(modButtonPrefab.Value, modButtonsParent.Value);
            button.onClick.AddListener(new System.Action(() => PopulateCommands(ChatCommandsManager.Commands)));
        }
        
        void PopulateCommands(List<ChatCommand> commands)
        {
            foreach (var existingButton in commandButtonsParent.Value.GetComponentsInChildren<Button>())
            {
                existingButton.gameObject.Destroy();
            }

            foreach (var command in commands)
            {
                var button = Object.Instantiate(commandButtonPrefab.Value, commandButtonsParent.Value);
                button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = command.Command;
                button.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = command.Description;
                button.onClick.AddListener(new System.Action(() =>
                {
                    gameObject.Destroy();
                    ExecuteCommandMenu.CreateAndShow(command);
                }));
            }
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape)) gameObject.Destroy();
        }

        private void OnDestroy()
        {
            ControllerManager.Instance.CloseOverlayMenu("CommandsMenu");
        }

        public static void CreateAndShow()
        {
            var menu = Object.Instantiate(HostUtilityPlugin.Bundle.LoadAsset<GameObject>("CommandsCanvas"));
            ControllerManager.Instance.OpenOverlayMenu("CommandsMenu", null);
        }
    }
}