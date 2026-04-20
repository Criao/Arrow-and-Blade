using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家装备切换类，负责在近战和弓箭模式之间切换
/// </summary>
public class PlayerChangeEquipment : MonoBehaviour
{
    [SerializeField] private Player_Combat combat; // 近战战斗组件
    [SerializeField] private Player_Bow bow; // 弓箭战斗组件

    private void Update()
    {
        // 按下切换装备键时，切换弓箭模式的启用状态
        if (Input.GetButtonDown("ChangeEquipment"))
        {
            bow.enabled = !bow.enabled;
        }
    }
}
