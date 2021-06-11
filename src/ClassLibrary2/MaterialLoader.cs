using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Material = UnityEngine.Material;

namespace MapMod
{
    class MaterialLoader
    {
		public static Material[] LoadMTLFile(string fn, bool alphaIsEnabledOld)
		{
			Material material = null;
			List<Material> list = new List<Material>();
			FileInfo fileInfo = new FileInfo(fn);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fn);
			string basePath = fileInfo.Directory.FullName + Path.DirectorySeparatorChar.ToString();
			string[] array = File.ReadAllLines(fn);
			for (int i = 0; i < array.Length; i++)
			{
				string mtlText = array[i].Trim().Replace("  ", " ");
				LoadArgs(mtlText, out string modifierType, out string modifierValue, out string texName, out string texType);
				string[] array2 = mtlText.Split(new char[]
				{
					' '
				});
				string path = mtlText.Remove(0, mtlText.IndexOf(' ') + 1);
				string texture = Search.OBJGetFilePath(texName, basePath, fileNameWithoutExtension);
				string alphaSwitch = path.Split('_')[0].ToLower();
				switch (texType)
				{
					case "newmtl":
						if (alphaSwitch == "alpha")
						{
							Debug.Log("alpha is enabled");
							alphaIsEnabled = true;
						}
						else
						{
							alphaIsEnabled = false;
						}
						if (alphaIsEnabled || alphaIsEnabledOld)
						{
							if (material != null)
							{
								list.Add(material);
							}
							material = new Material(alphaObjectMeshRenderer.material);
							material.name = path;
						}
						else
						{
							if (material != null)
							{
								list.Add(material);
							}
							material = new Material(Shader.Find("HDRP/Lit"));
							material.name = path;
						}
						break;
					case "map_Kd":
						if (texture != null)
						{
							material.SetTexture("_BaseColorMap", TexturesAndColors.LoadTexture(texture, false, alphaIsEnabled || alphaIsEnabledOld));
							material.SetColor("_BaseColor", Color.white);
						}
						break;
					case "Kd":
						material.SetColor("_BaseColor", TexturesAndColors.ParseColorFromCMPS(array2, 1f));
						break;
					case "d":
						float num = float.Parse(array2[1]);
						if (num < 1f)
						{
							Color color = material.color;
							color.a = num;
							material.SetColor("_BaseColor", color);
							material.SetInt("_DoubleSidedEnable", 1);
							material.SetOverrideTag("RenderType", "TransparentCutout");
							material.SetInt("_ZTestGBuffer", (int)UnityEngine.Rendering.CompareFunction.Equal);
							material.SetInt("_CullMode", (int)UnityEngine.Rendering.CullMode.Off);
							material.SetInt("_CullModeForward", (int)UnityEngine.Rendering.CullMode.Back);
							material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
							material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
							material.SetInt("_ZWrite", 1);
							material.SetInt("_ZTestGBuffer", (int)UnityEngine.Rendering.CompareFunction.Equal);
							material.EnableKeyword("_ALPHATEST_ON");
							material.EnableKeyword("_DOUBLESIDED_ON");
							material.DisableKeyword("_BLENDMODE_ALPHA");
							material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
							material.renderQueue = 3000;
						}
						break;
					case "map_d":
						if (texture != null)
						{
							/*material.SetOverrideTag("RenderType", "TransparentCutout");
							Color color = material.color;
							material.SetColor("_BaseColor", color);
							material.SetTexture("_BaseColorMap", LoadTexture(texture, false));
							material.SetInt("_DoubleSidedEnable", 1);
							material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
							material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
							material.SetInt("_AlphaSrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
							material.SetInt("_AlphaDstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
							material.SetInt("_Mode", 0);
							material.SetInt("_ZWrite", 0);
							material.SetInt("_ZTestDepthEqualForOpaque", 4);
							material.SetInt("_ZTestGBuffer", 4);
							material.SetInt("_ZTestModeDistortion", 4);
							material.SetInt("_CullMode", (int)UnityEngine.Rendering.CullMode.Back);
							material.SetInt("_CullModeForward", (int)UnityEngine.Rendering.CullMode.Back);
							material.EnableKeyword("_BLENDMODE_ALPHA");
							material.EnableKeyword("_ENABLE_FOG_ON_TRANSPARENT");
							material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
							material.DisableKeyword("_ALPHABLEND_ON");
							material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
							material.SetFloat("_AlphaCutoffEnable", 1f);
							material.SetFloat("_AlphaCutoff", 0.5f);
							material.SetFloat("_Cutoff", 0.5f);
							material.renderQueue = 3000;*/
						}
						break;
					case "Ks":
						material.SetColor("_SpecularColor", TexturesAndColors.ParseColorFromCMPS(array2, 1f));
						material.EnableKeyword("_MATERIAL_FEATURE_SPECULAR_COLOR");
						break;
					case "map_Ks":
						if (texture != null)
						{
							material.SetTexture("_SpecularColorMap", TexturesAndColors.LoadTexture(texture, false, alphaIsEnabled || alphaIsEnabledOld));
						}
						material.EnableKeyword("_MATERIAL_FEATURE_SPECULAR_COLOR");
						break;
					case "Ka":
						float num2 = float.Parse(array2[1]);
						if (num2 < 1f)
						{
							material.SetFloat("_Metallic", float.Parse(array2[1]));
						}
						break;
					case "Ns":
						float num3 = float.Parse(array2[1]);
						num3 /= 1000f;
						material.SetFloat("_Smoothness", num3);
						break;
					case "Ni":
						float num4 = float.Parse(array2[1]);
						if (num4 > 2.5f)
						{
							num4 = 2.5f;
						}
						if (num4 < 1f)
						{
							num4 = 1f;
						}
						material.SetFloat("_Ior", num4);
						break;
					case "map_Bump":
						if (texture != null)
						{
							material.EnableKeyword("_NORMALMAP");
							material.EnableKeyword("_NORMALMAP_TANGENT_SPACE");
							material.SetTexture("_NormalMap", TexturesAndColors.LoadTexture(texture, true, alphaIsEnabled || alphaIsEnabledOld));
							if (modifierType != "")
							{
								if (float.Parse(modifierValue) > 8)
								{
									modifierValue = "8";
								}
								material.SetFloat("_NormalScale", float.Parse(modifierValue));
							}
						}
						break;
				}
			}
			if (material != null)
			{
				list.Add(material);
				Debug.Log(list);
			}
			return list.ToArray();
		}
		public static void LoadAlphaMaterial()
		{
			switch (SceneManager.GetActiveScene().name)
			{
				case "Fiorano2":
					alphaObject = GameObject.Find("Azalea_Leaves_1_Mat001_LOD0");
					break;
				case "Japan":
					alphaObject = GameObject.Find("tree_array_LOD0");
					break;
				case "LosAngeles":
					alphaObject = GameObject.Find("Bush_004");
					break;
				case "Parking":
					alphaObject = GameObject.Find("leaves_LOD0");
					break;
				case "Petersburg":
					alphaObject = GameObject.Find("leaves_LOD0");
					break;
				case "RedRing":
					alphaObject = GameObject.Find("leaves_LOD0");
					break;
				case "RedRock":
					alphaObject = GameObject.Find("JoshuaTree06");
					break;
				case "SilverStone":
					alphaObject = GameObject.Find("tree_LOD3");
					break;
				case "Winterfell":
					alphaObject = GameObject.Find("Branches_LOD0");
					break;
				case "Irwindale":
					alphaObject = GameObject.Find("leaves_LOD0");
					break;
				case "RedRing_Winter":
					alphaObject = GameObject.Find("leaves_LOD0");
					break;
				case "Bathurst":
					alphaObject = GameObject.Find("AfricanOliveLeaves_LOD0");
					break;
				case "Ebisu":
					alphaObject = GameObject.Find("tile0_Forest_Part_01");
					break;
				case "Airfield":
					alphaObject = GameObject.Find("Fence");
					break;
				default:
					alphaObject = GameObject.Find("leaves_LOD0");
					break;
			}
			if (alphaObject != null)
			{
				Debug.Log(alphaObject.name);
				alphaObjectClone = UnityEngine.Object.Instantiate(alphaObject, new Vector3(1000000000f, 0, 0), Quaternion.identity, null);
				alphaObjectMeshRenderer = alphaObjectClone.GetComponent<MeshRenderer>();
			}
		}

		static void LoadArgs(string mtlLine, out string modifierType, out string modifierValue, out string texName, out string texType)
		{
			string[] parts = mtlLine.Split();
			bool readingName = false;
			bool skipNext = false;
			modifierType = "";
			modifierValue = "";
			texName = "";
			texType = "";
			for (int i = 0; i < parts.Length; i++)
			{
				if (skipNext)
				{
					skipNext = false;
					continue;
				}
				if (readingName)
				{
					texName += " " + parts[i];
					continue;
				}
				if (i == 0)
				{
					texType = parts[i];
					continue;
				}
				if (parts[i].StartsWith("-"))
				{
					skipNext = true;
					modifierType = parts[i];
					modifierValue = parts[i + 1];
					continue;
				}
				else
				{
					readingName = true;
					texName = parts[i];
					continue;
				}
			}
		}

		public static GameObject alphaObject;

		public static GameObject alphaObjectClone;

		public static MeshRenderer alphaObjectMeshRenderer;

		public static bool alphaIsEnabled;
	}
}
