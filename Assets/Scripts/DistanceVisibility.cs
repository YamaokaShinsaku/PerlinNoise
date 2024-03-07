using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceVisibility : MonoBehaviour
{
    public Transform target; // カメラのTransform
    public float visibilityDistance = 10f; // オブジェクトが表示される距離

    void Update()
    {
        // カメラとこのオブジェクトの距離を計算
        float distance = Vector3.Distance(transform.position, target.position);

        // カメラとの距離が指定した表示距離より近い場合はオブジェクトを表示、それ以外は非表示にする
        if (distance < visibilityDistance)
        {
            SetVisibility(true);
        }
        else
        {
            SetVisibility(false);
        }
    }

    void SetVisibility(bool visible)
    {
        // オブジェクトの表示・非表示を切り替える
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = visible;
        }
        else
        {
            // 子オブジェクトにRendererがある場合はそれらも表示・非表示にする
            foreach (Renderer childRenderer in GetComponentsInChildren<Renderer>())
            {
                childRenderer.enabled = visible;
            }
        }
    }

    public void GetTargetTransform(Transform targetTranform, float distance)
    {
        target = targetTranform;
        visibilityDistance = distance;
    }
}
