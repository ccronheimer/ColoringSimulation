using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyMe : MonoBehaviour
{
    void Awake()
    {
        // carry on to next scene
        DontDestroyOnLoad(this.gameObject);

    }
}
