using HarmonyLib;
using BepInEx.Harmony;
using System.Reflection;
using UnityEngine;

namespace MapMod
{
	static class Patch
	{
		public static void Hook()
		{
			Harmony harmony = new Harmony("map.mod.goat");
			harmony.PatchAll();
		}

		
		[HarmonyPatch(typeof(ModelValidator), "isValid", MethodType.Getter)]
		class Patch01
        {
			[HarmonyPrefix]
			private static bool ValidatorPatch(ref bool __result)
			{
				__result = valid;
				return false;
			}
		}
		public static bool valid = true;
	}
}
