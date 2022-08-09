using HarmonyLib;
using RevertWorkshop.Components;
using UnityEngine;
using UnityEngine.UI;

namespace RevertWorkshop.Patches
{
    [HarmonyPatch(typeof(scnCLS), "Awake")]
    public static class scnCLSAwakePatch
    {
        public static void Postfix(scnCLS __instance)
        {
            __instance.gameObject.AddComponent<WorkshopReverter>();
            Transform helpContainer = __instance.levelInfoCanvas.transform.Find("HelpContainer");
            if (helpContainer != null && helpContainer.gameObject.activeSelf)
            {
                helpContainer.localPosition = new Vector3(helpContainer.localPosition.x, helpContainer.localPosition.y + 40, helpContainer.localPosition.z);
                Transform helpRevert = Object.Instantiate(helpContainer.Find("HelpDel"), helpContainer);
                helpRevert.name = "HelpRevert";
                helpRevert.SetSiblingIndex(helpRevert.GetSiblingIndex() - 1);
                Object.DestroyImmediate(helpRevert.GetComponent<scrTextChanger>());
                helpRevert.GetComponent<Text>().text =
                    RDString.language == SystemLanguage.English
                    ? "R to revert level"
                    : "R: 레벨 복구";
            }
        }
    }
}
