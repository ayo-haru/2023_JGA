//=============================================================================
// @File	: [PlayerMove.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/05	スクリプト作成
//=============================================================================
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : PlayerAction
{
	[SerializeField] private Collider InteractCollision;        // 掴んでいるオブジェクト：コリジョン

	[SerializeField] private HashSet<Collider> WithinRange = new HashSet<Collider>();  // インタラクト範囲内にあるオブジェクトリスト
	public List<string> InteractObjects = new List<string>();

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	public override void AwakeState(PlayerManager pm)
	{
		base.AwakeState(pm);

	}

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	public override void StartState()
	{

	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	public override void FixedUpdateState()
	{
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	public override void UpdateState()
	{
		if (InteractObjects.Count != WithinRange.Count)
		{
			InteractObjects.Clear();
			foreach (Collider c in WithinRange)
			{
				InteractObjects.Add(c.name);
			}
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		// Playerと掴んでいるオブジェクトが接触していると、ぶっ飛ぶので離す
		if (InteractCollision != null && InteractCollision == collision.collider /*&& (_IsHold || _IsDrag)*/)
			collision.transform.localPosition += Vector3.forward / 10;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Interact"))
			return;
		if ((other.name.Contains("Corn") ||
			other.name.Contains("CardBoard"))
			&& other as SphereCollider)
			return;

		// 範囲内のオブジェクトリストに追加
		WithinRange.Add(other);

		if (WithinRange.Count == 1 && other.TryGetComponent(out Outline outline))
			outline.enabled = true;
	}

	private void OnTriggerStay(Collider other)
	{
		if (!other.CompareTag("Interact"))
			return;
		if ((other.name.Contains("Corn") ||
			other.name.Contains("CardBoard"))
			&& other as SphereCollider)
			return;

		// 範囲内のオブジェクトリストに追加
		WithinRange.Add(other);
	}

	private void OnTriggerExit(Collider other)
	{
		// 範囲内のオブジェクトリストから削除
		WithinRange.Remove(other);

		if (other.TryGetComponent(out Outline outline))
			outline.enabled = false;
	}

}
