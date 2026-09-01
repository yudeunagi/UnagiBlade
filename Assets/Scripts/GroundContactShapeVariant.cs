using UnityEngine;

/// <summary>
/// 接地部形状の一バリアントを定義する ScriptableObject。
/// 実際の 3D モデル（Mesh）と物理マテリアル（PhysicsMaterial）をセットで保持する。
/// Assets/ScriptableObjects/ 以下に各形状ごとのアセットを作成して使用する。
/// </summary>
[CreateAssetMenu(
    fileName = "NewGroundContactShapeVariant",
    menuName = "UnagiBlade/Ground Contact Shape Variant")]
public class GroundContactShapeVariant : ScriptableObject
{
    [Header("識別情報")]
    [Tooltip("この形状バリアントの表示名（例: Sharp, Flat, Rounded）")]
    [SerializeField] private string variantName = "NewVariant";

    [Header("3D モデル")]
    [Tooltip("接地部に使用する Mesh アセット（先端形状を表現する 3D モデル）")]
    [SerializeField] private Mesh mesh;

    [Header("物理マテリアル")]
    [Tooltip("接触面の摩擦・弾性を定義する PhysicsMaterial アセット")]
    [SerializeField] private PhysicsMaterial physicsMaterial;

    // ---- 公開プロパティ ----

    /// <summary>この形状バリアントの表示名。</summary>
    public string VariantName => variantName;

    /// <summary>接地部に適用する Mesh。</summary>
    public Mesh Mesh => mesh;

    /// <summary>接地部コライダーに適用する PhysicsMaterial。</summary>
    public PhysicsMaterial PhysicsMaterial => physicsMaterial;
}
