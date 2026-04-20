using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 退出游戏按钮功能
/// </summary>
public class ExitGame : MonoBehaviour
{
    /// <summary>
    /// 退出按钮点击事件
    /// </summary>
    public void OnExitClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
