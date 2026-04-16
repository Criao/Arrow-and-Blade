using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // 开始游戏，加载RPG场景
    public void StartGame()
    {
        SceneManager.LoadScene("RPG"); // 如果要加载RPG2，改成 "RPG2"
    }

    // 退出游戏
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
