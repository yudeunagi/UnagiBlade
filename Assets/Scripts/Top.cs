using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 独楽全体を管理するコンポーネント。
/// 上部・中間部・接地部の3パーツをジョイントで連結し、回転状態・破壊判定・フィールド外判定を行う。
/// </summary>
public class Top : MonoBehaviour
{
    // ---- Inspector 設定 ----

    [Header("Parts")]
    [SerializeField] private TopPart upperPart;
    [SerializeField] private TopPart middlePart;
    [SerializeField] private TopPart groundPart;

    [Header("Spin Settings")]
    [Tooltip("初期回転速度 (rad/s)")]
    [SerializeField] private float initialAngularSpeed = 30f;

    [Tooltip("回転停止とみなす角速度の閾値 (rad/s)")]
    [SerializeField] private float stoppedAngularSpeedThreshold = 1f;

    [Header("Ground Contact")]
    [SerializeField] private GroundContactShape contactShape = GroundContactShape.Rounded;

    [Header("Joint Break Force")]
    [Tooltip("この力を超えるとジョイントが外れる (N)")]
    [SerializeField] private float jointBreakForce = 50f;

    // ---- 公開プロパティ ----

    /// <summary>この独楽がまだ回転中かどうか。</summary>
    public bool IsSpinning { get; private set; }

    /// <summary>この独楽が破壊されているかどうか（少なくとも1ジョイントが外れた）。</summary>
    public bool IsBroken { get; private set; }

    /// <summary>接地形状。</summary>
    public GroundContactShape ContactShape => contactShape;

    // ---- 内部フィールド ----

    private List<ConfigurableJoint> joints = new List<ConfigurableJoint>();
    private Rigidbody groundRigidbody;
    private bool initialized;

    // ---- Unity ライフサイクル ----

    private void Awake()
    {
        groundRigidbody = groundPart != null ? groundPart.Rigidbody : null;
    }

    private void Start()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        if (!initialized) return;

        CheckSpinning();
        CheckJointBreak();
    }

    // ---- 公開メソッド ----

    /// <summary>
    /// 独楽を初期化し、ジョイントを設定して回転を開始する。
    /// </summary>
    public void Initialize()
    {
        if (initialized) return;

        SetupJoints();
        ApplyGroundContactDrag();
        StartSpin();

        IsSpinning = true;
        initialized = true;
    }

    /// <summary>
    /// 強制的に回転を止める（デバッグ・テスト用）。
    /// </summary>
    public void ForceStop()
    {
        if (groundRigidbody != null)
        {
            groundRigidbody.angularVelocity = Vector3.zero;
            groundRigidbody.linearVelocity = Vector3.zero;
        }

        IsSpinning = false;
    }

    // ---- 非公開メソッド ----

    private void SetupJoints()
    {
        // 中間部 → 接地部 のジョイント
        if (middlePart != null && groundPart != null)
        {
            var joint = CreateJoint(middlePart.gameObject, groundPart.Rigidbody);
            groundPart.AttachedJoint = joint;
            joints.Add(joint);
        }

        // 上部 → 中間部 のジョイント
        if (upperPart != null && middlePart != null)
        {
            var joint = CreateJoint(upperPart.gameObject, middlePart.Rigidbody);
            middlePart.AttachedJoint = joint;
            joints.Add(joint);
        }
    }

    private ConfigurableJoint CreateJoint(GameObject host, Rigidbody connectedBody)
    {
        var joint = host.AddComponent<ConfigurableJoint>();
        joint.connectedBody = connectedBody;

        // 位置をロック（パーツが離れないよう固定）
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        // 回転を許可（独楽として一体回転）
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;

        // 破壊閾値
        joint.breakForce = jointBreakForce;
        joint.breakTorque = jointBreakForce;

        return joint;
    }

    private void ApplyGroundContactDrag()
    {
        if (groundRigidbody == null) return;

        float multiplier = GroundContactShapeHelper.GetAngularDragMultiplier(contactShape);
        groundRigidbody.angularDamping = multiplier;
    }

    private void StartSpin()
    {
        if (groundRigidbody == null) return;

        // Y軸を中心に回転
        groundRigidbody.angularVelocity = Vector3.up * initialAngularSpeed;
    }

    private void CheckSpinning()
    {
        if (!IsSpinning) return;
        if (groundRigidbody == null) return;

        if (groundRigidbody.angularVelocity.magnitude < stoppedAngularSpeedThreshold)
        {
            IsSpinning = false;
        }
    }

    private void CheckJointBreak()
    {
        if (IsBroken) return;

        foreach (var joint in joints)
        {
            if (joint == null)
            {
                IsBroken = true;
                return;
            }
        }
    }

    // ---- Unity イベント ----

    private void OnJointBreak(float breakForce)
    {
        IsBroken = true;
        Debug.Log($"[Top] {name} のジョイントが外れました (force={breakForce:F1}N)");
    }
}
