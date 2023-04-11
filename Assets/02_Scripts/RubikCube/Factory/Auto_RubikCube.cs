using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auto_RubikCube : MonoBehaviour
{
    [SerializeField]
    private GameObject m_prefab;                                            //生成するプレハブ

    [SerializeField]
    private float m_offsetSize = 1.0f;                                      //生成する大きさのオフセット

    [SerializeField]
    private float m_offsetRange = 1.0f;                                     //離すオフセット距離

    [SerializeField]
    private Vector3 m_maxCubes = new Vector3(3.0f, 3.0f, 3.0f);             //生成するキューブの最大量

    private List<List<GameObject>> m_cubes;                                 //生成したキューブ一覧(基本的に27or26(中心いらない))

    private void Start()
    {
        Factory_RubikCubes();   //キューブの生成
    }

    /// <summary>
    /// すべてのキューブの生成
    /// </summary>
    private void Factory_RubikCubes()
    {
        var firstPosition = CalculateCubesFirstPosition();

        for(int x = 0; x < m_maxCubes.x; ++x)
        {
            for(int y = 0; y < m_maxCubes.y; ++y)
            {
                for (int z = 0; z < m_maxCubes.z; ++z)
                {
                    //キューブの生成
                    var cube = CreateCube(new Vector3(x, y, z) * m_offsetRange);
                }
            }
        }
    }
    
    /// <summary>
    /// キューブの生成
    /// </summary>
    /// <param name="position">生成する場所</param>
    private GameObject CreateCube(Vector3 position)
    {
        var cube = Instantiate(m_prefab, position, Quaternion.identity, transform);
        var scale = cube.transform.localScale;
        scale *= m_offsetSize;
        cube.transform.localScale = scale;

        return cube;
    }

    /// <summary>
    /// 初期位置の取得
    /// </summary>
    /// <returns>初期位置</returns>
    private Vector3 CalculateCubesFirstPosition()
    {
        var result = Vector3.zero;

        var halfMaxCubes = m_maxCubes * 0.5f;

        result.x = -halfMaxCubes.x; //xの左を取得

        result.y = -halfMaxCubes.y; //yの下を取得

        result.z = -halfMaxCubes.z; //zの手前を取得

        return result;
    }

    private Vector3 CalculateCubePosition(Vector3Int indexVec)
    {
        var result = new Vector3();

        var firstPosition = CalculateCubesFirstPosition();
        //firstPosition.x + (indexVec.x);

        return result;
    }

}
