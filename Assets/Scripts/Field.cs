using UnityEngine;

/// <summary>
/// 独楽が回転しているフィールドの境界を定義し、フィールド外への離脱を検知する。
/// このオブジェクトには Collider (IsTrigger=true) をアタッチする。
/// </summary>
public class Field : MonoBehaviour
{
    // ---- 公開メソッド ----

    /// <summary>
    /// 指定された位置がフィールド内かどうかを返す。
    /// </summary>
    public bool IsInsideField(Vector3 worldPosition)
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return false;

        return col.bounds.Contains(worldPosition);
    }
}
