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
	[BepInPlugin("map.mod.goat", "MapMod", "1.16.0")]
	public class Main : BaseUnityPlugin
	{
		static Main()
		{
			Patch.Hook();
			windowRect = new Rect(10f, 80f, 400f, 750f);
		}

		public void Update()
		{

			if (SceneManager.GetActiveScene().name == "SelectCar")
			{
				menuIsOpen = false;
			}
			if (Input.GetKeyDown(KeyCode.F6) && SceneManager.GetActiveScene().name != "SelectCar")
			{
				Search.SearchFolders();
				menuIsOpen = !menuIsOpen;
			}
			if (playerCar == null)
			{
				Search.CarSearch(ref playerCar);
			}
			if (Input.GetKeyDown(KeyCode.F3))
			{
				if (LoadExtras.currentSpawn < LoadExtras.spawnPos.Count-1) 
				{
					LoadExtras.currentSpawn++;
				}
				else
				{
					LoadExtras.currentSpawn = 0;
				}
			}
			if (Input.GetKeyDown(KeyCode.F5) && SceneManager.GetActiveScene().name != "SelectCar")
			{
				if(LoadExtras.spawnPos.Count == 0)
				{
					LoadExtras.spawnPos.Add(new Vector3(0f, 0f, 0f));
					LoadExtras.spawnRot.Add(new Vector3(0f, 0f, 0f));
					LoadExtras.currentSpawn = 0;
				}
				playerCar.getTransform.rotation = Quaternion.Euler(LoadExtras.spawnRot[LoadExtras.currentSpawn]);
				playerCar.getTransform.position = LoadExtras.spawnPos[LoadExtras.currentSpawn];
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
			if (menuIsOpen)
			{
				windowRect = GUI.Window(0, windowRect, new GUI.WindowFunction(DoMyWindow), "");
			}
		}

		void DoMyWindow(int windowID)
		{
			GUIStyle guistyle = new GUIStyle
			{
				fontSize = 28
			};
			GUIStyle guistyle4 = new GUIStyle(GUI.skin.button);
			Font myFont = (Font)Resources.Load("Arial", typeof(Font));
			guistyle4.font = myFont;
			guistyle4.fontSize = 18;
			GUIStyle guistyle5 = new GUIStyle(GUI.skin.button);
			Font myFont2 = (Font)Resources.Load("Arial", typeof(Font));
			guistyle5.font = myFont2;
			guistyle5.fontSize = 16;
			GUI.Label(new Rect(30f, 20f, 500f, 100f), "Goats Custom Map Loader", guistyle);
			if (GUI.Button(new Rect(10f, 60f, 185f, 40f), "Load Map", guistyle4))
			{
				Search.SearchFiles();
				playerCar.getTransform.position = Search.spawn;
				playerCar.getTransform.rotation = Search.rot;
				Rigidbody getRigidbody = playerCar.carX.getRigidbody;
				getRigidbody.velocity = Vector3.zero;
				getRigidbody.angularVelocity = Vector3.zero;
			}
			if (GUI.Button(new Rect(205f, 60f, 185f, 40f), "Clear Maps", guistyle4))
			{
				foreach (GameObject obj in Search.mapg0)
				{
					UnityEngine.Object.Destroy(obj);
				}
				foreach (GameObject light in LoadExtras.allLights)
				{
					UnityEngine.Object.Destroy(light);
				}
				LoadExtras.spawnPos.Clear();
				LoadExtras.spawnRot.Clear();
				Resources.UnloadUnusedAssets();
			}
			scrollHeight = Search.directoriesFiltered.Count * 60f;
			scrollPosition = GUI.BeginScrollView(new Rect(10f, 110f, 380f, 600f), scrollPosition, new Rect(0, 0, 360f, scrollHeight));
			selGridInt = GUI.SelectionGrid(new Rect(0f, 0f, 360f, scrollHeight), selGridInt, Search.directoriesFiltered.ToArray(), 1, guistyle5);
			GUI.EndScrollView();
			GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
		}

		public static RaceCar playerCar;

		public static List<GameObject> suns = new List<GameObject>();

		public GameObject currentMap;

		public static bool menuIsOpen;

		public static Rect windowRect;

		public Vector2 scrollPosition = Vector2.zero;

		public float scrollHeight;

		public List<string> maps = new List<string>();

		public static int selGridInt;
	}
}