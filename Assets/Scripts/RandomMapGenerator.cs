using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// パーリンノイズを使用した自動マップ生成クラス
/// </summary>
public class RandomMapGenerator : MonoBehaviour
{
    // シード
    float seedX;
    float seedZ;

    // マップサイズ
    [SerializeField, Header("実行中に変更できない")]
    float width = 50;
    [SerializeField]
    float depth = 50;

    // コライダーが必要かどうか
    [SerializeField]
    bool needToCollider = false;

    // 高さの最大値
    [SerializeField, Header("実行中に変更できる")]
    float maxHeight = 10;

    // パーリンノイズを使用したマップかどうか
    [SerializeField]
    bool isPerlinNoiseMap = true;

    // 起伏の激しさ
    [SerializeField, Header("起伏の激しさ")]
    float relief = 15.0f;

    // Y座標を滑らかにする(小数点以下をそのままにする)
    [SerializeField]
    bool isSmoothness = false;

    // マップの大きさ
    [SerializeField]
    float mapSize = 1.0f;

    // 洞窟の密度
    public float threshold = 0.5f;
    // 洞窟キューブの親
    public Transform caveParent;
    public float maxCaveHeight = 5;


    // 占有されている座標のセット
    private HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int>();

    private void Awake()
    {
        // マップサイズの設定
        transform.localScale = new Vector3 (mapSize, mapSize, mapSize);

        // 同じマップにならないようにシード値を生成
        seedX = Random.value * 100.0f;
        seedZ = Random.value * 100.0f;

        CreateBaseMap();
        GenerateCave(threshold);
        CreateBottomOfMap();

    }

    /// <summary>
    /// インスペクターの値が変更されたとき
    /// </summary>
    private void OnValidate()
    {
        // 実行中でないとき
        if(!Application.isPlaying)
        {
            return;
        }

        // マップの大きさ設定
        transform.localScale = new Vector3 (mapSize, mapSize, mapSize);

        // 各キューブのY座標変更
        foreach(Transform child in transform)
        {
            SetY(child.gameObject);
        }
        GenerateCave(threshold);
        CreateBottomOfMap();
    }

    /// <summary>
    /// 基本となるマップを生成
    /// </summary>
    void CreateBaseMap()
    {
        // キューブを生成
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // 新しいキューブを生成し、平面に置く
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localPosition = new Vector3(x, 1, z);
                cube.transform.SetParent(transform);

                // コライダーが不必要な時
                if (!needToCollider)
                {
                    Destroy(cube.GetComponent<BoxCollider>());
                }

                // 高さを設定
                SetY(cube);

                // 占有されている座標を追加
                occupiedPositions.Add(new Vector3Int(x, 1, z));
            }
        }
    }

    /// <summary>
    /// マップの底を生成
    /// </summary>
    public void CreateBottomOfMap()
    {
        // キューブを生成
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // 新しいキューブを生成し、平面に置く
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localPosition = new Vector3(x, 0, z);
                cube.transform.SetParent(transform);

                cube.GetComponent<MeshRenderer>().material.color = Color.black;

                // コライダーが不必要な時
                if (!needToCollider)
                {
                    Destroy(cube.GetComponent<BoxCollider>());
                }
            }
        }
    }

    /// <summary>
    /// キューブのY座標を設定する
    /// </summary>
    /// <param name="cube">キューブ</param>
    private void SetY(GameObject cube)
    {
        float y = 1.0f;

        // パーリンノイズを使用して高さを計算する場合
        if(isPerlinNoiseMap)
        {
            float xSample = (cube.transform.localPosition.x + seedX) / relief;
            float zSample = (cube.transform.localPosition.z + seedZ) / relief;

            float noise = Mathf.PerlinNoise(xSample, zSample);
            // PerlinNoiseの返り値を使用して高さを計算
            if (noise == 0.0f)
            {
                noise = 0.1f;
            }
            y = maxHeight * noise;
        }
        // 完全ランダムで高さを計算する場合
        else
        {
            y = Random.Range(1, maxHeight);
        }

        // 滑らかに変化しない場合は、ｙを四捨五入
        if(!isSmoothness)
        {
            y = Mathf.RoundToInt(y);
        }

        // キューブの位置を設定
        cube.transform.localPosition =
            new Vector3(cube.transform.localPosition.x, y, cube.transform.localPosition.z);

        // 高さによって色を段階的に変更
        Color color = Color.black;

        if (y > maxHeight * 0.3f)
        {
            // 草っぽい色
            ColorUtility.TryParseHtmlString("#019540FF", out color);
        }
        else if (y > maxHeight * 0.2f)
        {
            // 水っぽい色
            ColorUtility.TryParseHtmlString("#2432ADFF", out color);
        }
        else if (y > maxHeight * 0.1f)
        {
            // マグマっぽい色
            ColorUtility.TryParseHtmlString("#D4500EFF", out color);
        }

        // 設定した色をキューブに反映
        cube.GetComponent<MeshRenderer>().material.color = color;


        // 占有されている座標を追加
        occupiedPositions.Add(new Vector3Int((int)cube.transform.localPosition.x, (int)cube.transform.localPosition.y, (int)cube.transform.localPosition.z));
    }

    /// <summary>
    /// 洞窟の地形を生成
    /// </summary>
    private void GenerateCave(float threshold)
    {
        // キューブを生成
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // 地形が存在しない場合のみ洞窟を生成
                if (!IsPositionOccupied(x, z))
                {
                    float xCoord = (float)x / width;
                    float zCoord = (float)z / depth;

                    float noise = Mathf.PerlinNoise(xCoord, zCoord);

                    if (noise > threshold)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        // 洞窟が生成される位置を制限
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
    /// 洞窟地形のY座標を設定
    /// </summary>
    /// <param name="cube"></param>
    void SetCaveY(GameObject cube)
    {
        float y = Random.Range(1, maxCaveHeight); // Y座標を1からmaxHeight/2の範囲に設定

        // キューブの位置を設定
        cube.transform.localPosition = new Vector3(cube.transform.localPosition.x, y, cube.transform.localPosition.z);
        // パーリンノイズを使用して高さを計算する場合
        if (isPerlinNoiseMap)
        {
            float xSample = (cube.transform.localPosition.x + seedX) / relief;
            float zSample = (cube.transform.localPosition.z + seedZ) / relief;

            float noise = Mathf.PerlinNoise(xSample, zSample);
            // PerlinNoiseの返り値を使用して高さを計算
            if(noise == 0.0f)
            {
                noise = 0.1f;
            }
            y = maxCaveHeight * noise;
        }
        // 完全ランダムで高さを計算する場合
        else
        {
            y = Random.Range(1, maxCaveHeight);
        }

        // 滑らかに変化しない場合は、ｙを四捨五入
        if (!isSmoothness)
        {
            y = Mathf.RoundToInt(y);
        }

        // キューブの位置を設定
        cube.transform.localPosition =
            new Vector3(cube.transform.localPosition.x, y, cube.transform.localPosition.z);
    }
    /// <summary>
    /// 指定された座標が地形で占有されているかどうかを確認
    /// </summary>
    /// <param name="x">X座標</param>
    /// <param name="z">Z座標</param>
    /// <returns>地形で占有されているかどうか</returns>
    private bool IsPositionOccupied(int x, int z)
    {
        return occupiedPositions.Contains(new Vector3Int(x, 1, z));
    }
}

