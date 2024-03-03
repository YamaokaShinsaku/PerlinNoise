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

    // ���A�̖��x
    public float threshold = 0.5f;
    // ���A�L���[�u�̐e
    public Transform caveParent;
    public float maxCaveHeight = 5;


    // ��L����Ă�����W�̃Z�b�g
    private HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int>();

    private void Awake()
    {
        // �}�b�v�T�C�Y�̐ݒ�
        transform.localScale = new Vector3 (mapSize, mapSize, mapSize);

        // �����}�b�v�ɂȂ�Ȃ��悤�ɃV�[�h�l�𐶐�
        seedX = Random.value * 100.0f;
        seedZ = Random.value * 100.0f;

        CreateBaseMap();
        GenerateCave(threshold);
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
        GenerateCave(threshold);
        CreateBottomOfMap();
    }

    /// <summary>
    /// ��{�ƂȂ�}�b�v�𐶐�
    /// </summary>
    void CreateBaseMap()
    {
        // �L���[�u�𐶐�
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // �V�����L���[�u�𐶐����A���ʂɒu��
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localPosition = new Vector3(x, 1, z);
                cube.transform.SetParent(transform);

                // �R���C�_�[���s�K�v�Ȏ�
                if (!needToCollider)
                {
                    Destroy(cube.GetComponent<BoxCollider>());
                }

                // ������ݒ�
                SetY(cube);

                // ��L����Ă�����W��ǉ�
                occupiedPositions.Add(new Vector3Int(x, 1, z));
            }
        }
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
        float y = 1.0f;

        // �p�[�����m�C�Y���g�p���č������v�Z����ꍇ
        if(isPerlinNoiseMap)
        {
            float xSample = (cube.transform.localPosition.x + seedX) / relief;
            float zSample = (cube.transform.localPosition.z + seedZ) / relief;

            float noise = Mathf.PerlinNoise(xSample, zSample);
            // PerlinNoise�̕Ԃ�l���g�p���č������v�Z
            if (noise == 0.0f)
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
            y = Mathf.RoundToInt(y);
        }

        // �L���[�u�̈ʒu��ݒ�
        cube.transform.localPosition =
            new Vector3(cube.transform.localPosition.x, y, cube.transform.localPosition.z);

        // �����ɂ���ĐF��i�K�I�ɕύX
        Color color = Color.black;

        if (y > maxHeight * 0.3f)
        {
            // �����ۂ��F
            ColorUtility.TryParseHtmlString("#019540FF", out color);
        }
        else if (y > maxHeight * 0.2f)
        {
            // �����ۂ��F
            ColorUtility.TryParseHtmlString("#2432ADFF", out color);
        }
        else if (y > maxHeight * 0.1f)
        {
            // �}�O�}���ۂ��F
            ColorUtility.TryParseHtmlString("#D4500EFF", out color);
        }

        // �ݒ肵���F���L���[�u�ɔ��f
        cube.GetComponent<MeshRenderer>().material.color = color;


        // ��L����Ă�����W��ǉ�
        occupiedPositions.Add(new Vector3Int((int)cube.transform.localPosition.x, (int)cube.transform.localPosition.y, (int)cube.transform.localPosition.z));
    }

    /// <summary>
    /// ���A�̒n�`�𐶐�
    /// </summary>
    private void GenerateCave(float threshold)
    {
        // �L���[�u�𐶐�
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // �n�`�����݂��Ȃ��ꍇ�̂ݓ��A�𐶐�
                if (!IsPositionOccupied(x, z))
                {
                    float xCoord = (float)x / width;
                    float zCoord = (float)z / depth;

                    float noise = Mathf.PerlinNoise(xCoord, zCoord);

                    if (noise > threshold)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        // ���A�����������ʒu�𐧌�
                        cube.transform.localPosition = new Vector3(x, Random.Range(1, maxCaveHeight), z);
                        cube.transform.SetParent(caveParent);

                        if (!needToCollider)
                        {
                            Destroy(cube.GetComponent<BoxCollider>());
                        }
                        cube.GetComponent<MeshRenderer>().material.color = Color.red;

                        SetCaveY(cube);
                    }
                }
            }
        }
    }

    /// <summary>
    /// ���A�n�`��Y���W��ݒ�
    /// </summary>
    /// <param name="cube"></param>
    void SetCaveY(GameObject cube)
    {
        float y = Random.Range(1, maxCaveHeight); // Y���W��1����maxHeight/2�͈̔͂ɐݒ�

        // �L���[�u�̈ʒu��ݒ�
        cube.transform.localPosition = new Vector3(cube.transform.localPosition.x, y, cube.transform.localPosition.z);
        // �p�[�����m�C�Y���g�p���č������v�Z����ꍇ
        if (isPerlinNoiseMap)
        {
            float xSample = (cube.transform.localPosition.x + seedX) / relief;
            float zSample = (cube.transform.localPosition.z + seedZ) / relief;

            float noise = Mathf.PerlinNoise(xSample, zSample);
            // PerlinNoise�̕Ԃ�l���g�p���č������v�Z
            if(noise == 0.0f)
            {
                noise = 0.1f;
            }
            y = maxCaveHeight * noise;
        }
        // ���S�����_���ō������v�Z����ꍇ
        else
        {
            y = Random.Range(1, maxCaveHeight);
        }

        // ���炩�ɕω����Ȃ��ꍇ�́A�����l�̌ܓ�
        if (!isSmoothness)
        {
            y = Mathf.RoundToInt(y);
        }

        // �L���[�u�̈ʒu��ݒ�
        cube.transform.localPosition =
            new Vector3(cube.transform.localPosition.x, y, cube.transform.localPosition.z);
    }
    /// <summary>
    /// �w�肳�ꂽ���W���n�`�Ő�L����Ă��邩�ǂ������m�F
    /// </summary>
    /// <param name="x">X���W</param>
    /// <param name="z">Z���W</param>
    /// <returns>�n�`�Ő�L����Ă��邩�ǂ���</returns>
    private bool IsPositionOccupied(int x, int z)
    {
        return occupiedPositions.Contains(new Vector3Int(x, 1, z));
    }
}

