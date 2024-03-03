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


    private void Awake()
    {
        // マップサイズの設定
        transform.localScale = new Vector3 (mapSize, mapSize, mapSize);

        // 同じマップにならないようにシード値を生成
        seedX = Random.value * 100.0f;
        seedZ = Random.value * 100.0f;
        
        // キューブを生成
        for (int x = 0; x < width; x++)
        {
            for(int z = 0; z < depth; z++) 
            {
                // 新しいキューブを生成し、平面に置く
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localPosition = new Vector3(x, 0, z);
                cube.transform.SetParent(transform);

                // コライダーが不必要な時
                if(!needToCollider)
                {
                    Destroy(cube.GetComponent<BoxCollider>());
                }

                // 高さを設定
                SetY(cube);
            }
        }

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

        CreateBottomOfMap();
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
        float y = 0.0f;

        // パーリンノイズを使用して高さを計算する場合
        if(isPerlinNoiseMap)
        {
            float xSample = (cube.transform.localPosition.x + seedX) / relief;
            float zSample = (cube.transform.localPosition.z + seedZ) / relief;

            float noise = Mathf.PerlinNoise(xSample, zSample);
            // PerlinNoiseの返り血を使用して高さを計算
            if(noise == 0.0f)
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
            y = Mathf.Round(y);
        }

        // キューブの位置を設定
        cube.transform.localPosition =
            new Vector3(cube.transform.localPosition.x, y, cube.transform.localPosition.z);

        // 高さによって色を段階的に変更
        Color color = Color.black;

        if(y > maxHeight * 0.3f)
        {
            // 草っぽい色
            ColorUtility.TryParseHtmlString("#019540FF", out color);
        }
        else if(y > maxHeight * 0.2f)
        {
            // 水っぽい色
            ColorUtility.TryParseHtmlString("#2432ADFF", out color);
        }
        else if(y > maxHeight * 0.1f)
        {
            // マグマっぽい色
            ColorUtility.TryParseHtmlString("#D4500EFF", out color);
        }

        // 設定した色をキューブに反映
        cube.GetComponent<MeshRenderer>().material.color = color;               
    }

}
