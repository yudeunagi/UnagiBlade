using UnityEngine;

/// <summary>
/// 独楽を構成する一つのパーツ（上部・中間部・接地部）を表すコンポーネント。
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class TopPart : MonoBehaviour
{
    public enum PartType
    {
        Upper,  // 上部
        Middle, // 中間部
        Ground  // 接地部
    }

    [SerializeField] private PartType partType;

    /// <summary>このパーツの種別。</summary>
    public PartType Type => partType;

    /// <summary>このパーツに接続されたジョイント（null の場合はルート）。</summary>
    public Joint AttachedJoint { get; set; }

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>ジョイントを破壊し、パーツを独立させる。</summary>
    public void BreakJoint()
    {
        if (AttachedJoint != null)
        {
            Destroy(AttachedJoint);
            AttachedJoint = null;
        }
    }

    public Rigidbody Rigidbody => rb;
}
