using System.Linq;
using AmongUs.Data;
using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameEnd;

namespace MiraAPI.Patches.Events;

internal static class EndGamePatches
{
    // To un-inline RpcEndGame
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
    public static class LogicGameFlowNormalPatches
    {
        public static void Prefix(LogicGameFlowNormal __instance, ref bool __runOriginal)
        {
            __runOriginal = false;
            if (!GameData.Instance)
            {
                return;
            }

            if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.LifeSupp, out var systemType))
            {
                var lifeSuppSystemType = systemType.Cast<LifeSuppSystemType>();
                if (lifeSuppSystemType.Countdown < 0f)
                {
                    __instance.EndGameForSabotage();
                    lifeSuppSystemType.Countdown = 10000f;
                }
            }

            foreach (var systemType2 in ShipStatus.Instance.Systems.Values)
            {
                var criticalSabotage = systemType2.TryCast<ICriticalSabotage>();
                if (criticalSabotage is { Countdown: < 0f })
                {
                    __instance.EndGameForSabotage();
                    criticalSabotage.ClearSabotage();
                }
            }

            var playerCounts = __instance.GetPlayerCounts();
            var item = playerCounts.Item1;
            var item2 = playerCounts.Item2;
            var item3 = playerCounts.Item3;
            if (item2 <= 0 && (!TutorialManager.InstanceExists || item3 > 0))
            {
                if (!TutorialManager.InstanceExists)
                {
                    var gameOverReason = GameOverReason.CrewmatesByVote;
                    __instance.Manager.RpcEndGame(gameOverReason, !DataManager.Player.Ads.HasPurchasedAdRemoval);
                    return;
                }

                if (!Minigame.Instance)
                {
                    HudManager.Instance.ShowPopUp(
                        TranslationController.Instance.GetString(StringNames.GameOverImpostorDead));
                    __instance.Manager.ReviveEveryoneFreeplay();
                }
            }
            else if (item <= item2)
            {
                if (!TutorialManager.InstanceExists)
                {
                    var gameOverReason2 = GameData.LastDeathReason switch
                    {
                        DeathReason.Exile => GameOverReason.ImpostorsByVote,
                        DeathReason.Kill => GameOverReason.ImpostorsByKill,
                        _ => GameOverReason.CrewmateDisconnect,
                    };

                    __instance.Manager.RpcEndGame(gameOverReason2, !DataManager.Player.Ads.HasPurchasedAdRemoval);
                    return;
                }

                HudManager.Instance.ShowPopUp(TranslationController.Instance.GetString(StringNames.GameOverImpostorKills));
                __instance.Manager.ReviveEveryoneFreeplay();
            }
            else
            {
                if (!TutorialManager.InstanceExists)
                {
                    __instance.Manager.CheckEndGameViaTasks();
                    return;
                }

                if (PlayerControl.LocalPlayer.myTasks.ToArray().All(t => t.IsComplete))
                {
                    HudManager.Instance.ShowPopUp(
                        TranslationController.Instance.GetString(
                            StringNames.GameOverTaskWin));
                    ShipStatus.Instance.Begin();
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
    public static class GameManagerPatches
    {
        public static bool Prefix(GameOverReason endReason)
        {
            var @event = new BeforeGameEndEvent(endReason);
            MiraEventManager.InvokeEvent(@event);

            return !@event.IsCancelled;
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public static class EndGameManagerPatches
    {
        public static bool Prefix(EndGameManager __instance)
        {
            if (CustomGameOver.Instance is not { } gameOver)
            {
                return true;
            }

            var result = gameOver.BeforeEndGameSetup(__instance);
            return result;
        }

        public static void Postfix(EndGameManager __instance)
        {
            if (CustomGameOver.Instance is { } gameOver)
            {
                gameOver.AfterEndGameSetup(__instance);
                CustomGameOver.Instance = null;
            }

            var @event = new GameEndEvent(__instance);
            MiraEventManager.InvokeEvent(@event);
        }
    }
}
