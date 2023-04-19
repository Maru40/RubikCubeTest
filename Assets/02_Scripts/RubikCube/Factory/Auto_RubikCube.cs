using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;
using Unity.Services.Core.Environments;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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
        kZ,
    }

    private List<GroupData> mGroupDatas = new List<GroupData>();

    private Dictionary<AxisType, List<GroupData>> mGraupDatasMap =
        new Dictionary<AxisType, List<GroupData>>();

    [SerializeField]
    private GameObject m_prefab;                                            //生成するプレハブ

    [SerializeField]
    private GameObject mRotationParentPrefab;

    private RotationParent mRotationParent;

    [SerializeField]
    private float m_offsetSize = 1.0f;                                      //生成する大きさのオフセット

    [SerializeField]
    private float m_offsetRange = 1.0f;                                     //離すオフセット距離

    [SerializeField]
    private Vector3 m_maxCubes = new Vector3(3.0f, 3.0f, 3.0f);             //生成するキューブの最大量

    [SerializeField]
    private int mSize = 3;

    private List<GameObject> mCubes = new List<GameObject>();    //キューブの配列を保存する。
    private List<List<List<GameObject>>> mCubes3 = new List<List<List<GameObject>>>();

    private int mSelectIndex = 0;

    private AxisType mAxisType = AxisType.kX;

    private List<float> mComparisonSelectValue = new List<float>();

    public enum eOperationType {
        U,  //↑
        D,  //下
        L,
        R,
        F,
        B
    }

    public class State
    {
        //各コーナーパーツの場所を表す8次元ベクトル
        public int[] cp = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
        //各コーナーの向きがどの向きを向いているかどうかの情報を表す8次元ベクトル
        public int[] co = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        //各エッジパーツの場所を表す12次元ベクトル
        public int[] ep = new int[12] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        //各エッジパーツの向きを表す12次元ベクトル
        public int[] eo = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }

    private Dictionary<eOperationType, State> mMoveMap = new Dictionary<eOperationType, State>();

    //ステート管理系
    State mState;

    ////各コーナーパーツの場所を表す8次元ベクトル
    //int[] cp = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 }; 
    ////各コーナーの向きがどの向きを向いているかどうかの情報を表す8次元ベクトル
    //int[] co = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
    ////各エッジパーツの場所を表す12次元ベクトル
    //int[] ep = new int[12] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
    ////各エッジパーツの向きを表す12次元ベクトル
    //int[] eo = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    private void Start()
    {
        Factory_RubikCubes();   //キューブの生成
        CreateRotationGroup();  //グループの作成
        CreateComparisonSelectValue();

        CreateOperationMoves();
        ColorAdjust();    //カラー調整

        mRotationParent = Instantiate(mRotationParentPrefab).GetComponent<RotationParent>();
    }

    private void Update()
    {
        InputUpdate();
    }

    private void CreateOperationMoves()
    {
        var state = new State();

        state.cp = new int[8] { 3, 0, 1, 2, 4, 5, 6, 7 };
        state.co = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        state.ep = new int[12] { 0, 1, 2, 3, 7, 4, 5, 6, 8, 9, 10, 11 };
        state.eo = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        mMoveMap[eOperationType.U] = state;

        state.cp = new int[8] { 0, 1, 2, 3, 5, 6, 7, 4 };
        state.co = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        state.ep = new int[12] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 8 };
        state.eo = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        mMoveMap[eOperationType.D] = state;

        state.cp = new int[8] { 4, 1, 2, 0, 7, 5, 6, 3 };
        state.co = new int[8] { 2, 0, 0, 1, 1, 0, 0, 2 };
        state.ep = new int[12] { 11, 1, 2, 7, 4, 5, 6, 0, 8, 9, 10, 3 };
        state.eo = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        mMoveMap[eOperationType.L] = state;

        state.cp = new int[8] { 0, 2, 6, 3, 4, 1, 5, 7 };
        state.co = new int[8] { 0, 1, 2, 0, 0, 2, 1, 0 };
        state.ep = new int[12] { 0, 5, 9, 3, 4, 2, 6, 7, 8, 1, 10, 11 };
        state.eo = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        mMoveMap[eOperationType.R] = state;

        state.cp = new int[8] { 0, 1, 3, 7, 4, 5, 2, 6 };
        state.co = new int[8] { 0, 0, 1, 2, 0, 0, 2, 1 };
        state.ep = new int[12] { 0, 1, 6, 10, 4, 5, 3, 7, 8, 9, 2, 11 };
        state.eo = new int[12] { 0, 0, 1, 1, 0, 0, 1, 0, 0, 0, 1, 0 };
        mMoveMap[eOperationType.F] = state;

        state.cp = new int[8] { 1, 5, 2, 3, 0, 4, 6, 7 };
        state.co = new int[8] { 1, 2, 0, 0, 2, 1, 0, 0 };
        state.ep = new int[12] { 4, 8, 2, 3, 1, 5, 6, 7, 0, 9, 10, 11 };
        state.eo = new int[12] { 1, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0 };
        mMoveMap[eOperationType.B] = state;
    }

    State AddState(State self, State move)
    {
        State newState = new State();

        newState.cp = AddCpState(self, move);

        return newState;
    }

    int[] AddCpState(State self, State move)
    {
        var result = new int[8];

        for (int i = 0; i < result.Length; ++i)
        {
            int index = move.cp[i];
            result[i] = self.cp[index];
        }

        return result;
    }

    private void ColorAdjust()
    {
        var centerPosition = Vector3.zero;

        foreach(var cube in mCubes)
        {
            var toCubeVec = cube.transform.position - centerPosition;
        }
    }

    void StartRotation()
    {
        if(mRotationParent.IsRotation()) {
            return;
        }

        SettingGroup();
        mRotationParent.StartRotation(GetAxis());
    }

    private void SettingGroup()
    {
        foreach(var cube in mCubes)
        {
            cube.transform.SetParent(null);
        }

        mRotationParent.transform.rotation = Quaternion.identity;

        foreach(var cube in CreateRotationGroup((int)mAxisType, mSelectIndex))
        {
            cube.transform.SetParent(mRotationParent.transform);
        }
    }

    List<GameObject> CreateRotationGroup(int axisIndex, int selectIndex)
    {
        List<GameObject> result = new List<GameObject>();

        foreach(var cube in mCubes)
        {
            if (IsGroup(cube, axisIndex, selectIndex))
            {
                result.Add(cube);
            }
        }

        return result;
    }

    bool IsGroup(GameObject cube, int axisIndex, int selectIndex)
    {
        float t = 0.5f;

        var position = cube.transform.position;
        float axisValue = position[axisIndex];
        float select = mComparisonSelectValue[selectIndex];

        float min = select - t;
        float max = select + t; 

        return min < axisValue && axisValue < max;
    }

    void CreateComparisonSelectValue()
    {
        mComparisonSelectValue = new List<float>(){
            -1.0f, 0.0f, 1.0f
        };
    }

    private void InputUpdate()
    {
        SelectIndex();
        ChangeAxisType();
        InputStartRotation();
    }

    private void SelectIndex()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            mSelectIndex++;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            mSelectIndex--;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            mSelectIndex++;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            mSelectIndex--;
        }

        //クランプ
        if (mSelectIndex < 0)
        {
            mSelectIndex = mSize - 1;
        }

        if(mSize <= mSelectIndex)
        {
            mSelectIndex = 0;
        }
    }

    void ChangeAxisType()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            int axisIndex = (int)(mAxisType);
            axisIndex++;

            //クランプ
            if((int)(AxisType.kZ) < axisIndex)
            {
                axisIndex = 0;
            }

            mAxisType = (AxisType)axisIndex;
        }
    }

    void InputStartRotation()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            StartRotation();
        }
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

    private Vector3 GetAxis()
    {
        switch (mAxisType)
        {
            case AxisType.kX:
                return Vector3.right;

            case AxisType.kY:
                return Vector3.up;

            case AxisType.kZ:
                return Vector3.forward;
        }

        return Vector3.up;
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
                    var position = firstPosition + (new Vector3(x, y, z) * m_offsetRange);
                    var cube = CreateCube(position);
                    mCubes.Add(cube);
                }
            }
        }
    }

    private void Factory_RubikCube_Ex()
    {
        float OneSize = 1.0f;

        float halfCount = mSize * 0.5f;
        float halfOneCubeSize = OneSize * 0.5f;
        float fieldSize = mSize * OneSize;

        var startPosition = CalculateStartPosition_Ex(fieldSize);

        for (int i = 0; i < mSize; ++i)
        {
            
        }
    }
    
    private Vector3 CalculateStartPosition_Ex(float fieldSize)
    {
        Vector3 position = new Vector3(0, 0, 0);
        var scale = new Vector3(fieldSize, 0, fieldSize);
        var halfScale = scale * 0.5f;
        float x = position.x - halfScale.x;
        float y = position.y;
        float z = position.z - halfScale.z;

        return new Vector3 (x, y, z);
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

        var halfMaxCubes = m_maxCubes / mSize;

        result.x = -halfMaxCubes.x; //xの左を取得

        result.y = -halfMaxCubes.y; //yの下を取得

        result.z = -halfMaxCubes.z; //zの手前を取得

        return result;
    }
}
