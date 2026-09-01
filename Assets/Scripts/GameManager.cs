using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ゲーム全体を管理するクラス。
/// 2つの独楽を監視し、回転停止またはフィールド外離脱で勝敗を決定する。
/// </summary>
public class GameManager : MonoBehaviour
{
    // ---- Inspector 設定 ----

    [Header("Tops")]
    [SerializeField] private Top topA;
    [SerializeField] private Top topB;

    [Header("Field")]
    [SerializeField] private Field field;

    [Header("Settings")]
    [Tooltip("ゲーム開始前のカウントダウン時間 (秒)")]
    [SerializeField] private float countdownSeconds = 3f;

    [Tooltip("勝敗判定を行うポーリング間隔 (秒)")]
    [SerializeField] private float judgeInterval = 0.1f;

    // ---- イベント ----

    /// <summary>独楽 A が勝ったときに呼ばれる。</summary>
    public UnityEvent OnTopAWin;

    /// <summary>独楽 B が勝ったときに呼ばれる。</summary>
    public UnityEvent OnTopBWin;

    /// <summary>引き分けのときに呼ばれる。</summary>
    public UnityEvent OnDraw;

    // ---- 公開プロパティ ----

    /// <summary>現在のゲーム状態。</summary>
    public GameState State { get; private set; } = GameState.WaitingToStart;

    // ---- 内部フィールド ----

    private bool judgeCompleted;

    // ---- Unity ライフサイクル ----

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (State == GameState.Playing)
        {
            Judge();
        }
    }

    // ---- 公開メソッド ----

    /// <summary>ゲームを開始する（カウントダウン後に独楽を回す）。</summary>
    public void StartGame()
    {
        if (State != GameState.WaitingToStart) return;
        State = GameState.Countdown;
        StartCoroutine(CountdownRoutine());
    }

    // ---- 非公開メソッド ----

    private IEnumerator CountdownRoutine()
    {
        yield return new WaitForSeconds(countdownSeconds);

        topA.Initialize();
        topB.Initialize();

        State = GameState.Playing;
        Debug.Log("[GameManager] ゲーム開始！");
    }

    private void Judge()
    {
        if (judgeCompleted) return;

        bool aOut = IsOutOfField(topA);
        bool bOut = IsOutOfField(topB);
        bool aStopped = !topA.IsSpinning;
        bool bStopped = !topB.IsSpinning;

        bool aLose = aOut || aStopped;
        bool bLose = bOut || bStopped;

        if (!aLose && !bLose) return;

        judgeCompleted = true;
        State = GameState.Finished;

        if (aLose && bLose)
        {
            Debug.Log("[GameManager] 引き分け！");
            OnDraw?.Invoke();
        }
        else if (aLose)
        {
            string reason = aOut ? "フィールド外" : "回転停止";
            Debug.Log($"[GameManager] 独楽 B の勝利！（独楽 A が{reason}）");
            OnTopBWin?.Invoke();
        }
        else
        {
            string reason = bOut ? "フィールド外" : "回転停止";
            Debug.Log($"[GameManager] 独楽 A の勝利！（独楽 B が{reason}）");
            OnTopAWin?.Invoke();
        }
    }

    private bool IsOutOfField(Top top)
    {
        if (field == null) return false;
        if (top == null) return false;

        // 接地パーツの位置でフィールド外判定
        return !field.IsInsideField(top.transform.position);
    }
}

/// <summary>ゲームの進行状態。</summary>
public enum GameState
{
    WaitingToStart,
    Countdown,
    Playing,
    Finished
}
