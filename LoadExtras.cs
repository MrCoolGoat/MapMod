using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx;
using CarX;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.HighDefinition;
using Material = UnityEngine.Material;

namespace MapMod
{
    class LoadExtras
    {
		public static void MapExtras(string fn)
		{
			spawnPos.Clear();
			spawnRot.Clear();
			string[] extrasLines = File.ReadAllLines(fn);
			foreach (string line in extrasLines)
			{
				string[] split = line.Split(':');
				string extraType = split[0].ToLower();
				string[] parameters = split[1].ToLower().Split(' ');
				switch (extraType)
				{
					case "spawn":
						spawnPos.Add(new Vector3(-float.Parse(parameters[0]), float.Parse(parameters[2]), -float.Parse(parameters[1])));
						spawnRot.Add(new Vector3(float.Parse(parameters[3]), float.Parse(parameters[5]), float.Parse(parameters[4])));
						currentSpawn = 0;
						break;
					case "camera":
						FollowCamera followCamera = UnityEngine.Object.FindObjectOfType<FollowCamera>();
						if (followCamera == null)
						{
							Debug.LogWarning("Follow camera not found");
							return;
						}
						GameObject camerasGo = new GameObject("CameraTrackingPoint Extra");
						Transform cameraPos = camerasGo.transform;
						cameraPos.position = new Vector3(-float.Parse(parameters[0]), float.Parse(parameters[2]), -float.Parse(parameters[1]));
						followCamera.AddCustomCameraPoint(cameraPos);
						break;
				}
				if (extraType.Contains("light"))
				{
					float[] floatParameters = Array.ConvertAll(parameters, delegate (string s)
					{
						return float.Parse(s);
					});
					CreateLights(floatParameters, extraType);
				}
			}
		}

		static void CreateLights(float[] floatParameters, string extraType)
		{
			GameObject newLightGo = new GameObject("custom " + extraType);
			var newLight = newLightGo.AddHDLight(HDLightTypeAndShape.ConeSpot);
			newLight.intensity = 10f;
			newLight.range = 150f;
			newLight.color = Color.white;
			newLight.EnableColorTemperature(true);
			newLight.EnableShadows(true);
			newLight.SetShadowUpdateMode(ShadowUpdateMode.EveryFrame);
			newLight.useRayTracedShadows = true;
			switch (extraType)
			{
				case "spotlight":
					Debug.Log("spot");
					newLight.type = HDLightType.Spot;
					newLight.SetSpotAngle(floatParameters[10]);
					newLight.SetIntensity(floatParameters[6] * 1700);
					newLight.SetColor(new Color(floatParameters[7], floatParameters[8], floatParameters[9]), 5500f);
					newLightGo.transform.position = new Vector3(-floatParameters[0], floatParameters[2], -floatParameters[1]);
					allLights.Add(newLightGo);
					Rotate(newLightGo, floatParameters);
					break;
				case "sunlight":
					Debug.Log("sun");
					newLightGo = GameObject.Find("sunlight");
					HDAdditionalLightData sunLight = newLightGo.GetComponent<HDAdditionalLightData>();
					sunLight.SetIntensity(floatParameters[6]);
					sunLight.SetColor(new Color(floatParameters[7], floatParameters[8], floatParameters[9]), 5500f);
					newLightGo.transform.position = new Vector3(-floatParameters[0], floatParameters[2], -floatParameters[1]);
					Rotate(newLightGo, floatParameters);
					break;
				case "pointlight":
					Debug.Log("point");
					newLight.type = HDLightType.Point;
					newLight.SetIntensity(floatParameters[6] * 1700);
					newLight.SetColor(new Color(floatParameters[4], floatParameters[5], floatParameters[6]), 5500f);
					allLights.Add(newLightGo);
					break;
			}
		}

		public static void Rotate(GameObject go, float[] rot)
		{
			go.transform.eulerAngles = Vector3.zero;
			go.transform.Rotate(0.0f, rot[5] * -1 + 180, 0.0f);
			go.transform.Rotate(0.0f, 0.0f, rot[4] * -1);
			go.transform.Rotate(rot[3] * -1 + 90, 0.0f, 0.0f);
			Debug.Log("Quaternion Rotation After Mods" + " x: " + go.transform.rotation.x + " y: " + go.transform.rotation.y + " z: " + go.transform.rotation.z + " w: " + go.transform.rotation.w);
			Debug.Log("Euler Rotation After Mods" + " x: " + go.transform.eulerAngles.x + " y: " + go.transform.eulerAngles.y + " z: " + go.transform.eulerAngles.z);
		}

		public static List<GameObject> allLights = new List<GameObject>();

		public static List<Vector3> spawnPos = new List<Vector3>();

		public static List<Vector3> spawnRot = new List<Vector3>();

		public static int currentSpawn;
	}
}
