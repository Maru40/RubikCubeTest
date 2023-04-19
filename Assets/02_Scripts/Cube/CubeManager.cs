using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    [SerializeField]
    List<RectObject> mRects = new List<RectObject>();

    [SerializeField]
    List<Material> mMaterials = new List<Material>();

    [SerializeField]
    Material mLostMaterials = null;

    private void Start()
    {
        int index = 0;

        foreach(var child in GetComponentsInChildren<Renderer>())
        {
            child.material = mMaterials[index++];
            mRects.Add(child.GetComponent<RectObject>());
        }
    }
}
