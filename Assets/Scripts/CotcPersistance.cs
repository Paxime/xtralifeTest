using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CotcSdk;

public class CotcPersistance : MonoBehaviour
{
    // For one instance in scene
    private void Awake()
    {
        var ct = FindObjectOfType<CotcGameObject>();
        if (ct != null && ct.gameObject != this.gameObject)
        {
            Destroy(this.gameObject);
        }
    }
}
