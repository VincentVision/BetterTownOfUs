using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using Reactor;
using Reactor.Extensions;
using BetterTownOfUs.CustomOption;
using BetterTownOfUs.RainbowMod;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BetterTownOfUs
{
    [BepInPlugin(Id, "Better Town Of Us", MajorVersion)]
    [BepInDependency(ReactorPlugin.Id)]
    public class BetterTownOfUs : BasePlugin
    {
        public const string MajorVersion = "2.1.0";
        public static string GetVersion() => typeof(BetterTownOfUs).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        private const string Id = "fr.vincentvision.BetterTownOfUs";

        public static Sprite JanitorClean;
        public static Sprite EngineerFix;
        public static Sprite SwapperSwitch;
        public static Sprite SwapperSwitchDisabled;
        public static Sprite Shift;
        public static Sprite Footprint;
        public static Sprite Rewind;
        public static Sprite NormalKill;
        public static Sprite ShiftKill;
        public static Sprite MedicSprite;
        public static Sprite SeerSprite;
        public static Sprite SampleSprite;
        public static Sprite MorphSprite;
        public static Sprite Camouflage;
        public static Sprite Arrow;
        public static Sprite Abstain;
        public static Sprite MineSprite;
        public static Sprite SwoopSprite;
        public static Sprite DouseSprite;
        public static Sprite IgniteSprite;
        public static Sprite ReviveSprite;
        public static Sprite ButtonSprite;
        public static Sprite VotezVertSprite;

        public static Sprite CycleSprite;
        public static Sprite GuessSprite;


        public static Sprite DragSprite;
        public static Sprite DropSprite;
        public static Sprite FlashSprite;
        public static Sprite SettingsButtonSprite;
        public static Vector3 ButtonPosition { get; private set; } = new Vector3(2.6f, 0.7f, -9f);

        private static DLoadImage _iCallLoadImage;


        private Harmony _harmony;

        public static ConfigEntry<bool> StreamMode;
        public ConfigEntry<string> Ip { get; set; }

        public ConfigEntry<ushort> Port { get; set; }
        public static ManualLogSource log;


        public override void Load()
        {
            log = Log;
            System.Console.WriteLine("000.000.000.000/000000000000000000");

            _harmony = new Harmony("fr.vincentvision.BetterTownOfUs");

            Generate.GenerateAll();

            JanitorClean = CreateSprite("BetterTownOfUs.Resources.Janitor.png");
            EngineerFix = CreateSprite("BetterTownOfUs.Resources.Engineer.png");
            SwapperSwitch = CreateSprite("BetterTownOfUs.Resources.SwapperSwitch.png");
            SwapperSwitchDisabled = CreateSprite("BetterTownOfUs.Resources.SwapperSwitchDisabled.png");
            Shift = CreateSprite("BetterTownOfUs.Resources.Shift.png");
            Footprint = CreateSprite("BetterTownOfUs.Resources.Footprint.png");
            Rewind = CreateSprite("BetterTownOfUs.Resources.Rewind.png");
            NormalKill = CreateSprite("BetterTownOfUs.Resources.NormalKill.png");
            ShiftKill = CreateSprite("BetterTownOfUs.Resources.ShiftKill.png");
            MedicSprite = CreateSprite("BetterTownOfUs.Resources.Medic.png");
            SeerSprite = CreateSprite("BetterTownOfUs.Resources.Seer.png");
            SampleSprite = CreateSprite("BetterTownOfUs.Resources.Sample.png");
            MorphSprite = CreateSprite("BetterTownOfUs.Resources.Morph.png");
            Camouflage = CreateSprite("BetterTownOfUs.Resources.Camouflage.png");
            Arrow = CreateSprite("BetterTownOfUs.Resources.Arrow.png");
            Abstain = CreateSprite("BetterTownOfUs.Resources.Abstain.png");
            MineSprite = CreateSprite("BetterTownOfUs.Resources.Mine.png");
            SwoopSprite = CreateSprite("BetterTownOfUs.Resources.Swoop.png");
            DouseSprite = CreateSprite("BetterTownOfUs.Resources.Douse.png");
            IgniteSprite = CreateSprite("BetterTownOfUs.Resources.Ignite.png");
            ReviveSprite = CreateSprite("BetterTownOfUs.Resources.Revive.png");
            ButtonSprite = CreateSprite("BetterTownOfUs.Resources.Button.png");
            DragSprite = CreateSprite("BetterTownOfUs.Resources.Drag.png");
            DropSprite = CreateSprite("BetterTownOfUs.Resources.Drop.png");
            VotezVertSprite = CreateSprite("BetterTownOfUs.Resources.Votez Vert.png");
            CycleSprite = CreateSprite("BetterTownOfUs.Resources.Cycle.png");
            GuessSprite = CreateSprite("BetterTownOfUs.Resources.Guess.png");
            FlashSprite = CreateSprite("BetterTownOfUs.Resources.Flash.png");
            SettingsButtonSprite = CreateSprite("BetterTownOfUs.Resources.SettingsButton.png");

            PalettePatch.Load();
            ClassInjector.RegisterTypeInIl2Cpp<RainbowBehaviour>();

            Ip = Config.Bind("Custom", "Ipv4 or Hostname", "127.0.0.1");
            Port = Config.Bind("Custom", "Port", (ushort) 22023);
            var defaultRegions = ServerManager.DefaultRegions.ToList();
            var ip = Ip.Value;
            if (Uri.CheckHostName(Ip.Value).ToString() == "Dns")
                foreach (var address in Dns.GetHostAddresses(Ip.Value))
                {
                    if (address.AddressFamily != AddressFamily.InterNetwork)
                        continue;
                    ip = address.ToString();
                    break;
                }

            ServerManager.DefaultRegions = defaultRegions.ToArray();
 
            StreamMode = Config.Bind("Custom", "Enable Streamer Mode", false);
            _harmony.PatchAll();
            DirtyPatches.Initialize(_harmony);
            log.LogMessage($"Better Town of Us {GetVersion()} by Votez Vert.");
        }

        public static Sprite CreateSprite(string name)
        {
            var pixelsPerUnit = 100f;
            var pivot = new Vector2(0.5f, 0.5f);

            var assembly = Assembly.GetExecutingAssembly();
            var tex = GUIExtensions.CreateEmptyTexture();
            var imageStream = assembly.GetManifestResourceStream(name);
            var img = imageStream.ReadFully();
            LoadImage(tex, img, true);
            tex.DontDestroy();
            var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), pivot, pixelsPerUnit);
            sprite.DontDestroy();
            return sprite;
        }

        public static void LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            _iCallLoadImage ??= IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
            var il2CPPArray = (Il2CppStructArray<byte>) data;
            _iCallLoadImage.Invoke(tex.Pointer, il2CPPArray.Pointer, markNonReadable);
        }

        private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
    }
}
