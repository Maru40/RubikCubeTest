using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Factory_RubikCube : MonoBehaviour
{
    [SerializeField]
    private GameObject m_prefab;        //�����������v���n�u

    [SerializeField]
    //private Vector3Int m_maxNumCubes = new Vector3Int(3, 3, 3);  //�����������t�F�C�X�̐�
    private int mSides = 3;               //���ʍ�邩�ǂ���

    [SerializeField]
    private List<Material> m_materials = new List<Material>();   //�}�e���A���Q

    private List<GameObject> m_rects = new List<GameObject>();   //��������Rect���i�[����z��

    private List<List<List<GameObject>>> mCubes;    //���������L���[�u�̃J�E���g

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        CreateRubikCube();
    }

    private void CreateRubikCube()
    {
        int numOneFace = mSides * mSides;   //��ʂɕK�v�Ȑ�   
        const int NumFace = 6;              //�t�F�C�X�̐�(�����6��)

        for (int i = 0; i < NumFace; ++i)
        {
            CreateFaces(i);
        }
    }

    private void CreateFaces(int loopCount)
    {
        int numOneFace = mSides * mSides;   //��ʂɕK�v�Ȑ�
        int baseNumber = numOneFace * loopCount;

        for(int i = 0; i < numOneFace; ++i)
        {
            
        }
    }

}
