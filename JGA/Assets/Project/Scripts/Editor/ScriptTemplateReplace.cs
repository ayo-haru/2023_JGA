/* =========================================================================

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

/**
 * Editor extention for ScriptTemplate.
 *
 * Template files Location:
 * win: $(UNITY)/Resources/ScriptTemplate/
 * mac: Unity.app/Contents/Resources/ScriptTemplates
 *
 * see also https://support.unity3d.com/hc/en-us/articles/210223733-How-to-customize-Unity-script-templates
 *
 */
public class ScriptTemplateReplace : UnityEditor.AssetPostprocessor
{
    /** using for "#NAME#" replacement */
    static readonly string MY_NAME = "Your Name";

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
                    text = Regex.Replace(text, "#" + key + "#", MY_NAME);
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

                    var text = File.ReadAllText(path);
                    if (text.Contains("#" + key + "#"))
                    {
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
