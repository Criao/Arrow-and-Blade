using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

/// <summary>
/// 技能树界面切换类，处理技能树UI的打开和关闭
/// </summary>
public class ToggleSkillTree : MonoBehaviour
{
    [SerializeField] private CanvasGroup statsCanvas; // 技能树画布组
    private bool skillTreeOpen = false; // 技能树是否打开

    private void Update()
    {
        if (Input.GetButtonDown("ToggleSkillTree"))
        {
            if (skillTreeOpen)
            {
                // 关闭技能树
                Time.timeScale = 1;
                statsCanvas.alpha = 0;
                statsCanvas.blocksRaycasts = false;
                skillTreeOpen = false;
            }
            else
            {
                // 打开技能树
                Time.timeScale = 0;
                statsCanvas.alpha = 1;
                statsCanvas.blocksRaycasts = true;
                skillTreeOpen = true;
            }
        }
    }
}
