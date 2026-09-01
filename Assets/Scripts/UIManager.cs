using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI を管理するクラス。ゲーム状態に応じてメッセージパネルを表示する。
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text resultText;
    [SerializeField] private Text countdownText;

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    private GameState lastState;

    private void Awake()
    {
        if (resultPanel != null) resultPanel.SetActive(false);
        if (countdownText != null) countdownText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameManager == null) return;

        GameState currentState = gameManager.State;
        if (currentState == lastState) return;
        lastState = currentState;

        switch (currentState)
        {
            case GameState.Countdown:
                if (countdownText != null) countdownText.gameObject.SetActive(true);
                break;

            case GameState.Playing:
                if (countdownText != null) countdownText.gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>独楽 A の勝利メッセージを表示する。</summary>
    public void ShowTopAWin()
    {
        ShowResult("独楽 A の勝利！");
    }

    /// <summary>独楽 B の勝利メッセージを表示する。</summary>
    public void ShowTopBWin()
    {
        ShowResult("独楽 B の勝利！");
    }

    /// <summary>引き分けメッセージを表示する。</summary>
    public void ShowDraw()
    {
        ShowResult("引き分け！");
    }

    // ---- 非公開メソッド ----

    private void ShowResult(string message)
    {
        if (resultText != null) resultText.text = message;
        if (resultPanel != null) resultPanel.SetActive(true);
    }
}
