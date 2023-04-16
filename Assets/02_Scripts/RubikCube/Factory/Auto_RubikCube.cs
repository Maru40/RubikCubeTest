using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Auto_RubikCube : MonoBehaviour
{

    [System.Serializable]
    public struct GroupData
    {
        public List<int> indices;
    }

    public enum AxisType
    {
        kX,
        kY,
        kZ
    }

    private List<GroupData> mGroupDatas = new List<GroupData>();

    private Dictionary<AxisType, List<GroupData>> mGraupDatasMap = 
        new Dictionary<AxisType, List<GroupData>>();

    [SerializeField]
    private GameObject m_prefab;                                            //生成するプレハブ

    [SerializeField]
    private float m_offsetSize = 1.0f;                                      //生成する大きさのオフセット

    [SerializeField]
    private float m_offsetRange = 1.0f;                                     //離すオフセット距離

    [SerializeField]
    private Vector3 m_maxCubes = new Vector3(3.0f, 3.0f, 3.0f);             //生成するキューブの最大量

    [SerializeField]
    private int mSize = 3;

    private List<GameObject> mCubes = new List<GameObject>();    //キューブの配列を保存する。

    private void Start()
    {
        Factory_RubikCubes();   //キューブの生成
        CreateRotationGroup();
    }

    //回転グループの生成
    private void CreateRotationGroup()
    {
        mGraupDatasMap[AxisType.kX] = new List<GroupData>();
        mGraupDatasMap[AxisType.kY] = new List<GroupData>();
        mGraupDatasMap[AxisType.kZ] = new List<GroupData>();

        for (int i = 0; i < mSize; ++i)
        {
            CreateAxisGroup(i, AxisType.kX);
            CreateAxisGroup(i, AxisType.kY);
            CreateAxisGroup(i, AxisType.kZ);
        }
    }

    private void CreateAxisGroup(int mulStartIndex, AxisType type)
    {
        var data = new GroupData();
        data.indices = new List<int>();

        int startIndex = GetStartBaseIndex(type) * mulStartIndex;

        for (int i = 0; i < mSize; ++i)
        {
            int stepIndex = GetStepBaseIndex(type) * i;
            for (int j = 0; j < mSize; ++j)
            {
                int oneLoopIndex = GetOneLoopBaseIndex(type) * j;
                int index = startIndex + stepIndex + oneLoopIndex;

                data.indices.Add(index);
            }
        }

        mGroupDatas.Add(data);
        mGraupDatasMap[type].Add(data);
    }

    private int GetStartBaseIndex(AxisType type)
    {
        switch (type)
        {
            case AxisType.kX:
                return mSize * mSize;
            case AxisType.kY:
                return mSize;
            case AxisType.kZ:
                return 1;
        }

        return 0;
    }

    private int GetStepBaseIndex(AxisType type)
    {
        switch (type)
        {
            case AxisType.kX:
                return mSize;
            case AxisType.kY:
                return 1;
            case AxisType.kZ:
                return mSize;
        }

        return 0;
    }

    private int GetOneLoopBaseIndex(AxisType type)
    {
        switch (type)
        {
            case AxisType.kX:
                return 1;
            case AxisType.kY:
                return mSize * mSize;
            case AxisType.kZ:
                return mSize * mSize;
        }

        return 0;
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
                    mCubes.Add(cube);
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
}
