using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 主菜单管理器，处理主菜单的按钮功能
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    /// <summary>
    /// 开始游戏，加载RPG场景
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("RPG");
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
