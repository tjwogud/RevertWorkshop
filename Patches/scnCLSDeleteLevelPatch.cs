using HarmonyLib;
using RevertWorkshop.Components;

namespace RevertWorkshop.Patches
{
    [HarmonyPatch(typeof(scnCLS), "DeleteLevel")]
    public static class scnCLSDeleteLevelPatch
    {
        public static bool Prepare()
        {
            return Main.optionsPanelsCLS != null;
        }

        public static void Postfix(scnCLS __instance)
        {
            __instance.GetComponent<WorkshopReverter>().SetText();
        }
    }
}
