using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �p�[�����m�C�Y���g�p���������}�b�v�����N���X
/// </summary>
public class RandomMapGenerator : MonoBehaviour
{
    // �V�[�h
    float seedX;
    float seedZ;

    // �}�b�v�T�C�Y
    [SerializeField, Header("���s���ɕύX�ł��Ȃ�")]
    float width = 50;
    [SerializeField]
    float depth = 50;

    // �R���C�_�[���K�v���ǂ���
    [SerializeField]
    bool needToCollider = false;

    // �����̍ő�l
    [SerializeField, Header("���s���ɕύX�ł���")]
    float maxHeight = 10;

    // �p�[�����m�C�Y���g�p�����}�b�v���ǂ���
    [SerializeField]
    bool isPerlinNoiseMap = true;

    // �N���̌�����
    [SerializeField, Header("�N���̌�����")]
    float relief = 15.0f;

    // Y���W�����炩�ɂ���(�����_�ȉ������̂܂܂ɂ���)
    [SerializeField]
    bool isSmoothness = false;

    // �}�b�v�̑傫��
    [SerializeField]
    float mapSize = 1.0f;


    private void Awake()
    {
        // �}�b�v�T�C�Y�̐ݒ�
        transform.localScale = new Vector3 (mapSize, mapSize, mapSize);

        // �����}�b�v�ɂȂ�Ȃ��悤�ɃV�[�h�l�𐶐�
        seedX = Random.value * 100.0f;
        seedZ = Random.value * 100.0f;
        
        // �L���[�u�𐶐�
        for (int x = 0; x < width; x++)
        {
            for(int z = 0; z < depth; z++) 
            {
                // �V�����L���[�u�𐶐����A���ʂɒu��
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localPosition = new Vector3(x, 0, z);
                cube.transform.SetParent(transform);

                // �R���C�_�[���s�K�v�Ȏ�
                if(!needToCollider)
                {
                    Destroy(cube.GetComponent<BoxCollider>());
                }

                // ������ݒ�
                SetY(cube);
            }
        }

        CreateBottomOfMap();

    }

    /// <summary>
    /// �C���X�y�N�^�[�̒l���ύX���ꂽ�Ƃ�
    /// </summary>
    private void OnValidate()
    {
        // ���s���łȂ��Ƃ�
        if(!Application.isPlaying)
        {
            return;
        }

        // �}�b�v�̑傫���ݒ�
        transform.localScale = new Vector3 (mapSize, mapSize, mapSize);

        // �e�L���[�u��Y���W�ύX
        foreach(Transform child in transform)
        {
            SetY(child.gameObject);
        }

        CreateBottomOfMap();
    }

    /// <summary>
    /// �}�b�v�̒�𐶐�
    /// </summary>
    public void CreateBottomOfMap()
    {
        // �L���[�u�𐶐�
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // �V�����L���[�u�𐶐����A���ʂɒu��
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localPosition = new Vector3(x, 0, z);
                cube.transform.SetParent(transform);

                cube.GetComponent<MeshRenderer>().material.color = Color.black;

                // �R���C�_�[���s�K�v�Ȏ�
                if (!needToCollider)
                {
                    Destroy(cube.GetComponent<BoxCollider>());
                }
            }
        }
    }

    /// <summary>
    /// �L���[�u��Y���W��ݒ肷��
    /// </summary>
    /// <param name="cube">�L���[�u</param>
    private void SetY(GameObject cube)
    {
        float y = 0.0f;

        // �p�[�����m�C�Y���g�p���č������v�Z����ꍇ
        if(isPerlinNoiseMap)
        {
            float xSample = (cube.transform.localPosition.x + seedX) / relief;
            float zSample = (cube.transform.localPosition.z + seedZ) / relief;

            float noise = Mathf.PerlinNoise(xSample, zSample);
            // PerlinNoise�̕Ԃ茌���g�p���č������v�Z
            if(noise == 0.0f)
            {
                noise = 0.1f;
            }
            y = maxHeight * noise;
        }
        // ���S�����_���ō������v�Z����ꍇ
        else
        {
            y = Random.Range(1, maxHeight);
        }

        // ���炩�ɕω����Ȃ��ꍇ�́A�����l�̌ܓ�
        if(!isSmoothness)
        {
            y = Mathf.Round(y);
        }

        // �L���[�u�̈ʒu��ݒ�
        cube.transform.localPosition =
            new Vector3(cube.transform.localPosition.x, y, cube.transform.localPosition.z);

        // �����ɂ���ĐF��i�K�I�ɕύX
        Color color = Color.black;

        if(y > maxHeight * 0.3f)
        {
            // �����ۂ��F
            ColorUtility.TryParseHtmlString("#019540FF", out color);
        }
        else if(y > maxHeight * 0.2f)
        {
            // �����ۂ��F
            ColorUtility.TryParseHtmlString("#2432ADFF", out color);
        }
        else if(y > maxHeight * 0.1f)
        {
            // �}�O�}���ۂ��F
            ColorUtility.TryParseHtmlString("#D4500EFF", out color);
        }

        // �ݒ肵���F���L���[�u�ɔ��f
        cube.GetComponent<MeshRenderer>().material.color = color;               
    }

}
