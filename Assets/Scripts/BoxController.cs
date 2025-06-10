using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    [SerializeField] private ObjectColor objectColor;

    public ObjectColor GetObjectColor()
    {
        return objectColor;
    }
}
