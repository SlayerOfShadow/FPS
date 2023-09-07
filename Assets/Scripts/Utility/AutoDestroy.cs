using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] float time;

    void Start()
    {
        Invoke("DestroyObject", time);
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
