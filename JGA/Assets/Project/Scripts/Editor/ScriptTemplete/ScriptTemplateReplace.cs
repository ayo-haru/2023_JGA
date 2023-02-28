﻿/* =========================================================================

	ScriptTemplateReplace.cs
	Copyright(c) R-Koubou

   ======================================================================== */

/*
	MIT License

	Copyright (c) 2017 R-Koubou

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.
*/

using System;
using System.IO;
using System.Text.RegularExpressions;

using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

/**
 * Editor extention for ScriptTemplate.
 *
 * Template files Location:
 * win: $(UNITY)/Resources/ScriptTemplate/
 * mac: Unity.app/Contents/Resources/ScriptTemplates
 *
 * see also https://support.unity3d.com/hc/en-us/articles/210223733-How-to-customize-Unity-script-templates
 *
 * @Editer	:Sakai Ryotaro
 * @Detail	:
 * 2023/02/08 - OnPostprocessAllAssets関数にて、
 *				拡張子判定時に、「ファイル名に含まれる場合」から「ファイル名の末尾に含まれる場合」に変更。
 *				自分の名前を「あらかじめ静的変数で定義」する方法から「テキストファイルから取得」する方法に変更。
 * 2023/02/17 - ファイルが見つからない場合の例外処理追加
 */

public class ScriptTemplateReplace : UnityEditor.AssetPostprocessor
{
	/** using for "Your Company" replacement */
	static readonly string MY_COMPANY = "Your Company";

	/** using for "com.example.myapp" replacement */
	static readonly string NAMESPACE = "com.example.myapp";

	/** keyword list */
	static string[] keys = new[]
	{
		"YEAR",             // YYYY
		"DATE",             // YYYY/MM/dd
		"MY_NAME",          // YOUR_NAME
		"MY_COMPANY",       // YOUR_COMPANY
		"NAMESPACE",        // C# Namespace
	};

	//static private string directory;

	/**
	 *  Implementation of Replacemet
	 */
	static string ReplaceText(string key, string text)
	{
		switch (key)
		{
			case "YEAR":
				{
					text = Regex.Replace(text, "#" + key + "#", DateTime.Now.Year.ToString());
				}
				break;

			case "DATE":
				{
					string date = DateTime.Now.ToString("yyyy/MM/dd");
					text = Regex.Replace(text, "#" + key + "#", date);
				}
				break;

			case "MY_NAME":
				{
					string name = "";
					// 候補１：UnityIDのユーザー名を取得
					{
						//name = CloudProjectSettings.userName;
					}
					// 候補２：PCのログオンユーザー名を取得
					{
						//name = Environment.UserName;
					}
					// 候補３：あらかじめテキストファイルで定義した名前を取得
					{
						TextAsset textasset = new TextAsset();
						try
						{
							textasset = AssetDatabase.LoadMainAssetAtPath("Assets/UserName.txt") as TextAsset;
							name = textasset.text;
						}
						catch (System.Exception e)
						{
							Debug.LogWarning($"UserName.txtがAssetsファイル内に見つかりませんでした。 : {e}"); //例外が発生したら警告メッセージを表示している。
						}
					}
					// 候補４：ファイル生成時のディレクトリ名を取得
					{
						//name = directory;
					}
					if (name == "" || name == null) name = "YOUR_NAME";
					text = Regex.Replace(text, "#" + key + "#", name);
				}
				break;

			case "MY_COMPANY":
				{
					text = Regex.Replace(text, "#" + key + "#", MY_COMPANY);
				}
				break;

			case "NAMESPACE":
				{
					text = Regex.Replace(text, "#" + key + "#", NAMESPACE);
				}
				break;

			default:
				break;
		}

		return text;
	}

	/**
	 * It called when assets importing.
	 */
	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
	{
		foreach (var path in importedAssets)
		{
			if (path.EndsWith(".cs")
				|| path.EndsWith(".js")
				|| path.EndsWith(".boo")
				|| path.EndsWith(".shader")
				|| path.EndsWith(".compute"))
			{
				for (int i = 0; i < keys.Length; i++)
				{
					var key = keys[i];

					var text = System.IO.File.ReadAllText(path);
					if (text.Contains("#" + key + "#"))
					{
						//directory = Path.GetDirectoryName(path);
						text = ReplaceText(key, text);

						StreamWriter writer = new StreamWriter(path, false, new System.Text.UTF8Encoding(false, false));
						writer.Write(text);
						writer.Close();

						AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
					}
				}
			}
		}
	}
}
