using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace MK.Utilities
{
	public class TextureArrayCreator : ScriptableWizard
	{
		[MenuItem("Window/Texture Array Creator")]
		public static void ShowWindow()
		{
			ScriptableWizard.DisplayWizard<TextureArrayCreator>("Create Texture Array", "Build Asset");
		}

		public string path = "Assets/";
		public string filename = "TextureArray";
		public bool mipmap = true;
		public bool normal = false;

		public List<Texture2D> textures = new List<Texture2D>();

		private ReorderableList list;

		void OnWizardCreate()
		{
			CompileArray(textures, path, filename);
		}

		private void CompileArray(List<Texture2D> textures, string path, string filename)
		{
			if(textures == null || textures.Count == 0)
			{
				Debug.LogError("No textures assigned");
				return;
			}

			Texture2D sample = textures[0];
			Texture2DArray textureArray = new Texture2DArray(sample.width, sample.height, textures.Count, TextureFormat.ARGB32, mipmap, normal);
			textureArray.filterMode = sample.filterMode;
			textureArray.wrapMode = sample.wrapMode;

			for (int i = 0; i < textures.Count; i++)
			{
				Texture2D tex = textures[i];
				// Graphics.CopyTexture(tex, 0, textureArray, i);
				textureArray.SetPixels32(tex.GetPixels32(0), i, 0);
			}
			textureArray.Apply();
			
			string uri = path + filename+".asset";
			AssetDatabase.CreateAsset(textureArray, uri);
			Debug.Log("Saved asset to " + uri);
		}
	}
}
