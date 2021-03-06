using System;
using Hazel;
using UnityEngine;

namespace BetterTownOfUs.Roles
{
    public class Cannibal : Role
    {
        public DeadBody CurrentTarget { get; set; }
        public DateTime LE { get; set; }
        public int EatNeed;
        public bool CannibalWin;
        
        public Cannibal(PlayerControl player) : base(player, RoleEnum.Cannibal)
        {
            ImpostorText = () => "Eat Dead";
            EatNeed = CustomGameOptions.EatNeed == 0 ? PlayerControl.AllPlayerControls._size / 3 : CustomGameOptions.EatNeed;
            string body = EatNeed == 1 ? "Body" : "Bodies";
            TaskText = () => $"You're hungry, you need to eat {EatNeed} Dead {body} to Win\nFake Tasks:";
        }
        
        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (EatNeed < 1)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.CannibalWin, SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            
            return true;
        }

        public void Wins()
        {
            CannibalWin = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._CoBegin_d__18 __instance)
        {
            var cannibalteam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            cannibalteam.Add(PlayerControl.LocalPlayer);
            __instance.yourTeam = cannibalteam;
        }

        public float CannibalTimer()
        {
            var t = DateTime.UtcNow - LE;
            var i = CustomGameOptions.CannibalCd * 1000;
            if (i - (float) t.TotalMilliseconds < 0) return 0;
            return (i - (float) t.TotalMilliseconds) / 1000;
        }
    }
}