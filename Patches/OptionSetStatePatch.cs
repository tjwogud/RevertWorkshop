using HarmonyLib;
using RevertWorkshop.Components;
using RevertWorkshop.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace RevertWorkshop.Patches
{
    [HarmonyPatch]
    public static class OptionSetStatePatch
    {
        public static bool Prepare()
        {
            return Main.optionsPanelsCLS != null;
        }

        public static MethodBase TargetMethod()
        {
            return AccessTools.Method(Main.option, "SetState");
        }

        public static void Postfix(object __instance, ref bool ___selected, bool _highlighted, bool _selected)
        {
            if (__instance.Get<int>("name") == (int)Enum.Parse(Main.optionName, "Delete"))
                ___selected = false;
        }

        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            for (int i = 0; i < instructions.Count(); i++)
            {
                CodeInstruction code = instructions.ElementAt(i);
                if (code.operand is MethodInfo method && method.Name == "DeleteLevel")
                {
                    Label revertLabel = generator.DefineLabel();
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Main.option, "text"));
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(MonoBehaviour), "transform").GetGetMethod());
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(Transform), "parent").GetGetMethod());
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(Transform), "name").GetGetMethod());
                    yield return new CodeInstruction(OpCodes.Ldstr, "Revert");
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), "op_Equality"));
                    yield return new CodeInstruction(OpCodes.Brtrue, revertLabel);
                    yield return code;
                    yield return new CodeInstruction(OpCodes.Ret);
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(MonoBehaviour), "GetComponent").MakeGenericMethod(typeof(WorkshopReverter))).WithLabels(revertLabel);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(WorkshopReverter), "RevertLevel"));
                    yield return new CodeInstruction(OpCodes.Ret);
                } else
                    yield return code;
            }
        }
    }
}
