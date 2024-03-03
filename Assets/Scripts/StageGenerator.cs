using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージを自動生成するクラス
/// </summary>
public class StageGenerator : MonoBehaviour
{
    // キューブのプレハブ
    GameObject cubePrefab;
    // ステージのサイズ
    public int width = 10; 
    public int height = 5;
    public int depth = 10;

    // Perlin Noiseのスケール
    public float scale = 0.1f;
    // 洞窟を生成する閾値
    public float caveThreshold = 0.4f;

    // 洞窟
    public int caveNum = 5;
    // 洞窟の最小半径
    public float caveRadius = 5.0f;
    // 洞窟の最大半径
    public float caveMaxRadius = 10.0f;

    // シード
    float seedX;
    float seedZ;

    // 起伏の激しさ
    [SerializeField, Header("起伏の激しさ")]
    float relief = 1.0f;

    void Start()
    {
        // 同じマップにならないようにシード値を生成
        seedX = Random.value * 100.0f;
        seedZ = Random.value * 100.0f;

        GenerateBaseMap();
        GenerateButtomOfMap();
    }

    /// <summary>
    /// 基本マップを生成
    /// </summary>
    void GenerateBaseMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // Perlin Noiseを使用して高さを生成
                //float y = Mathf.PerlinNoise(x * scale, z * scale) * height;
                float xSample = (x * scale + seedX) / relief;
                float zSample = (z * scale + seedZ) / relief;
                float y = Mathf.PerlinNoise(xSample, zSample) * height;

                for (int yIndex = 0; yIndex < Mathf.CeilToInt(y); yIndex++)
                {              
                    // キューブを生成
                    GameObject cube = Instantiate(cubePrefab, new Vector3(x, yIndex, z), Quaternion.identity);
                    // 生成したキューブをこのスクリプトの子オブジェクトに設定
                    cube.transform.parent = transform;
                    SetCubeColorByHeight(cube, yIndex);
                }
            }
        }

        // 生成した地形を彫り込んで洞窟を生成する
        // 複数の洞窟を生成
        GenerateCave(caveNum, caveThreshold);
    }

    /// <summary>
    /// 洞窟を生成
    /// </summary>
    /// <param name="numCaves">洞窟の数</param>
    /// <param name="caveDensity">洞窟を生成する際の閾値</param>
    void GenerateCave(int numCaves, float caveDensity)
    {
        for (int i = 0; i < numCaves; i++)
        {
            Vector3 center = new Vector3(Random.Range(0, width), Random.Range(0, height / 3), Random.Range(0, depth));
            // 洞窟の半径をランダムに設定
            float radius = Random.Range(caveRadius, caveMaxRadius);
            // パーリンノイズのスケールをランダムに設定
            float noiseScale = Random.Range(caveRadius, caveMaxRadius);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        float distance = Vector3.Distance(new Vector3(x, y, z), center);
                        float noiseValue = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);

                        // 指定した中心点から一定の距離内で、ランダムな形状の洞窟を生成する
                        if (distance < radius && noiseValue < caveDensity)
                        {
                            // キューブを削除
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
    /// マップの底を生成する
    /// </summary>
    void GenerateButtomOfMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                // キューブを生成
                GameObject cube = Instantiate(cubePrefab, new Vector3(x, -1, z), Quaternion.identity);
                // 生成したキューブをこのスクリプトの子オブジェクトに設定
                cube.transform.parent = transform;
                cube.GetComponent<MeshRenderer>().material.color = Color.black;
            }
        }
    }

    /// <summary>
    /// 生成したブロックの色を高さに応じて変更
    /// </summary>
    void SetCubeColorByHeight(GameObject cube, float y)
    {
        Renderer renderer = cube.GetComponent<Renderer>();
        Material material = renderer.material;
        
        float ratio = y / height;

        // 高い部分は緑色
        if (ratio > 0.2f)
        {
            material.color = Color.green;
        }
        // 中間の部分は水色
        else if (ratio > 0.1f)
        {
            material.color = Color.cyan;
        }
        // 低い部分は赤色
        else
        {
            material.color = Color.red;
        }
    }
}
