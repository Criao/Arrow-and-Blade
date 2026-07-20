using UnityEngine;

/// <summary>
/// NPC角色数据ScriptableObject，存储角色名称和头像
/// </summary>
[CreateAssetMenu(fileName = "ActorSO", menuName = "Dialogue/NPC")]
public class ActorSo : ScriptableObject
{
    public string actorName; // 角色名称
    public Sprite portrait; // 角色头像
    [Header("Dialogue Audio")]
    public bool isPlayerActor;
    public string voiceClipName = "npc_talk_blip_01";
    [Range(0f, 1f)] public float voiceVolume = 0.55f;

}
