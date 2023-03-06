//=============================================================================
// @File	: [AddressableLoader.cs]
// @Brief	: アドレスから検索する
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/02	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableLoader<T> : MonoBehaviour
{
    private static T result;


    /// <summary>
    /// アセットバンドルのローダー
    /// </summary>
    /// <param name="_assetPath">読み込みのパス</param>
    /// <returns></returns>
    public static T Load(string _assetPath) {
        // アドレッサブルの読み込み
        // WaitForCompletionは読み込みが終わるまで待ってくれる
        var handle = Addressables.LoadAssetAsync<T>(_assetPath).WaitForCompletion();

        return handle;
    }
}
