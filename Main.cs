using HarmonyLib;
using System;
using System.Reflection;
using UnityModManagerNet;

namespace RevertWorkshop
{
    public static class Main
    {
        public static UnityModManager.ModEntry ModEntry;
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static Harmony Harmony;
        public static readonly Type optionsPanelsCLS = typeof(ADOBase).Assembly.GetType("OptionsPanelsCLS");
        public static readonly Type option = typeof(ADOBase).Assembly.GetType("OptionsPanelsCLS+Option");
        public static readonly Type optionName = typeof(ADOBase).Assembly.GetType("OptionsPanelsCLS+OptionName");

        public static void Setup(UnityModManager.ModEntry modEntry)
        {
            ModEntry = modEntry;
            Logger = modEntry.Logger;
            Harmony = new Harmony(modEntry.Info.Id);
            modEntry.OnToggle = (_, value) =>
            {
                if (value)
                    Harmony.PatchAll(Assembly.GetExecutingAssembly());
                else
                    Harmony.UnpatchAll(modEntry.Info.Id);
                return true;
            };
        }
    }
}
