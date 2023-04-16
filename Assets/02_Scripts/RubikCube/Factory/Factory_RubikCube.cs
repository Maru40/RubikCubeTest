using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Factory_RubikCube : MonoBehaviour
{
    [SerializeField]
    private GameObject m_prefab;        //生成したいプレハブ

    [SerializeField]
    //private Vector3Int m_maxNumCubes = new Vector3Int(3, 3, 3);  //生成したいフェイスの数
    private int mSides = 3;               //何面作るかどうか

    [SerializeField]
    private List<Material> m_materials = new List<Material>();   //マテリアル群

    private List<GameObject> m_rects = new List<GameObject>();   //生成したRectを格納する配列

    private List<List<List<GameObject>>> mCubes;    //生成したキューブのカウント

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
        int numOneFace = mSides * mSides;   //一面に必要な数   
        const int NumFace = 6;              //フェイスの数(今回は6面)

        for (int i = 0; i < NumFace; ++i)
        {
            CreateFaces(i);
        }
    }

    private void CreateFaces(int loopCount)
    {
        int numOneFace = mSides * mSides;   //一面に必要な数
        int baseNumber = numOneFace * loopCount;

        for(int i = 0; i < numOneFace; ++i)
        {
            
        }
    }

}
