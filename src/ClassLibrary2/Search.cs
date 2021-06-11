using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using UnityEngine;

namespace MapMod
{
    class Search
    {
		public static void CarSearch(ref RaceCar playerCar)
		{
			allCars = UnityEngine.Object.FindObjectsOfType<RaceCar>();
			for (int i = 0; i < allCars.Length; i++)
			{
				if (!allCars[i].isNetworkCar)
				{
					playerCar = allCars[i];
					return;
				}
			}
		}

		public static string OBJGetFilePath(string path, string basePath, string fileName)
		{
			string[] array = searchPaths;
			for (int i = 0; i < array.Length; i++)
			{
				string str = array[i].Replace("%FileName%", fileName);
				if (File.Exists(basePath + str + path))
				{
					return basePath + str + path;
				}
				if (File.Exists(path))
				{
					return path;
				}
			}
			return null;
		}

		public static void SearchFolders()
		{
			directoriesFiltered.Clear();
			MaterialLoader.LoadAlphaMaterial();
			directories = Directory.GetDirectories(Paths.PluginPath, "Maps", SearchOption.AllDirectories);
			foreach (string directory in directories)
			{
				directories2 = Directory.GetDirectories(directory);
				foreach (string directory2 in directories2)
				{
					string folder = Path.GetFileName(directory2);
					Debug.Log(directory2);
					if (!directoriesFiltered.Contains(folder))
					{
						directoriesFiltered.Add(folder);
					}
				}
			}
		}

		public static void SearchFiles(bool loadExtrasOnly = false)
		{
			files = Directory.EnumerateFiles(directories2[Main.selGridInt], "*.obj*", SearchOption.AllDirectories).ToArray();
			for (int i = 0; i < files.Length; i++)
			{
				if (files[i].Contains(".objdata") || loadExtrasOnly)
				{
					try
					{
						Debug.Log("Loading extras");
						ExtrasLoader.LoadExtras(files[i]);
						spawn = ExtrasLoader.spawnPos[ExtrasLoader.currentSpawn];
						rot = Quaternion.Euler(ExtrasLoader.spawnRot[ExtrasLoader.currentSpawn]);
					}
					catch
					{
						Debug.Log("Couldn't load extras");
					}
					loadExtrasOnly = false;
				}
				else
				{
					mapGos.Add(OBJLoader.LoadOBJFile(files[i]));
				}
			}
			Resources.UnloadUnusedAssets();
		}

		public static string[] searchPaths = new string[]
		{
			"",
			"%FileName%_Textures" + Path.DirectorySeparatorChar.ToString()
		};

		public static string[] directories;

		public static string[] directories2;

		public static List<string> directoriesFiltered = new List<string>();

		public static RaceCar[] allCars;

		public static string[] files;

		public static List<GameObject> mapGos = new List<GameObject>();

		public static Vector3 spawn = new Vector3(0f, 0f, 0f);

		public static Quaternion rot = Quaternion.LookRotation(new Vector3(0f, 0f, 0f));
	}
}
