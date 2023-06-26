//=============================================================================
// @File	: [Rope.cs]
// @Brief	: ロープ
// @Author	: OgusuYuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/04	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
	[SerializeField,Header("ロープの質点")] private Transform[] ropePoint;
	[SerializeField, Header("ropePoint表示する場合はチェック入れてください")] private bool bShowRope = false;
	LineRenderer line;
	[SerializeField, Header("ロープ用のマテリアル")] private Material lineMaterial;
	[SerializeField, Header("ロープの太さ"), Range(0.1f, 1.0f)] private float lineWidth = 0.5f;
	[SerializeField, Header("ロープの角の丸み"), Range(0, 10)] private int lineNumCapVertices = 10;

	private void OnValidate()
	{
		if(line)line.startWidth = line.endWidth = lineWidth;
		if (line) line.material = lineMaterial;
		if(line) line.numCapVertices = lineNumCapVertices;

		if (ropePoint == null) return;
		for (int i = 0; i < ropePoint.Length; ++i)
		{
			ropePoint[i].gameObject.GetComponent<MeshRenderer>().enabled = bShowRope;
		}
	}
	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		//LineRendererの設定
		line = GetComponent<LineRenderer>();
		if (!line) return;
		line.positionCount = ropePoint.Length;
		line.useWorldSpace = true;
		line.material = lineMaterial;
		line.startWidth = line.endWidth = lineWidth;
		line.numCapVertices = lineNumCapVertices;

		//質点の表示設定・位置設定
		for (int i = 0; i < ropePoint.Length; ++i)
		{
#if UNITY_EDITOR
			ropePoint[i].gameObject.GetComponent<MeshRenderer>().enabled = bShowRope;
#else
			ropePoint[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
#endif
		}
	}
#if false
	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		
	}
#endif
	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
		if (!line || ropePoint == null ? true : ropePoint.Length <= 0) return;
		//LineRendererの位置更新
		for(int i = 0; i < ropePoint.Length; ++i)
		{
			line.SetPosition(i, transform.TransformPoint(ropePoint[i].localPosition));
		}
	}
#if false
	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}
#endif
}
