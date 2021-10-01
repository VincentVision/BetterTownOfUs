using System;
using HarmonyLib;
using UnityEngine;

namespace BetterTownOfUs
{
    //[HarmonyPriority(Priority.VeryHigh)] // to show this message first, or be overrided if any plugins do
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTracker_Update
    {
        public static GameObject spriteObject;
        [HarmonyPrefix]
        public static void Prefix(PingTracker __instance)
        {
            if (!__instance.GetComponentInChildren<SpriteRenderer>())
            {
                spriteObject = new GameObject("VotezVertSprite");
                spriteObject.AddComponent<SpriteRenderer>().sprite = BetterTownOfUs.VotezVertSprite;
                spriteObject.transform.parent = __instance.transform;
                spriteObject.transform.localPosition =  new Vector3(-1.1f, -0.7f, 0);
                spriteObject.transform.localScale *= 0.72f;
            }
        }

        [HarmonyPostfix]
        public static void Postfix(PingTracker __instance)
        {
            var position = __instance.GetComponent<AspectPosition>();
            position.DistanceFromEdge = new Vector3(3.1f, 0.02f, 0);
            position.AdjustPosition();

            __instance.text.fontSize = 2.5f;
            __instance.text.text =
                "<color=#018001FF>BetterTownOfUs v1.0.0</color>\n" +
                "Disponible sur/Available on :\n" +
                "<color=#3769feFF>discord.gg/XSCnBzkneK</color>\n" +
                $"Ping: {AmongUsClient.Instance.Ping}ms\n" +
                (!MeetingHud.Instance
                    ? "<color=#018001FF>Votez Vert ft. JMC</color>"
                    : "");
            if (MeetingHud.Instance) PingTracker_Update.spriteObject.transform.localPosition =  new Vector3(-1.1f, -0.6f, 0);
        }
    }
}