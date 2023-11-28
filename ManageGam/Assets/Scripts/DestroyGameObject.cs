using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObject : MonoBehaviour
{
    public void OnClick()
    {
        Destroy(gameObject);
    }
}
