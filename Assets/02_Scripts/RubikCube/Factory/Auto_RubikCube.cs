using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auto_RubikCube : MonoBehaviour
{
    public enum eOperatorType
    {
        Left,
        Right,
        Up,
        Down,
        Front,
        Back,
        Middle,
        Equator,
        Standing,
    }

    [System.Serializable]
    public struct OperatorData
    {
        public eOperatorType operatorType;    //��]�^�C�v
        public Vector3 axis;    //��]��
        public float direction; //��]����
        public float speed;     //�X�s�[�h
        public List<int> memberIndices;
    }

    [SerializeField]
    Dictionary<eOperatorType, OperatorData> mOperatorDataMap = new Dictionary<eOperatorType, OperatorData>(); //����f�[�^

    [SerializeField]
    private GameObject m_prefab;                                            //��������v���n�u

    [SerializeField]
    private float m_offsetSize = 1.0f;                                      //��������傫���̃I�t�Z�b�g

    [SerializeField]
    private float m_offsetRange = 1.0f;                                     //�����I�t�Z�b�g����

    [SerializeField]
    private Vector3 m_maxCubes = new Vector3(3.0f, 3.0f, 3.0f);             //��������L���[�u�̍ő��

    //private List<List<GameObject>> m_cubes;                               //���������L���[�u�ꗗ(��{�I��27or26(���S����Ȃ�))

    private List<GameObject> mCubes = new List<GameObject>();    //�L���[�u�̔z���ۑ�����B

    //private GameObject mRotationParent = new GameObject();

    private void Start()
    {
        Factory_RubikCubes();   //�L���[�u�̐���
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
                    var cube = CreateCube(new Vector3(x, y, z) * m_offsetRange);
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

        var halfMaxCubes = m_maxCubes * 0.5f;

        result.x = -halfMaxCubes.x; //x�̍����擾

        result.y = -halfMaxCubes.y; //y�̉����擾

        result.z = -halfMaxCubes.z; //z�̎�O���擾

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
