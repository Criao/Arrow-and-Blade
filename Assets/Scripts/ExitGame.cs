using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void OnExitClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 编辑器里停止运行
        #else
        Application.Quit(); // 打包后退出游戏
        #endif
    }
}
