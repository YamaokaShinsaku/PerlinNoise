using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceVisibility : MonoBehaviour
{
    public Transform target; // �J������Transform
    public float visibilityDistance = 10f; // �I�u�W�F�N�g���\������鋗��

    void Update()
    {
        // �J�����Ƃ��̃I�u�W�F�N�g�̋������v�Z
        float distance = Vector3.Distance(transform.position, target.position);

        // �J�����Ƃ̋������w�肵���\���������߂��ꍇ�̓I�u�W�F�N�g��\���A����ȊO�͔�\���ɂ���
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
        // �I�u�W�F�N�g�̕\���E��\����؂�ւ���
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = visible;
        }
        else
        {
            // �q�I�u�W�F�N�g��Renderer������ꍇ�͂������\���E��\���ɂ���
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
