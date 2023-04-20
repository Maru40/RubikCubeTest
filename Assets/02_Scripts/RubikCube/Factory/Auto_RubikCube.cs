using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;
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

    public struct State
    {
        //各コーナーパーツの場所を表す8次元ベクトル
        public int[] cp;// = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
        //各コーナーの向きがどの向きを向いているかどうかの情報を表す8次元ベクトル
        public int[] co;// = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        //各エッジパーツの場所を表す12次元ベクトル
        public int[] ep;// = new int[12] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        //各エッジパーツの向きを表す12次元ベクトル
        public int[] eo;// = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //各センターパーツの場所を表す
        public int[] centerPoint;
        //各センタパーツの向きを表す
        public int[] centerRotation;

        public State(int i)
        {
            cp = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };

            co = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

            ep = new int[12] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

            eo = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            centerPoint = new int[6] { 0, 1, 2, 3, 4, 5 };

            centerRotation = new int[6] { 0, 0, 0, 0, 0, 0 };
        }

        public State(int[] cp, int[] co, int[] ep, int[] eo)
        {
            this.cp = cp;
            this.co = co;
            this.ep = ep;
            this.eo = eo;
            centerPoint = new int[6] { 0, 1, 2, 3, 4, 5 };
            centerRotation = new int[6] { 0, 0, 0, 0, 0, 0 };
        }

        public State(int[] cp, int[] co, int[] ep, int[] eo, int[] centerPoint, int[] centerRotation)
        {
            this.cp = cp;
            this.co = co;
            this.ep = ep;
            this.eo = eo;
            this.centerPoint = centerPoint;
            this.centerRotation = centerRotation;
        }
    }

    //private Dictionary<eOperationType, State> mMoveMap = new Dictionary<eOperationType, State>();
    private List<State> mStateSummary = new List<State>();

    //ステート管理系
    State mState = new State(0);

    //操作データ
    public struct OperationData
    {
        public int selectType;
        public AxisType axisType;
        public int direction;

        public OperationData(int selectType, AxisType axisType, int direction)
        {
            this.selectType = selectType;
            this.axisType = axisType;
            this.direction = direction; 
        }

        public bool IsEqual(OperationData data)
        {
            return this.selectType == data.selectType &&
                this.axisType == data.axisType &&
                this.direction == data.direction;
        }
    }

    // Dictionary<eOperationType, OperationData> mOperationDataMap = new Dictionary<eOperationType, OperationData>();
    List<OperationData> mOperationDataSummary = new List<OperationData>();

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        Factory_RubikCubes();   //キューブの生成
        CreateRotationGroup();  //グループの作成
        CreateComparisonSelectValue();

        CreateOperationMoves();
        CreateOperationDataMap();
        ColorAdjust();    //カラー調整

        mRotationParent = Instantiate(mRotationParentPrefab).GetComponent<RotationParent>();
    }

    private void Update()
    {
        InputUpdate();
    }

    void CreateOperationDataMap()
    {
        OperationData[] datas = 
        {
            new OperationData(2, AxisType.kY, 1),
            new OperationData(0, AxisType.kY, 1),
            new OperationData(0, AxisType.kX, 1),
            new OperationData(2, AxisType.kX, 1),
            new OperationData(0, AxisType.kZ, 1),
            new OperationData(2, AxisType.kZ, 1),
            new OperationData(1, AxisType.kX, 1), //XMiddle
            new OperationData(1, AxisType.kY, 1),//YMiddle
            new OperationData(1, AxisType.kZ, 1),//ZMiddle
        };

        foreach(var data in datas)
        {
            mOperationDataSummary.Add(data);
            var newData = data;
            newData.direction = -1;
            mOperationDataSummary.Add(newData);
        }
    }

    private void CreateOperationMoves()
    {
        State[] states =
        {
            //Up
            new State(
                new int[8] { 3, 0, 1, 2, 4, 5, 6, 7 },
                new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[12] { 0, 1, 2, 3, 7, 4, 5, 6, 8, 9, 10, 11 },
                new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            ),
            //Down
            new State(
                new int[8] { 0, 1, 2, 3, 5, 6, 7, 4 },
                new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[12] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 8 },
                new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            ),
            //Left
            new State(
                new int[8] { 4, 1, 2, 0, 7, 5, 6, 3 },
                new int[8] { 2, 0, 0, 1, 1, 0, 0, 2 },
                new int[12] { 11, 1, 2, 7, 4, 5, 6, 0, 8, 9, 10, 3 },
                new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            ),
            //Right
            new State(
                new int[8] { 0, 2, 6, 3, 4, 1, 5, 7 },
                new int[8] { 0, 1, 2, 0, 0, 2, 1, 0 },
                new int[12] { 0, 5, 9, 3, 4, 2, 6, 7, 8, 1, 10, 11 },
                new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            ),
            //Front
            new State(
                new int[8] { 0, 1, 3, 7, 4, 5, 2, 6 },
                new int[8] { 0, 0, 1, 2, 0, 0, 2, 1 },
                new int[12] { 0, 1, 6, 10, 4, 5, 3, 7, 8, 9, 2, 11 },
                new int[12] { 0, 0, 1, 1, 0, 0, 1, 0, 0, 0, 1, 0 }
            ),
            //Back
            new State(
                new int[8] { 1, 5, 2, 3, 0, 4, 6, 7 },
                new int[8] { 1, 2, 0, 0, 2, 1, 0, 0 },
                new int[12] { 4, 8, 2, 3, 1, 5, 6, 7, 0, 9, 10, 11 },
                new int[12] { 1, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0 }
            ),

            //XMiddle
            new State(
                new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 },
                new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[12] { 0, 1, 2, 3, 6, 5, 10, 7, 4, 9, 8, 11 },
                new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[6] { 4, 1, 5, 3, 2, 0 },
                new int[6] { 0, 0, 0, 0, 0, 0 }
            ),

            //YMiddle
            new State(
                new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 },
                new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[12] { 3, 0, 1, 2, 4, 5, 6, 7, 8, 9, 10, 11 },
                new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[6] { 3, 0, 1, 2, 4, 5 },
                new int[6] { 0, 0, 0, 0, 0, 0 }
            ),

            //ZMiddle
            new State(
                new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 },
                new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[12] { 0, 1, 2, 3, 4, 9, 6, 5, 8, 11, 10, 7 },
                new int[12] { 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1 },
                new int[6] { 0, 4, 2, 5, 3, 1 },
                new int[6] { 0, 1, 0, 1, 1, 1 }
            ),
        };

        int index = 0;
        foreach(var state in states)
        {
            mStateSummary.Add(state);
            var doubleState = AddState(state, state);
            mStateSummary.Add(AddState(doubleState, state));
            index++;
            if(index == 12)
            {
                int s = 0;
            }
        }
    }

    State AddState(State self, State move)
    {
        State newState = new State(0);

        newState.cp = AddCornerPointState(self, move);
        newState.co = AddCornerDirectionState(self, move);
        newState.ep = AddEdgePointState(self, move);
        newState.eo = AddEdgeDirectionState(self, move);
        newState.centerPoint = AddCenterPointState(self, move);
        newState.centerRotation = AddCenterRotationState(self, move);

        return newState;
    }

    private int[] AddCenterPointState(State self, State move)
    {
        var result = new int[6];

        for (int i = 0; i < result.Length; ++i)
        {
            int index = move.centerPoint[i];
            result[i] = self.centerPoint[index];
        }

        return result;
    }

    private int[] AddCenterRotationState(State self, State move)
    {
        var result = new int[6];

        for (int i = 0; i < result.Length; ++i)
        {
            int moveIndex = move.centerPoint[i];
            result[i] = (self.centerRotation[moveIndex] + move.centerRotation[i]) % 2;
        }

        return result;
    }

    private int[] AddCornerPointState(State self, State move)
    {
        var result = new int[8];

        for (int i = 0; i < result.Length; ++i)
        {
            int index = move.cp[i];
            result[i] = self.cp[index];
        }

        return result;
    }

    private int[] AddCornerDirectionState(State self, State move)
    {
        var result = new int[8];

        for (int i = 0; i < result.Length; ++i)
        {
            int moveIndex = move.cp[i];
            result[i] = (self.co[moveIndex] + move.co[i]) % 3;
        }

        return result;
    }

    private int[] AddEdgePointState(State self, State move)
    {
        var result = new int[12];

        for(int i = 0; i < result.Length; ++i)
        {
            int moveIndex = move.ep[i];
            result[i] = self.ep[moveIndex];
        }

        return result;
    }

    private int[] AddEdgeDirectionState(State self, State move)
    {
        var result = new int[12];

        for(int i = 0; i < result.Length; ++i)
        {
            int moveIndex = move.ep[i];
            result[i] = (self.eo[moveIndex] + move.eo[i]) % 2;
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

        SettingGroup(mAxisType, mSelectIndex);
        mRotationParent.StartRotation(GetAxis(mAxisType), 1);
    }

    void StartRotation(OperationData data)
    {
        if (mRotationParent.IsRotation())
        {
            return;
        }

        SettingGroup(data.axisType, data.selectType);
        mRotationParent.StartRotation(GetAxis(data.axisType), data.direction);
        mState = AddState(mState, mStateSummary[MatchIndexSummary(data)]);
    }

    private int MatchIndexSummary(OperationData currentData)
    {
        int index = 0;
        foreach(var data in mOperationDataSummary)
        {
            if (data.IsEqual(currentData)) {
                break;
            }

            index++;
        }

        return index;
    }

    private void SettingGroup(AxisType axisType, int selectIndex)
    {
        foreach(var cube in mCubes)
        {
            cube.transform.SetParent(null);
        }

        mRotationParent.transform.rotation = Quaternion.identity;

        foreach(var cube in CreateRotationGroup((int)axisType, selectIndex))
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
            //StartRotation();
            var data = new OperationData(mSelectIndex, mAxisType, +1);
            StartRotation(data);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            //StartRotation();
            var data = new OperationData(mSelectIndex, mAxisType, -1);
            StartRotation(data);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            StartRotation(mOperationDataSummary[(int)eOperationType.R]);
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

    private Vector3 GetAxis(AxisType axisType)
    {
        switch (axisType)
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
