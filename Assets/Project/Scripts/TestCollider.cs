using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollider : MonoBehaviour
{
    private void Update()
    {
        Collider[] a  = Physics.OverlapSphere(transform.position, 2);

        for(int i = 0; a.Length > i; i++)
        {
            Debug.Log($"{a[i].isTrigger}");
            
        }
    }   

}
