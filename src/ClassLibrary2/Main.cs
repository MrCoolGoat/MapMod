using System;
using System.Text.RegularExpressions;
using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using Binding.Components;
using TMPro;
using Cinemachine;

namespace MapMod
{
	[BepInPlugin("map.mod.goat", "MapMod", "1.8.0")]
	public class Main : BaseUnityPlugin
	{
		static Main()
		{
			Patch.Hook();
		}

		void Start()
		{

			OGmatrix = GUI.matrix;
		}
		void Update()
		{
			if (mapLoaded && timerEnabled)
			{
				if (timeRemaining > 0)
				{
					timeRemaining -= Time.deltaTime;
				}
				else
				{
					timeRemaining = float.Parse(timerDuration);
					if (OBJLoader.rbGos != null)
					{
						for (int i = 0; i < OBJLoader.rbGos.Count; i++)
						{
							OBJLoader.rbGos[i].transform.position = Vector3.zero;
							OBJLoader.rbGos[i].transform.rotation = Quaternion.Euler(Vector3.zero);
						}
					}
				}
			}
			if (Input.GetKeyDown(KeyCode.F))
			{
				Debug.Log(Patch.valid);
			}
			if (SceneManager.GetActiveScene().name == "SelectCar")
			{
				menuIsOpen = false;
				mapLoaded = false;
				ui = GameObject.Find("UGUI");
				if (ui != null && !loadButtonOnce && GameObject.Find("ModdedRooms") == null)
				{
					Transform[] t = ui.GetComponentsInChildren<Transform>();
					if (t != null)
					{
						for (int i = 0; i < t.Length; i++)
						{
							en = null;
							en = t[i].gameObject.GetComponent<ElementNavigate>();
							if (t[i].gameObject.name == "Rooms")
							{
								en.onSubmit.AddListener(DeactivateMaps);
								newGo = Instantiate(t[i].gameObject, t[i].gameObject.transform.parent);
								newGo.transform.localPosition = new Vector3(t[i].gameObject.transform.localPosition.x + 550, t[i].gameObject.transform.localPosition.y, t[i].gameObject.transform.localPosition.z);
								newGo.name = "ModdedRooms";
								ElementNavigate en2 = newGo.GetComponent<ElementNavigate>();
								en2.onSubmit.AddListener(ActivateMaps);
								Transform[] t2 = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
								if (t2 != null)
								{
									for (int j = 0; j < t2.Length; j++)
									{
										if (t2[j].gameObject.name == "SelectedBG" && t2[j].parent.gameObject.name == "ModdedRooms")
										{
											t2[j].gameObject.SetActive(true);
											t2[j].gameObject.SetActive(true);
										}
									}
								}
								VisibilityBoolBinding[] vbb = newGo.GetComponentsInChildren<VisibilityBoolBinding>();
								if (vbb != null)
								{
									for (int l = 0; l < vbb.Length; l++)
									{
										Type typ = typeof(VisibilityBoolBinding);
										FieldInfo type = typ.BaseType.BaseType.GetField("m_path", System.Reflection.BindingFlags.NonPublic| System.Reflection.BindingFlags.Instance);
										type.SetValue(vbb[l], "ModdedRooms");
									}
								}
								ColorToggleBinding[] ctb = newGo.GetComponentsInChildren<ColorToggleBinding>();
								if (ctb != null)
								{
									for (int k = 0; k < ctb.Length; k++)
									{
										Type typ = typeof(VisibilityBoolBinding);
										FieldInfo type = typ.BaseType.BaseType.GetField("m_path", System.Reflection.BindingFlags.NonPublic| System.Reflection.BindingFlags.Instance);
										type.SetValue(ctb[k], "ModdedRooms");
									}
								}
								loadButtonOnce = true;
							}
							else if (t[i].gameObject.name == "MultiplayerMenuConsole")
							{
								uiMulti = t[i].gameObject;
							}
							else if (t[i].gameObject.name == "Garage" && uiMulti != null && uiMulti.activeSelf)
							{
								t[i].gameObject.transform.localPosition = new Vector3(t[i].gameObject.transform.localPosition.x, 30f, t[i].gameObject.transform.localPosition.z);
							}
							else if (t[i].gameObject.name == "DriftMode" || t[i].gameObject.name == "XDSMode" || t[i].gameObject.name == "TrainingMode" || t[i].gameObject.name == "TimeattackMode")
							{
								en.onSubmit.AddListener(ActivateMaps);
							}
						}
					}
				}
				if (uiMulti == null || !uiMulti.activeSelf)
				{
					loadButtonOnce = false;
				}
				if (newGo != null)
				{
					TextMeshProUGUI tmp = newGo.GetComponentInChildren<TextMeshProUGUI>();
					if (tmp != null)
					{
						Vector2 v = new Vector2();
						v.x = 270;
						v.y = 190;
						tmp.rectTransform.sizeDelta = v;
						tmp.transform.gameObject.SetActive(false);
						tmp.text = "MODDED MAP ROOM LIST";
						tmp.ForceMeshUpdate(true, true);
						tmp.transform.gameObject.SetActive(true);
					}
				}
			}
			if (GameObject.Find("ModdedMap") == null)
			{
				timerEnabled = false;
				mapLoaded = false;
				if (!loadWindowOnce)
				{
					loadWindowOnce = true;
					windowRect = new Rect(10f, 80f, 310f, 800f);
				}
			}
			else
			{
				loadWindowOnce = false;
			}
			if (Input.GetKeyDown(KeyCode.F6) && SceneManager.GetActiveScene().name != "SelectCar" && activateMaps)
			{
				if (SceneManager.GetActiveScene().name != "Airfield")
				{
					Search.SearchFolders();
					menuIsOpen = !menuIsOpen;
				}
				else
				{
					Debug.Log("Maps not supported for Navaro");
				}
			}
			if (playerCar == null)
			{
				Search.CarSearch(ref playerCar);
			}
			if (Input.GetKeyDown(KeyCode.F3))
			{
				if (ExtrasLoader.currentSpawn < ExtrasLoader.spawnPos.Count-1) 
				{
					ExtrasLoader.currentSpawn++;
				}
				else
				{
					ExtrasLoader.currentSpawn = 0;
				}
			}
			if (Input.GetKeyDown(KeyCode.F5) && SceneManager.GetActiveScene().name != "SelectCar")
			{
				if(ExtrasLoader.spawnPos.Count == 0)
				{
					ExtrasLoader.spawnPos.Add(new Vector3(0f, 0f, 0f));
					ExtrasLoader.spawnRot.Add(new Vector3(0f, 0f, 0f));
					ExtrasLoader.currentSpawn = 0;
				}
				playerCar.getTransform.rotation = Quaternion.Euler(ExtrasLoader.spawnRot[ExtrasLoader.currentSpawn]);
				playerCar.getTransform.position = ExtrasLoader.spawnPos[ExtrasLoader.currentSpawn];
				Rigidbody getRigidbody = playerCar.carX.getRigidbody;
				getRigidbody.velocity = Vector3.zero;
				getRigidbody.angularVelocity = Vector3.zero;
			}
			if (Input.GetKeyDown(KeyCode.F10))
			{
				playerCar.getTransform.position = Camera.main.transform.position;
				Rigidbody getRigidbody2 = playerCar.carX.getRigidbody;
				getRigidbody2.velocity = Vector3.zero;
				getRigidbody2.angularVelocity = Vector3.zero;
			}
		}

		void OnGUI()
		{
			float resX = (float)(Screen.width) / originalWidth;
			float resY = (float)(Screen.height) / originalHeight;
			GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(resX * guiScale, resY * guiScale, 1));
			if (menuIsOpen)
			{
				windowRect = GUI.Window(0, windowRect, new GUI.WindowFunction(DoMyWindow), "");
			}
			GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(resX, resY, 1));
		}

		void ActivateMaps()
		{
			Patch.valid = false;
			activateMaps = true;
		}
		void DeactivateMaps()
		{
			Patch.valid = true;
			activateMaps = false;
		}

		void DoMyWindow(int windowID)
		{
			GUIStyle guistyle = new GUIStyle();
			guistyle.fontSize = 19;
			GUIStyle guistyle2 = new GUIStyle(GUI.skin.button);
			Font myFont = (Font)Resources.Load("Arial", typeof(Font));
			guistyle2.font = myFont;
			guistyle2.fontSize = 18;
			GUIStyle guistyle3 = new GUIStyle(GUI.skin.button);
			Font myFont2 = (Font)Resources.Load("Arial", typeof(Font));
			guistyle3.font = myFont2;
			guistyle3.fontSize = 18;
			GUIStyle guistyle4 = new GUIStyle();
			guistyle4.fontSize = 22;
			guistyle4.normal.textColor = Color.white;
			GUIStyle guistyle5 = new GUIStyle();
			guistyle5.fontSize = 50;
			guistyle5.normal.textColor = Color.white;
			GUI.Label(new Rect(10f, 30f, 500f, 100f), "Goats Custom Map Loader V1.8", guistyle);
			if (GUI.Button(new Rect(10f, 60f, 140f, 40f), "Load Map", guistyle2))
			{
				if(clearOnLoad && mapLoaded)
					ClearMap();
				loading = true;
				windowRect = new Rect(10f, 80f, 620f, 800f);
				Search.SearchFiles();
				mapLoaded = true;
				UnityEngine.Object.Destroy(GameObject.Find("map"));
				playerCar.getTransform.position = Search.spawn;
				playerCar.getTransform.rotation = Search.rot;
				Rigidbody getRigidbody = playerCar.carX.getRigidbody;
				getRigidbody.velocity = Vector3.zero;
				getRigidbody.angularVelocity = Vector3.zero;
				loading = false;
			}
			if (GUI.Button(new Rect(160f, 60f, 140f, 40f), "Clear Maps", guistyle2))
			{
				ClearMap();
			}
			scrollHeight = Search.directoriesFiltered.Count * 60f;
			scrollPosition = GUI.BeginScrollView(new Rect(10f, 110f, 290f, 600f), scrollPosition, new Rect(0, 0, 270f, scrollHeight));
			selGridInt = GUI.SelectionGrid(new Rect(0f, 0f, 270f, scrollHeight), selGridInt, Search.directoriesFiltered.ToArray(), 1, guistyle3);
			GUI.EndScrollView();
			GUI.Label(new Rect(70f, 720f, 500f, 100f), "Scale: " + Math.Round(guiScale * 10), guistyle4);
			if (GUI.Button(new Rect(10f, 750f, 140f, 40f), "<<", guistyle2))
			{
				guiScale -= 0.1f;
				if (guiScale <= 0.4f)
				{
					guiScale = 0.4f;
				}
			}
			if (GUI.Button(new Rect(160f, 750f, 140f, 40f), ">>", guistyle2))
			{
				guiScale += 0.1f;
				if (guiScale >= 1f)
				{
					guiScale = 1f;
				}
			}
			if (mapLoaded)
			{
				if (GUI.Button(new Rect(310f, 60f, 300f, 40f), "Reload Extras", guistyle2))
				{
					foreach (Transform child in ExtrasLoader.allLights.transform)
					{
						Destroy(child.gameObject);
					}
					Search.SearchFiles(true);
				}
				GUI.Label(new Rect(460f, 125f, 500f, 100f), "Clear On Load", guistyle4);
				if(clearOnLoad)
                {
					if (GUI.Button(new Rect(310f, 120f, 140f, 40f), "On", guistyle2))
					{
						clearOnLoad = false;
					}
				}
				else
                {
					if (GUI.Button(new Rect(310f, 120f, 140f, 40f), "Off", guistyle2))
					{
						clearOnLoad = true;
					}
				}
				
				GUI.Label(new Rect(460f, 170f, 500f, 100f), "Lights", guistyle4);
				if (GUI.Button(new Rect(310f, 200f, 140f, 40f), "On", guistyle2))
				{
					lightEnabled = true;
				}
				if (GUI.Button(new Rect(460f, 200f, 140f, 40f), "Off", guistyle2))
				{
					lightEnabled = false;
				}
				GUI.Label(new Rect(350f, 250f, 500f, 100f), "Light Range: " + lightRange, guistyle4);
				lightRange = GUI.HorizontalSlider(new Rect(310f, 330f, 300f, 20f), lightRange, 0f, 200f);
				try
				{
					foreach (Transform child in ExtrasLoader.allLights.transform)
					{
						Light light = child.gameObject.GetComponent<Light>();
						light.enabled = lightEnabled;
						light.range = lightRange;
					}
				}
				catch { }
				GUI.Label(new Rect(350f, 390f, 500f, 100f), "Rigidbodies", guistyle4);
				if (GUI.Button(new Rect(310f, 440f, 140f, 40f), "Reset Timer On", guistyle2))
				{
					timerEnabled = true;
					timeRemaining = float.Parse(timerDuration);
				}
				if (GUI.Button(new Rect(460f, 440f, 140f, 40f), "Reset Timer Off", guistyle2))
				{
					timerEnabled = false;
				}
				GUI.Label(new Rect(350f, 490f, 500f, 100f), "Timer Duration (s)", guistyle4);
				timerDuration = GUI.TextField(new Rect(360f, 570f, 100f, 20f), timerDuration, guistyle4);
				timerDuration = Regex.Replace(timerDuration, @"[^0-9]", "");
				GUI.Label(new Rect(350f, 600f, 500f, 100f), "View Distance: " + viewDistance, guistyle4);
				viewDistance = GUI.HorizontalSlider(new Rect(310f, 630f, 300f, 20f), viewDistance, 10f, 2000f);
				if (Camera.main != null)
				{
					Camera.main.farClipPlane = viewDistance;
				}
				GameObject vc = GameObject.Find("VirtualCamera");
				if (vc != null)
				{
					CinemachineVirtualCamera cvc = vc.GetComponent<CinemachineVirtualCamera>();
					cvc.m_Lens.FarClipPlane = viewDistance;
				}
			}
			if (loading)
			{
				GUI.Label(new Rect(1000f, 500f, 500f, 100f), "Loading....", guistyle5);
			}
			GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
		}

		void ClearMap()
        {
			windowRect = new Rect(10f, 80f, 310f, 800f);
			foreach (GameObject obj in Search.mapGos)
			{
				UnityEngine.Object.Destroy(obj);
			}
			foreach (GameObject rb in OBJLoader.rbGos)
			{
				UnityEngine.Object.Destroy(rb);
			}
			foreach (Transform child in ExtrasLoader.allLights.transform)
			{
				Destroy(child.gameObject);
			}
			OBJLoader.rbGos.Clear();
			ExtrasLoader.spawnPos.Clear();
			ExtrasLoader.spawnRot.Clear();
			mapLoaded = false;
			timeRemaining = 30f;
			Resources.UnloadUnusedAssets();
		}

		static RaceCar playerCar;

		static bool menuIsOpen;

		static Rect windowRect = new Rect(10f, 80f, 310f, 800f);

		Vector2 scrollPosition = Vector2.zero;

		float scrollHeight;

		public static int selGridInt;

		float originalWidth = 1920f;

		float originalHeight = 1080f;

		float guiScale = 0.8f;

		float lightRange = 30f;

		string timerDuration = "30";

		float viewDistance = 1500f;

		bool lightEnabled = true;

		static float timeRemaining = 30f;

		static bool mapLoaded = false;

		static bool timerEnabled = true;

		ElementNavigate en;

		GameObject ui;

		GameObject uiMulti;

		GameObject newGo;

		bool activateMaps = false;

		bool loadButtonOnce = false;

		bool loadWindowOnce = false;

		bool loading = false;

		bool clearOnLoad = true;

		Matrix4x4 OGmatrix;
	}
}