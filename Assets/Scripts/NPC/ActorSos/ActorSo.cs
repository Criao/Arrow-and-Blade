using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC角色数据ScriptableObject，存储角色名称和头像
/// </summary>
[CreateAssetMenu(fileName = "ActorSO", menuName = "Dialogue/NPC")]
public class ActorSo : ScriptableObject
{
    public string actorName; // 角色名称
    public Sprite portrait; // 角色头像

}
