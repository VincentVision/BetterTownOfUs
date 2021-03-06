using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace BetterTownOfUs
{
    [HarmonyPatch(typeof(ShipStatus))]
    public static class ShipStatusPatch
    {
        // Positions
        public static readonly Vector3 DvdScreenNewPos = new Vector3(26.635f, -15.92f, 1);
        public static readonly Vector3 DvdScreenNewPos2 = new Vector3(25.8f, -15.95f, 1);
        public static readonly Vector3 DvdScreenNewPos3 = new Vector3(26.26f, -15.92f, 1);
        public static Vector3 VitalsNewPos;
        public static readonly Vector3 WifiNewPos = new Vector3(17.38f, 0.15f, 1f);
        public static readonly Vector3 KeysNewPos = new Vector3(20.13f, -10.35f, 0f);
        public static readonly Vector3 QrNewPos = new Vector3(11.07f, -15.2f, -0.015f);
        public static readonly Vector3 DownloadNewPos = new Vector3(21.47f, -24.5f, -0.015f); //22.47f, -24.5f, -0.015f    
        public static readonly Vector3 TempColdNewPos = new Vector3(7.772f, -17.103f, -0.017f);
        
        
        // Checks
        public static bool IsAdjustmentsDone;
        public static bool IsObjectsFetched;
        public static bool IsRoomsFetched;
        public static bool IsObjectsFetched2;
        public static bool IsRoomsFetched2;
        public static bool IsVentsFetched;

        //DvdScreen
        public static GameObject DvdScreenOffice;

        // Tasks Tweak
        public static Console WifiConsole;
        public static Console KeysConsole;
        public static Console DownloadConsole;
        public static Console QrConsole;

        // Vitals Tweak
        public static SystemConsole Vitals;
        public static Console TempCold;
        
        // Vents Tweak
        public static Vent ElectricBuildingVent;
        public static Vent ElectricalVent;
        public static Vent ScienceBuildingVent;
        public static Vent StorageVent;
        
        // Rooms
        public static GameObject Weapons;
        public static GameObject Comms;
        public static GameObject Admin;
        public static GameObject DropShip;
        public static GameObject Storage;
        public static GameObject Outside;
        public static GameObject Science;

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        public static class ShipStatusBeginPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch]       
            public static void Prefix(ShipStatus __instance)
            {
                ApplyChanges(__instance);
            }
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
        public static class ShipStatusAwakePatch
        {
            [HarmonyPrefix]
            [HarmonyPatch]
            public static void Prefix(ShipStatus __instance)
            {
                ApplyChanges(__instance);
            }
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
        public static class ShipStatusFixedUpdatePatch
        {
            [HarmonyPrefix]
            [HarmonyPatch]
            public static void Prefix(ShipStatus __instance)
            {
                if (!IsObjectsFetched || !IsAdjustmentsDone)
                {
                    ApplyChanges(__instance);
                }
            }
        }

        private static void ApplyChanges(ShipStatus instance)
        {
            if ((CustomGameOptions.BetterPolus || CustomGameOptions.PolusVents || CustomGameOptions.SwitchTask) && PlayerControl.GameOptions.MapId == 2)
            {
                FindPolusObjects();
                AdjustPolus();
            }
        }

        public static void FindPolusObjects()
        {
            if (CustomGameOptions.PolusVents) FindVents();
            if (CustomGameOptions.BetterPolus || CustomGameOptions.SwitchTask)
            {
                FindRooms();
                FindObjects();
            }
        }

        public static void AdjustPolus()
        {
            if (CustomGameOptions.BetterPolus || CustomGameOptions.SwitchTask)
            {
                if ((IsObjectsFetched || IsObjectsFetched2) && (IsRoomsFetched || IsRoomsFetched2))
                {
                    if (CustomGameOptions.BetterPolus) MoveVitals();

                    if (CustomGameOptions.SwitchTask) SwitchTasks();

                    MoveDvdScreen();
                }
                else
                {
                    BetterTownOfUs.log.LogError("Couldn't move elements as not all of them have been fetched.");
                }
            }

            if (CustomGameOptions.PolusVents) AdjustVents();

            IsAdjustmentsDone = true;
        }
        
        public static void FindVents()
        {
            var ventsList = Object.FindObjectsOfType<Vent>().ToList();
            
            if (ElectricBuildingVent == null)
            {
                ElectricBuildingVent = ventsList.Find(vent => vent.gameObject.name == "ElectricBuildingVent");
            }
            
            if (ElectricalVent == null)
            {
                ElectricalVent = ventsList.Find(vent => vent.gameObject.name == "ElectricalVent");
            }
            
            if (ScienceBuildingVent == null)
            {
                ScienceBuildingVent = ventsList.Find(vent => vent.gameObject.name == "ScienceBuildingVent");
            }
            
            if (StorageVent == null)
            {
                StorageVent = ventsList.Find(vent => vent.gameObject.name == "StorageVent");
            }

            IsVentsFetched = ElectricBuildingVent != null && ElectricalVent != null && ScienceBuildingVent != null && StorageVent != null;
        }
        
        public static void FindRooms()
        {
            if (CustomGameOptions.BetterPolus)
            {
                if (Science == null)
                {
                    Science = Object.FindObjectsOfType<GameObject>().ToList().Find(o => o.name == "Science");
                }

                IsRoomsFetched = Science != null;
            }

            if (CustomGameOptions.SwitchTask)
            {
                if (Outside == null)
                {
                    Outside = Object.FindObjectsOfType<GameObject>().ToList().Find(o => o.name == "Outside");
                }
            
                if (Weapons == null)
                {
                    Weapons = Object.FindObjectsOfType<GameObject>().ToList().Find(o => o.name == "Weapons");
                }

                if (Comms == null)
                {
                    Comms = Object.FindObjectsOfType<GameObject>().ToList().Find(o => o.name == "Comms");
                }
            
                if (DropShip == null)
                {
                    DropShip = Object.FindObjectsOfType<GameObject>().ToList().Find(o => o.name == "Dropship");
                }

                if (Admin == null)
                {
                    Admin = Object.FindObjectsOfType<GameObject>().ToList().Find(o => o.name == "Admin");
                }
            
                if (Storage == null)
                {   
                    Storage = Object.FindObjectsOfType<GameObject>().ToList().Find(o => o.name == "Storage");
                }

                IsRoomsFetched2 = Outside != null && Weapons != null && Comms != null && Admin != null && Storage != null && DropShip != null;

            }
        }

        // --------------------
        // - Objects Fetching -
        // --------------------
        public static void FindObjects()
        {
            if (DvdScreenOffice == null)
            {
                GameObject DvdScreenAdmin = Object.FindObjectsOfType<GameObject>().ToList()
                    .Find(o => o.name == "dvdscreen");

                if (DvdScreenAdmin != null)
                {
                    DvdScreenOffice = Object.Instantiate(DvdScreenAdmin);
                }
            }
            
            if (CustomGameOptions.BetterPolus)
            {
                if (Vitals == null)
                {
                    Vitals = Object.FindObjectsOfType<SystemConsole>().ToList()
                        .Find(console => console.name == "panel_vitals");
                }

                IsObjectsFetched = Vitals != null && DvdScreenOffice != null;
            }

            if (CustomGameOptions.SwitchTask)
            {
                if (TempCold == null)
                {
                    TempCold = Object.FindObjectsOfType<Console>().ToList()
                        .Find(console => console.name == "panel_tempcold");
                }

                if (WifiConsole == null)
                {
                    WifiConsole = Object.FindObjectsOfType<Console>().ToList()
                        .Find(console => console.name == "panel_wifi");
                }

                if (QrConsole == null)
                {
                    QrConsole = Object.FindObjectsOfType<Console>().ToList()
                        .Find(console => console.name == "panel_boardingpass");
                }

                if (KeysConsole == null)
                {
                    KeysConsole = Object.FindObjectsOfType<Console>().ToList()
                        .Find(console => console.name == "panel_keys");
                }
            
                if (DownloadConsole == null)
                {
                    DownloadConsole = Object.FindObjectsOfType<Console>().ToList()
                        .FindAll(console => console.name == "panel_data").Find(console => console.transform.parent.parent.name == "Weapons");
                }

                IsObjectsFetched2 = TempCold != null && WifiConsole != null && KeysConsole != null && DownloadConsole != null && QrConsole != null && DvdScreenOffice != null;
            }
        }

        // -------------------
        // - Map Adjustments -
        // -------------------
        
        public static void AdjustVents()
        {
            if (IsVentsFetched)
            {
                ElectricBuildingVent.Left = ElectricalVent;
                ElectricalVent.Center = ElectricBuildingVent;

                ScienceBuildingVent.Left = StorageVent;
                StorageVent.Center = ScienceBuildingVent;
            }
        }

        public static void MoveDvdScreen()
        {
            if (!CustomGameOptions.SwitchTask)
            {
                if (DvdScreenOffice.transform.position != DvdScreenNewPos)
                {
                    Transform dvdScreenTransform = DvdScreenOffice.transform;
                    dvdScreenTransform.position = DvdScreenNewPos;
                
                    var localScale = dvdScreenTransform.localScale;
                    localScale = new Vector3(0.75f, localScale.y, localScale.z);
                    dvdScreenTransform.localScale = localScale;
                }
            }
            else if (!CustomGameOptions.BetterPolus)
            {
                if (DvdScreenOffice.transform.position != DvdScreenNewPos2)
                {
                    Transform dvdScreenTransform = DvdScreenOffice.transform;
                    dvdScreenTransform.position = DvdScreenNewPos2;
                
                    var localScale = dvdScreenTransform.localScale;
                    localScale = new Vector3(0.75f, localScale.y, localScale.z);
                    dvdScreenTransform.localScale = localScale;
                }
            }
            else if (DvdScreenOffice.transform.position != DvdScreenNewPos3)
            {
                Transform dvdScreenTransform = DvdScreenOffice.transform;
                dvdScreenTransform.position = DvdScreenNewPos3;
                var localScale = dvdScreenTransform.localScale;
                dvdScreenTransform.localScale = new Vector3(1.1f, localScale.y, localScale.z);
            }
        }
        
        public static void SwitchTasks()
        {
            if (TempCold.transform.position != TempColdNewPos)
            {
                Transform tempColdTransform = TempCold.transform;
                tempColdTransform.parent = Outside.transform;
                tempColdTransform.position = TempColdNewPos;
                BoxCollider2D collider = TempCold.GetComponent<BoxCollider2D>();
                collider.isTrigger = false;
                collider.size += new Vector2(0, -0.3f);
            }
            
            if (WifiConsole.transform.position != WifiNewPos)
            {
                Transform wifiTransform = WifiConsole.transform;
                wifiTransform.parent = DropShip.transform;
                wifiTransform.position = WifiNewPos;
            }

            if (QrConsole.transform.position != QrNewPos)
            {
                Transform QrTransform = QrConsole.transform;
                QrTransform.parent = Comms.transform;
                QrTransform.position = QrNewPos;
                QrTransform.localScale = new Vector3(0.25f, 0.5f, 1);
                
                // Prevents crewmate being able to do the task from outside
                QrConsole.checkWalls = true;
            }

            if (KeysConsole.transform.position != KeysNewPos)
            {
                Transform keysTransform = KeysConsole.transform;
                keysTransform.parent = Storage.transform.GetChild(1);
                keysTransform.position = KeysNewPos;
                
                // Prevents crewmate being able to do the task from outside
                KeysConsole.checkWalls = true;
            }

            /*if (DownloadConsole.transform.position != DownloadNewPos)
            {
                var downloadTransform = DownloadConsole.transform;
                downloadTransform.parent = Admin.transform.GetChild(0);
                downloadTransform.position = DownloadNewPos;
               
                BetterBetterTownOfUs.log.LogMessage(DownloadConsole.enabled);
                BetterBetterTownOfUs.log.LogMessage(DownloadConsole.isActiveAndEnabled);
                BetterBetterTownOfUs.log.LogMessage(DownloadConsole.Room);
                BetterBetterTownOfUs.log.LogMessage(DownloadConsole.TaskTypes.ToString());
                // Prevents crewmate being able to do the task from outside
                DownloadConsole.checkWalls = true;
            }*/
        }
        
        public static void MoveVitals()
        {
            if (Vitals.transform.position != VitalsNewPos)
            {
                Transform vitalsTransform = Vitals.gameObject.transform;
                vitalsTransform.parent = Science.transform;
                VitalsNewPos = CustomGameOptions.SwitchTask ? new Vector3(31.275f, -6.45f, 1) : new Vector3(30.25f, -6.65f, 1);
                vitalsTransform.position = VitalsNewPos;
                if (!CustomGameOptions.SwitchTask)
                {  
                    var WeatherMap = Object.FindObjectsOfType<GameObject>().ToList()
                        .Find(console => console.name == "Weathermap0001");
                    WeatherMap.SetActive(false);
                    vitalsTransform.localScale = new Vector3(1.15f, 1.15f, 1);
                }
            }
        }
    }
}