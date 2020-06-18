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
    class OBJLoader
    {
		public static GameObject LoadOBJFile(string fn)
		{
			Debug.Log("loading....");
			Debug.Log(fn);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fn);
			if (fileNameWithoutExtension.Contains("alpha"))
			{
				alphaIsEnabledOld = true;
			}
			else
			{
				alphaIsEnabledOld = false;
			}
			bool flag2 = false;
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			List<Vector2> list3 = new List<Vector2>();
			List<Vector3> list4 = new List<Vector3>();
			List<Vector3> list5 = new List<Vector3>();
			List<Vector2> list6 = new List<Vector2>();
			List<string> list7 = new List<string>();
			List<string> list8 = new List<string>();
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			List<OBJFace> list9 = new List<OBJFace>();
			string text = "";
			string text2 = "default";
			Material[] array = null;
			FileInfo fileInfo = new FileInfo(fn);
			foreach (string text3 in File.ReadAllLines(fn))
			{
				if (text3.Length > 0 && text3[0] != '#')
				{
					string text4 = text3.Trim().Replace("  ", " ");
					string[] array2 = text4.Split(new char[]
					{
						' '
					});
					string text5 = text4.Remove(0, text4.IndexOf(' ') + 1);
					switch (array2[0])
					{
						case "o":
							text2 = text5;
							if (!list8.Contains(text2))
							{
								list8.Add(text2);
							}
							break;
						case "mtllib":
							string text6 = Search.OBJGetFilePath(text5, fileInfo.Directory.FullName + Path.DirectorySeparatorChar.ToString(), fileNameWithoutExtension);
							if (text6 != null)
							{
								array = MaterialLoader.LoadMTLFile(text6, alphaIsEnabledOld);
							}
							break;
						case "usemtl":
							text = text5;
							if (!list7.Contains(text))
							{
								list7.Add(text);
							}
							if (splitByMaterial && !list8.Contains(text))
							{
								list8.Add(text);
							}
							break;
						case "v":
							list.Add(ParseVectorFromCMPS(array2));
							break;
						case "vn":
							list2.Add(ParseVectorFromCMPS(array2));
							break;
						case "vt":
							list3.Add(ParseVectorFromCMPS(array2));
							break;
						case "f":
							int[] array3 = new int[array2.Length - 1];
							for (int i = 1; i < array2.Length; i++)
							{
								string text7 = array2[i];
								int num = -1;
								int num2 = -1;
								int num3;
								if (text7.Contains("//"))
								{
									string[] array8 = text7.Split(new char[]
									{
									'/'
									});
									num3 = int.Parse(array8[0]) - 1;
									num = int.Parse(array8[2]) - 1;
								}
								else if (text7.Count(new Func<char, bool>(lol2.haha2.LoadOBJFile4)) == 2)
								{
									string[] array9 = text7.Split(new char[]
									{
									'/'
									});
									num3 = int.Parse(array9[0]) - 1;
									num2 = int.Parse(array9[1]) - 1;
									num = int.Parse(array9[2]) - 1;
								}
								else if (!text7.Contains("/"))
								{
									num3 = int.Parse(text7) - 1;
								}
								else
								{
									string[] array10 = text7.Split(new char[]
									{
									'/'
									});
									num3 = int.Parse(array10[0]) - 1;
									num2 = int.Parse(array10[1]) - 1;
								}
								string key = string.Concat(new object[]
								{
								num3,
								"|",
								num,
								"|",
								num2
								});
								if (dictionary.ContainsKey(key))
								{
									array3[i - 1] = dictionary[key];
								}
								else
								{
									array3[i - 1] = dictionary.Count;
									dictionary[key] = dictionary.Count;
									list4.Add(list[num3]);
									if (num < 0 || num > list2.Count - 1)
									{
										list5.Add(Vector3.zero);
									}
									else
									{
										flag2 = true;
										list5.Add(list2[num]);
									}
									if (num2 < 0 || num2 > list3.Count - 1)
									{
										list6.Add(Vector2.zero);
									}
									else
									{
										list6.Add(list3[num2]);
									}
								}
							}
							if (array3.Length < 5 && array3.Length >= 3)
							{
								List<OBJFace> list10 = list9;
								OBJFace objface3 = new OBJFace
								{
									materialName = text,
									indexes = new int[]
									{
									array3[0],
									array3[1],
									array3[2]
									},
									meshName = (splitByMaterial ? text : text2)
								};
								OBJFace item = objface3;
								list10.Add(item);
								if (array3.Length > 3)
								{
									List<OBJFace> list11 = list9;
									objface3 = new OBJFace
									{
										materialName = text,
										meshName = (splitByMaterial ? text : text2),
										indexes = new int[]
										{
										array3[2],
										array3[3],
										array3[0]
										}
									};
									item = objface3;
									list11.Add(item);
								}
							}
							break;
					}
				}
			}
			if (list8.Count == 0)
			{
				list8.Add("default");
			}
			GameObject gameObject = new GameObject(fileNameWithoutExtension + "new");
			GameObject result;
			using (List<string>.Enumerator enumerator = list8.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					lol3 lol = new lol3();
					lol.obj = enumerator.Current;
					lol.lol = new lol();
					GameObject gameObject2 = new GameObject(lol.obj);
					gameObject2.transform.parent = gameObject.transform;
					gameObject2.transform.localScale = new Vector3(-1f, 1f, 1f);
					Mesh mesh = new Mesh();
					mesh.name = lol.obj;
					List<Vector3> list12 = new List<Vector3>();
					List<Vector3> list13 = new List<Vector3>();
					List<Vector2> list14 = new List<Vector2>();
					List<int[]> list15 = new List<int[]>();
					Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
					lol.lol.meshMaterialNames = new List<string>();
					OBJFace[] source = list9.Where(new Func<OBJFace, bool>(lol.LoadOBJFile2)).ToArray<OBJFace>();
					using (List<string>.Enumerator enumerator2 = list7.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							lol4 lol2 = new lol4();
							lol2.mn = enumerator2.Current;
							OBJFace[] array4 = source.Where(new Func<OBJFace, bool>(lol2.LoadOBJFile5)).ToArray<OBJFace>();
							if (array4.Length != 0)
							{
								int[] array5 = new int[0];
								foreach (OBJFace objface2 in array4)
								{
									int num4 = array5.Length;
									Array.Resize<int>(ref array5, num4 + objface2.indexes.Length);
									Array.Copy(objface2.indexes, 0, array5, num4, objface2.indexes.Length);
								}
								lol.lol.meshMaterialNames.Add(lol2.mn);
								if (mesh.subMeshCount != lol.lol.meshMaterialNames.Count)
								{
									mesh.subMeshCount = lol.lol.meshMaterialNames.Count;
								}
								for (int j = 0; j < array5.Length; j++)
								{
									int num5 = array5[j];
									if (dictionary2.ContainsKey(num5))
									{
										array5[j] = dictionary2[num5];
									}
									else
									{
										list12.Add(list4[num5]);
										list13.Add(list5[num5]);
										list14.Add(list6[num5]);
										dictionary2[num5] = list12.Count - 1;
										array5[j] = dictionary2[num5];
									}
								}
								list15.Add(array5);
							}
						}
					}
					mesh.vertices = list12.ToArray();
					mesh.normals = list13.ToArray();
					mesh.uv = list14.ToArray();
					for (int k = 0; k < list15.Count; k++)
					{
						mesh.SetTriangles(list15[k], k);
					}
					if (!flag2)
					{
						mesh.RecalculateNormals();
					}
					mesh.RecalculateBounds();
					Material[] array6 = new Material[lol.lol.meshMaterialNames.Count];
					lol.i = 0;
					while (lol.i < lol.lol.meshMaterialNames.Count)
					{
						if (array == null)
						{
							Debug.Log("array null");
							array6[lol.i] = new Material(Shader.Find("HDRP/Lit"));
						}
						else
						{
							Material[] array12 = array;
							Predicate<Material> match;
							if ((match = lol.haha3) == null)
							{
								match = (lol.haha3 = new Predicate<Material>(lol.LoadOBJFile3));
							}
							Material material = Array.Find(array12, match);
							Debug.Log(material);
							if (material == null)
							{
								Debug.Log("material null");
								array6[lol.i] = new Material(Shader.Find("HDRP/Lit"));
							}
							else
							{
								array6[lol.i] = material;
							}
						}
						array6[lol.i].name = lol.lol.meshMaterialNames[lol.i];
						int i2 = lol.i;
						lol.i = i2 + 1;
					}
					meshFilter = gameObject2.AddComponent<MeshFilter>();
					meshRenderer = gameObject2.AddComponent<MeshRenderer>();
					meshRenderer.receiveShadows = true;
					meshFilter.mesh = mesh;
					meshRenderer.materials = array6;
					if (!alphaIsEnabledOld)
					{
						meshCollider = gameObject2.AddComponent<MeshCollider>();
						meshCollider.convex = false;
						meshCollider.sharedMesh = mesh;
						CarX.Material carXMaterial = gameObject2.AddComponent<CarX.Material>();
						string objectParams = gameObject2.name.Split('_')[0].ToLower();
						switch (objectParams)
						{
							case "road":
								Debug.Log("road set");
								gameObject2.AddComponent<CARXSurface>().template = CarXSurfaceManager.Get("asphalt");
								carXMaterial.SetParameters(CarX.SurfaceType.Asphalt, 1, 0.01f, 0f, 0f, 100f);
								break;
							case "kerb":
								Debug.Log("kerb set");
								gameObject2.AddComponent<CARXSurface>().template = CarXSurfaceManager.Get("kerb_racetrack");
								carXMaterial.SetParameters(CarX.SurfaceType.Asphalt, 1, 0.015f, -0.007f, 0f, 12f);
								break;
							case "sand":
								Debug.Log("sand set");
								gameObject2.AddComponent<CARXSurface>().template = CarXSurfaceManager.Get("sand");
								carXMaterial.SetParameters(CarX.SurfaceType.Sand, 1, 0.13f, 0.06f, 0.05f, 25f);
								break;
							case "snow":
								Debug.Log("snow set");
								gameObject2.AddComponent<CARXSurface>().template = CarXSurfaceManager.Get("snow");
								carXMaterial.SetParameters(CarX.SurfaceType.Snow, 2, 0.1f, -0.01f, -0.01f, 30f);
								break;
							case "grass":
								Debug.Log("grass set");
								CARXSurface surf = gameObject2.AddComponent<CARXSurface>();
								surf.template = CarXSurfaceManager.Get("grass");
								surf.template.wheelVfxData.skidmarkColor = new Color32(189, 162, 134, 78);
								surf.template.wheelVfxData.particleData = Resources.Load<CARXWheelsParticlesData>("GrassParticlesData");
								surf.templateName = "grass";
								carXMaterial.SetParameters(CarX.SurfaceType.Grass, 1, 0f, 0.05f, 0.04f, 25f);
								break;
							case "gravel":
								Debug.Log("gravel set");
								gameObject2.AddComponent<CARXSurface>().template = CarXSurfaceManager.Get("gravel");
								carXMaterial.SetParameters(CarX.SurfaceType.Sand, 2, 0.1f, -0.01f, -0.01f, 30f);
								break;
							case "icyroad":
								Debug.Log("icyroad set");
								gameObject2.AddComponent<CARXSurface>().template = CarXSurfaceManager.Get("icy_road");
								carXMaterial.SetParameters(CarX.SurfaceType.Asphalt, 0.73f, 0.025f, 0f, 0f, 30f);
								break;
							case "dirt":
								Debug.Log("dirt set");
								gameObject2.AddComponent<CARXSurface>().template = CarXSurfaceManager.Get("gravel");
								carXMaterial.SetParameters(CarX.SurfaceType.Earth, 1, 0.025f, -0.007f, 0f, 12f);
								break;
							case "nocol":
								Debug.Log("nocol set");
								meshCollider.enabled = false;
								break;
							case "rb":
								Debug.Log("rb set");
								gameObject2.transform.SetParent(null);
								meshCollider.convex = true;
								Rigidbody rb = gameObject2.AddComponent<Rigidbody>();
								break;
							default:
								gameObject2.AddComponent<CARXSurface>().template = CarXSurfaceManager.Get("asphalt");
								carXMaterial.SetParameters(CarX.SurfaceType.Asphalt, 1, 0.01f, 0f, 0f, 100f);
								Debug.Log("Surface not set for this object");
								break;
						}
					}
					gameObject2.layer = 11;
				}
				result = gameObject;
			}
			return result;
		}
		static Vector3 ParseVectorFromCMPS(string[] cmps)
		{
			float x = float.Parse(cmps[1]);
			float y = float.Parse(cmps[2]);
			if (cmps.Length == 4)
			{
				float z = float.Parse(cmps[3]);
				return new Vector3(x, y, z);
			}
			return new Vector2(x, y);
		}

		private struct OBJFace
		{
			public string materialName;

			public string meshName;

			public int[] indexes;
		}

		[CompilerGenerated]
		[Serializable]
		private sealed class lol2
		{
			static lol2()
			{
			}

			public lol2()
			{
			}

			internal bool LoadOBJFile4(char x)
			{
				return x == '/';
			}

			public static readonly lol2 haha2 = new lol2();
		}

		[CompilerGenerated]
		private sealed class lol
		{
			public lol()
			{
			}

			public List<string> meshMaterialNames;
		}

		[CompilerGenerated]
		private sealed class lol3
		{
			public lol3()
			{
			}

			internal bool LoadOBJFile2(OBJFace x)
			{
				return x.meshName == obj;
			}

			internal bool LoadOBJFile3(Material x)
			{
				return x.name == lol.meshMaterialNames[i];
			}

			public string obj;

			public lol lol;

			public int i;

			public Predicate<Material> haha3;
		}

		[CompilerGenerated]
		private sealed class lol4
		{
			public lol4()
			{
			}

			internal bool LoadOBJFile5(OBJFace x)
			{
				return x.materialName == mn;
			}

			public string mn;
		}

		public static MeshCollider meshCollider;

		public static MeshRenderer meshRenderer;

		public static MeshFilter meshFilter;

		private static bool alphaIsEnabledOld;

		public static bool splitByMaterial = false;
	}
}
