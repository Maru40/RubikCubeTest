using System.Collections;
using System.Collections.Generic;
using System.Configuration;
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
        kZ,
    }

    private List<GroupData> mGroupDatas = new List<GroupData>();

    private Dictionary<AxisType, List<GroupData>> mGraupDatasMap = 
        new Dictionary<AxisType, List<GroupData>>();

    [SerializeField]
    private GameObject m_prefab;                                            //��������v���n�u

    [SerializeField]
    private GameObject mRotationParentPrefab;

    private RotationParent mRotationParent;

    [SerializeField]
    private float m_offsetSize = 1.0f;                                      //��������傫���̃I�t�Z�b�g

    [SerializeField]
    private float m_offsetRange = 1.0f;                                     //�����I�t�Z�b�g����

    [SerializeField]
    private Vector3 m_maxCubes = new Vector3(3.0f, 3.0f, 3.0f);             //��������L���[�u�̍ő��

    [SerializeField]
    private int mSize = 3;

    private List<GameObject> mCubes = new List<GameObject>();    //�L���[�u�̔z���ۑ�����B

    private int mSelectIndex = 0;

    private AxisType mAxisType = AxisType.kX;

    private void Start()
    {
        Factory_RubikCubes();   //�L���[�u�̐���
        CreateRotationGroup();

        mRotationParent = Instantiate(mRotationParentPrefab).GetComponent<RotationParent>();
    }

    private void Update()
    {
        InputUpdate();
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

        var datas = mGraupDatasMap[mAxisType];

        foreach (var index in datas[mSelectIndex].indices)
        {
            mCubes[index].transform.SetParent(mRotationParent.transform);
        }
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

        //�N�����v
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

            //�N�����v
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

    //��]�O���[�v�̐���
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
    /// ���ׂẴL���[�u�̐���
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
                    //�L���[�u�̐���
                    var position = firstPosition + (new Vector3(x, y, z) * m_offsetRange);
                    var cube = CreateCube(position);
                    mCubes.Add(cube);
                }
            }
        }
    }
    
    /// <summary>
    /// �L���[�u�̐���
    /// </summary>
    /// <param name="position">��������ꏊ</param>
    private GameObject CreateCube(Vector3 position)
    {
        var cube = Instantiate(m_prefab, position, Quaternion.identity, transform);
        var scale = cube.transform.localScale;
        scale *= m_offsetSize;
        cube.transform.localScale = scale;

        return cube;
    }

    /// <summary>
    /// �����ʒu�̎擾
    /// </summary>
    /// <returns>�����ʒu</returns>
    private Vector3 CalculateCubesFirstPosition()
    {
        var result = Vector3.zero;

        var halfMaxCubes = m_maxCubes / mSize;

        result.x = -halfMaxCubes.x; //x�̍����擾

        result.y = -halfMaxCubes.y; //y�̉����擾

        result.z = -halfMaxCubes.z; //z�̎�O���擾

        return result;
    }
}
