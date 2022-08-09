using HarmonyLib;
using RevertWorkshop.Components;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace RevertWorkshop.Patches
{
    [HarmonyPatch]
    public static class OptionsPanelsCLSTogglePanelPatch
    {
        public static bool Prepare()
        {
            return Main.optionsPanelsCLS != null;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method(Main.optionsPanelsCLS, "TogglePanel");
        }

        public static void Postfix(bool left)
        {
            if (!left)
                scnCLS.instance.GetComponent<WorkshopReverter>().SetText();
        }

        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            Label? label = null;
            bool rtn = false;
            for (int i = 0; i < instructions.Count(); i++)
            {
                CodeInstruction code = instructions.ElementAt(i);
                if (label == null)
                {
                    if (code.operand is Label l)
                        label = l;
                }
                else if (code.labels.Contains(label.Value))
                {
                    rtn = true;
                }
                if (rtn)
                    yield return code;
            }
        }
    }
}
