using UnityEngine;

/// <summary>
/// 接地部分の形状種別。形状により摩擦・抵抗が変わり、独楽の回転持続時間に影響を与える。
/// </summary>
public enum GroundContactShape
{
    /// <summary>先端が尖っている（接触面積が小さく摩擦が少ない）。</summary>
    Sharp,
    /// <summary>先端が平らな円盤形（接触面積が大きく摩擦が多い）。</summary>
    Flat,
    /// <summary>先端が球面（摩擦は中程度）。</summary>
    Rounded
}

/// <summary>
/// 接地形状ごとの角速度減衰係数。値が大きいほど速く停止する。
/// </summary>
public static class GroundContactShapeHelper
{
    /// <summary>
    /// 指定された接地形状に対応する角速度減衰係数を返す。
    /// </summary>
    public static float GetAngularDragMultiplier(GroundContactShape shape)
    {
        switch (shape)
        {
            case GroundContactShape.Sharp:
                return 0.5f;   // 摩擦が少ない
            case GroundContactShape.Flat:
                return 2.0f;   // 摩擦が多い
            case GroundContactShape.Rounded:
            default:
                return 1.0f;   // 標準
        }
    }
}
