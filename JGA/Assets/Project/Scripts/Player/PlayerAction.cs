//=============================================================================
// @File	: [PlayerAction.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/07	スクリプト作成
//=============================================================================
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
	protected PlayerManager _playerManager;
	public virtual void AwakeState(PlayerManager pm) { _playerManager = pm; }
	public virtual void StartState() { }
	public virtual void FixedUpdateState() { }
	public virtual void UpdateState() { }
}
