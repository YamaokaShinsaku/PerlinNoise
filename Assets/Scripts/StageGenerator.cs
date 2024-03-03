using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�e�[�W��������������N���X
/// </summary>
public class StageGenerator : MonoBehaviour
{
    // �L���[�u�̃v���n�u
    GameObject cubePrefab;
    // �X�e�[�W�̃T�C�Y
    public int width = 10; 
    public int height = 5;
    public int depth = 10;

    // Perlin Noise�̃X�P�[��
    public float scale = 0.1f;
    // ���A�𐶐�����臒l
    public float caveThreshold = 0.4f;

    // ���A
    public int caveNum = 5;
    // ���A�̍ŏ����a
    public float caveRadius = 5.0f;
    // ���A�̍ő唼�a
    public float caveMaxRadius = 10.0f;

    // �V�[�h
    float seedX;
    float seedZ;

    // �N���̌�����
    [SerializeField, Header("�N���̌�����")]
    float relief = 1.0f;

    void Start()
    {
        // �����}�b�v�ɂȂ�Ȃ��悤�ɃV�[�h�l�𐶐�
        seedX = Random.value * 100.0f;
        seedZ = Random.value * 100.0f;

        GenerateBaseMap();
        GenerateButtomOfMap();
    }

    /// <summary>
    /// ��{�}�b�v�𐶐�
    /// </summary>
    void GenerateBaseMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // Perlin Noise���g�p���č����𐶐�
                //float y = Mathf.PerlinNoise(x * scale, z * scale) * height;
                float xSample = (x * scale + seedX) / relief;
                float zSample = (z * scale + seedZ) / relief;
                float y = Mathf.PerlinNoise(xSample, zSample) * height;

                for (int yIndex = 0; yIndex < Mathf.CeilToInt(y); yIndex++)
                {              
                    // �L���[�u�𐶐�
                    GameObject cube = Instantiate(cubePrefab, new Vector3(x, yIndex, z), Quaternion.identity);
                    // ���������L���[�u�����̃X�N���v�g�̎q�I�u�W�F�N�g�ɐݒ�
                    cube.transform.parent = transform;
                    SetCubeColorByHeight(cube, yIndex);
                }
            }
        }

        // ���������n�`�𒤂荞��œ��A�𐶐�����
        // �����̓��A�𐶐�
        GenerateCave(caveNum, caveThreshold);
    }

    /// <summary>
    /// ���A�𐶐�
    /// </summary>
    /// <param name="numCaves">���A�̐�</param>
    /// <param name="caveDensity">���A�𐶐�����ۂ�臒l</param>
    void GenerateCave(int numCaves, float caveDensity)
    {
        for (int i = 0; i < numCaves; i++)
        {
            Vector3 center = new Vector3(Random.Range(0, width), Random.Range(0, height / 3), Random.Range(0, depth));
            // ���A�̔��a�������_���ɐݒ�
            float radius = Random.Range(caveRadius, caveMaxRadius);
            // �p�[�����m�C�Y�̃X�P�[���������_���ɐݒ�
            float noiseScale = Random.Range(caveRadius, caveMaxRadius);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        float distance = Vector3.Distance(new Vector3(x, y, z), center);
                        float noiseValue = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);

                        // �w�肵�����S�_������̋������ŁA�����_���Ȍ`��̓��A�𐶐�����
                        if (distance < radius && noiseValue < caveDensity)
                        {
                            // �L���[�u���폜
                            foreach (Transform child in transform)
                            {
                                if (child.position == new Vector3(x, y, z))
                                {
                                    Destroy(child.gameObject);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// �}�b�v�̒�𐶐�����
    /// </summary>
    void GenerateButtomOfMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // �L���[�u�𐶐�
                GameObject cube = Instantiate(cubePrefab, new Vector3(x, -1, z), Quaternion.identity);
                // ���������L���[�u�����̃X�N���v�g�̎q�I�u�W�F�N�g�ɐݒ�
                cube.transform.parent = transform;
                cube.GetComponent<MeshRenderer>().material.color = Color.black;
            }
        }
    }

    /// <summary>
    /// ���������u���b�N�̐F�������ɉ����ĕύX
    /// </summary>
    void SetCubeColorByHeight(GameObject cube, float y)
    {
        Renderer renderer = cube.GetComponent<Renderer>();
        Material material = renderer.material;
        
        float ratio = y / height;

        // ���������͗ΐF
        if (ratio > 0.2f)
        {
            material.color = Color.green;
        }
        // ���Ԃ̕����͐��F
        else if (ratio > 0.1f)
        {
            material.color = Color.cyan;
        }
        // �Ⴂ�����͐ԐF
        else
        {
            material.color = Color.red;
        }
    }
}
