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
    class TexturesAndColors
    {
		static Texture2D LoadTGA(string fileName)
		{
			Texture2D result;
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				result = LoadTGA(fileStream);
			}
			return result;
		}

		static Texture2D LoadDDSManual(string ddsPath, bool flip)
		{
			Texture2D result;
			try
			{
				byte[] array = File.ReadAllBytes(ddsPath);
				if (array[4] != 124)
				{
					throw new Exception("Invalid DDS DXTn texture. Unable to read");
				}
				int height = (int)array[13] * 256 + (int)array[12];
				int width = (int)array[17] * 256 + (int)array[16];
				byte b = array[87];
				TextureFormat textureFormat = TextureFormat.DXT5;
				if (b == 49)
				{
					textureFormat = TextureFormat.DXT1;
				}
				if (b == 53)
				{
					textureFormat = TextureFormat.DXT5;
				}
				int num = 128;
				byte[] array2 = new byte[array.Length - num];
				Buffer.BlockCopy(array, num, array2, 0, array.Length - num);
				FileInfo fileInfo = new FileInfo(ddsPath);
				Texture2D texture2D = new Texture2D(width, height, textureFormat, false);
				texture2D.LoadRawTextureData(array2);
				texture2D.Apply();
				if (flip)
				{
					FlipTexture(ref texture2D);
				}

				texture2D.name = fileInfo.Name;
				result = texture2D;
			}
			catch (Exception)
			{
				Debug.LogError("Error: Could not load DDS");
				result = new Texture2D(8, 8);
			}
			return result;
		}
		static void FlipTexture(ref Texture2D original)
		{
			Texture2D flipped = new Texture2D(original.width, original.height);
			try
			{

				int xN = original.width;
				int yN = original.height;


				for (int i = 0; i < xN; i++)
				{
					for (int j = 0; j < yN; j++)
					{
						flipped.SetPixel(j, xN - i - 1, original.GetPixel(j, i));
					}
				}
				original = flipped;
				original.Apply();
			}
			catch (Exception)
			{
				Debug.LogError("Error: Could not load flipped image");
			}
		}

		static Texture2D SetNormalMap(string fn)
		{
			FileStream NormalMap = new FileStream(fn, FileMode.Open, FileAccess.Read);
			NormalMap.Seek(0, SeekOrigin.Begin);
			byte[] bytes = new byte[NormalMap.Length];
			NormalMap.Read(bytes, 0, (int)NormalMap.Length);
			NormalMap.Close();
			NormalMap.Dispose();
			Texture2D NormalTexture = new Texture2D(1, 1, TextureFormat.R8, true, true);
			NormalTexture.LoadImage(bytes);
			return NormalTexture;
		}

		public static Texture2D LoadTexture(string fn, bool normalMap = false, bool flipDDS = false)
		{
			if (!File.Exists(fn))
			{
				return null;
			}
			string format = Path.GetExtension(fn).ToLower();
			if (format == ".jpg")
			{
				if (normalMap)
				{
					return SetNormalMap(fn);
				}
				else
				{
					Texture2D texture2D2 = new Texture2D(1, 1, TextureFormat.RGBA32, true);
					texture2D2.LoadImage(File.ReadAllBytes(fn));
					return texture2D2;
				}
			}
			if (format == ".png")
			{
				if (normalMap)
				{
					return SetNormalMap(fn);
				}
				else
				{
					Texture2D texture2D2 = new Texture2D(1, 1, TextureFormat.RGBA32, true);
					texture2D2.LoadImage(File.ReadAllBytes(fn));
					return texture2D2;
				}
			}

			if (format == ".dds")
			{
				Texture2D result = LoadDDSManual(fn, flipDDS);
				if (normalMap)
				{
					result = SetNormalMap(fn);
				}
				return result;
			}
			if (format == ".tga")
			{
				Texture2D result2 = LoadTGA(fn);
				return result2;
			}
			Debug.Log("texture not supported : " + fn);
			return null;
		}

		static Texture2D LoadTGA(Stream TGAStream)
		{
			Texture2D result;
			using (BinaryReader binaryReader = new BinaryReader(TGAStream))
			{
				binaryReader.BaseStream.Seek(12L, SeekOrigin.Begin);
				short num = binaryReader.ReadInt16();
				short num2 = binaryReader.ReadInt16();
				int num3 = (int)binaryReader.ReadByte();
				binaryReader.BaseStream.Seek(1L, SeekOrigin.Current);
				Texture2D texture2D = new Texture2D((int)num, (int)num2);
				Color32[] array = new Color32[(int)(num * num2)];
				if (num3 == 32)
				{
					for (int i = 0; i < (int)(num * num2); i++)
					{
						byte b = binaryReader.ReadByte();
						byte g = binaryReader.ReadByte();
						byte r = binaryReader.ReadByte();
						byte a = binaryReader.ReadByte();
						array[i] = new Color32(r, g, b, a);
					}
				}
				else
				{
					if (num3 != 24)
					{
						throw new Exception("TGA texture had non 32/24 bit depth.");
					}
					for (int j = 0; j < (int)(num * num2); j++)
					{
						byte b2 = binaryReader.ReadByte();
						byte g2 = binaryReader.ReadByte();
						byte r2 = binaryReader.ReadByte();
						array[j] = new Color32(r2, g2, b2, 1);
					}
				}
				texture2D.SetPixels32(array);
				texture2D.Apply();
				result = texture2D;
			}
			return result;
		}
		public static Color ParseColorFromCMPS(string[] cmps, float scalar = 1f)
		{
			float r = float.Parse(cmps[1]) * scalar;
			float g = float.Parse(cmps[2]) * scalar;
			float b = float.Parse(cmps[3]) * scalar;
			return new Color(r, g, b);
		}
	}
}
