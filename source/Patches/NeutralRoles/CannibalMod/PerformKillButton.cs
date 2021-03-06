using System;
using HarmonyLib;
using Hazel;
using Reactor;
using BetterTownOfUs.Roles;
using UnityEngine;

namespace BetterTownOfUs.NeutralRoles.CannibalMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton

    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Cannibal)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!__instance.isActiveAndEnabled) return false;
            if (__instance.isCoolingDown) return false;
            var role = Role.GetRole<Cannibal>(PlayerControl.LocalPlayer);
            if (role.CannibalTimer() != 0) return false;

            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            if (Vector2.Distance(role.CurrentTarget.TruePosition,
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;

            var playerId = role.CurrentTarget.ParentId;
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.CannibalEat, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            role.LE = DateTime.UtcNow;

            Coroutines.Start(Coroutine.EatCoroutine(role.CurrentTarget, role));
            return false;
        }
    }
}