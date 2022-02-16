using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using QFSW.QC;

namespace Kyota
{
    public static class QCCustom
    {
        private static Dictionary<string, InputAction> inputActions = new Dictionary<string, InputAction>();
        
        static QCCustom()
        {
            inputActions.Clear();
            foreach (InputAction action in Player.playerInputs.Land.Get().actions)
            {
                string actionName = action.name;
                inputActions.Add(actionName.ToLower(), action);
                Debug.Log(actionName);
            }
        }
        
        [Command("all-commands")]
        private static string ListAllCommands()
        {
            string text = "Commands list:";
            text += "\nrunMoveSpeed - Target X velocity when running (you also have to change moveSpeed for this to take effect)";
            text += "\nmoveSpeed - Move speed";
            text += "\nxDeceleration - X Deceleration per second when not sliding";
            text += "\njumpForce - Instance Y velocity when jump";
            text += "\nfallMultiplier - Gravity multiplier when falling";
            text += "\ntopJumpMultiplier - Gravity multiplier at jump peak";
            text += "\ntopJumpThreshold - Threshold to activate (lower value = higher peak)";
            text += "\nlowJumpMultiplier - Gravity multiplier when not pressing jump key (higher value = faster low jump)";
            text += "\nwallJumpForce - Force when wall jumping (set with format \"(0, 0)\")";
            text += "\ndownSlopeXVelocity - Target X velocity when sliding on a ramp";
            text += "\ndownSlopeXAcceleration - X Acceleration per second when sliding on a ramp";
            text += "\nbulletForce - Bullet speed";
            return text;
        }
        
        [Command("rebind-key")]
        private static string RebindKey(string actionName, int bindingIndex)
        {
            InputAction action = inputActions[actionName.ToLower()];
            action.Disable();

            InputActionRebindingExtensions.RebindingOperation operation = action.PerformInteractiveRebinding(bindingIndex);

            operation.WithControlsExcluding("Mouse");
            operation.OnMatchWaitForAnother(0.1f);
            operation.Start().OnComplete(op =>
            {
                QuantumConsole.Instance.OverrideConsoleInput("", false);
                op.action.Enable();
                Debug.Log($"{actionName[0].ToString().ToUpper() + actionName.ToLower().Substring(1)} input action has been rebinded to {op.selectedControl}");
            });
            
            return "Press the desired key for the rebind.";
        }

        [Command("load-scene")]
        private static void LoadScene(string sceneName)
        {
            SceneLoader.LoadScene(sceneName);
        }
    }
}
