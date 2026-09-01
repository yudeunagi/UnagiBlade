using UnityEngine;

/// <summary>
/// 独楽を構成する一つのパーツ（上部・中間部・接地部）を表すコンポーネント。
/// 接地部（Ground）の場合は <see cref="GroundContactShapeVariant"/> を指定することで、
/// 3D モデルの Mesh と PhysicsMaterial が実行時に自動的に適用される。
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

    [Header("Ground Contact Shape (接地部のみ使用)")]
    [Tooltip("接地部に適用する形状バリアント ScriptableObject。" +
             "PartType が Ground の場合のみ有効。")]
    [SerializeField] private GroundContactShapeVariant groundContactVariant;

    // ---- 公開プロパティ ----

    /// <summary>このパーツの種別。</summary>
    public PartType Type => partType;

    /// <summary>このパーツに接続されたジョイント（null の場合はルート）。</summary>
    public Joint AttachedJoint { get; set; }

    /// <summary>適用済みの接地形状バリアント。接地部以外は null。</summary>
    public GroundContactShapeVariant GroundContactVariant => groundContactVariant;

    public Rigidbody Rigidbody => rb;

    // ---- 内部フィールド ----

    private Rigidbody rb;

    // ---- Unity ライフサイクル ----

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (partType == PartType.Ground && groundContactVariant != null)
        {
            ApplyGroundContactVariant();
        }
    }

    // ---- 公開メソッド ----

    /// <summary>ジョイントを破壊し、パーツを独立させる。</summary>
    public void BreakJoint()
    {
        if (AttachedJoint != null)
        {
            Destroy(AttachedJoint);
            AttachedJoint = null;
        }
    }

    // ---- 非公開メソッド ----

    /// <summary>
    /// <see cref="GroundContactShapeVariant"/> の Mesh と PhysicsMaterial を
    /// このゲームオブジェクトの MeshFilter・MeshCollider に適用する。
    /// </summary>
    private void ApplyGroundContactVariant()
    {
        Mesh mesh = groundContactVariant.Mesh;
        PhysicsMaterial physMat = groundContactVariant.PhysicsMaterial;

        // MeshFilter へ Mesh を適用（なければ追加）
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
        if (mesh != null) meshFilter.sharedMesh = mesh;

        // MeshCollider へ Mesh と PhysicsMaterial を適用（なければ追加）
        // MeshCollider を使用することで 3D モデルの形状に沿ったコライダーになる
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null) meshCollider = gameObject.AddComponent<MeshCollider>();
        if (mesh != null) meshCollider.sharedMesh = mesh;
        if (physMat != null) meshCollider.sharedMaterial = physMat;

        // Convex を有効にすることで Rigidbody との組み合わせが可能になる
        meshCollider.convex = true;

        Debug.Log($"[TopPart] {name} に接地形状「{groundContactVariant.VariantName}」を適用しました。");
    }
}
