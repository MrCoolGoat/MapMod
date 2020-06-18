using BepInEx.Harmony;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace MapMod
{
	internal static class Patch
	{
		public static void Hook()
		{
			HarmonyWrapper.PatchAll();
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(ModelValidator), "isValid", MethodType.Getter)]
		private static bool ValidatorPatch(ref bool __result)
		{
			Debug.Log("patched");
			__result = false;
			return false;
		}
	}
}
