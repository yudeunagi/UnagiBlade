using UnityEngine;

/// <summary>
/// 独楽の接地パーツが接地面と衝突したとき、衝撃の大きさに応じてジョイントに力を加えるコンポーネント。
/// 大きな衝撃の場合はジョイントが外れ、パーツが分離する。
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ImpactDetector : MonoBehaviour
{
    [Tooltip("ジョイントを外すのに必要な衝撃の閾値 (N)")]
    [SerializeField] private float breakImpulseThreshold = 20f;

    private void OnCollisionEnter(Collision collision)
    {
        float impulse = collision.impulse.magnitude;
        if (impulse < breakImpulseThreshold) return;

        // 親の Top コンポーネントを取得
        Top top = GetComponentInParent<Top>();
        if (top == null) return;

        // 衝撃を接地パーツの Rigidbody に追加の力として加えることで
        // ConfigurableJoint の breakForce を超えさせる
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(collision.impulse * 2f, ForceMode.Impulse);
        }

        Debug.Log($"[ImpactDetector] {name} に大きな衝撃 ({impulse:F1}N) が加わりました。");
    }
}
