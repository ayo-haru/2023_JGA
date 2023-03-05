/// @file	WhiteTagAndLayer.cs
/// @brief	WhiteTagAndLayerクラス
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditorInternal;
using UnityEngine;

public class WhiteTagAndLayer : IPreprocessBuildWithReport/*, IPostprocessBuildWithReport*/
{
	public int callbackOrder => 1;

	public const string fileNameTag = "tags.txt";
	public const string fileNameLayer = "layer.txt";

	public void OnPreprocessBuild(BuildReport report)
	{
		Debug.Log("ビルド前処理");

		WriteFile(InternalEditorUtility.tags, $"{Application.dataPath}/{fileNameTag}");
		WriteFile(InternalEditorUtility.layers, $"{Application.dataPath}/{fileNameLayer}");
	}

	//public void OnPostprocessBuild(BuildReport report)
	//{
	//	Debug.Log("ビルド後処理");
	//}


	void WriteFile(string[] txt, string path)
	{
		using (var fileStream = new FileStream(path, FileMode.OpenOrCreate))
		{
			fileStream.SetLength(0);
		}
		FileInfo fi = new FileInfo(path);
		using (StreamWriter sw = fi.AppendText())
		{
			foreach (string s in txt)
			{
				sw.WriteLine(s);
			}
		}
	}


	//void ReadFile()
	//{
	//	string path = Application.dataPath + "/" + fileNameTag;
	//	FileInfo fi = new FileInfo(path);
	//	try
	//	{
	//		using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
	//		{
	//			string readTxt = sr.ReadToEnd();
	//			Debug.Log(readTxt);
	//		}
	//	}
	//	catch (Exception e)
	//	{
	//		Debug.Log(e);
	//	}
	//}

}
