using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    [SerializeField] Vector3 centerPos;
    private void Update()
    {
        MoveCursor();
    }

    void MoveCursor()
    {
       Cursor.visible = false;
       Vector3 pos = new Vector3(Input.mousePosition.x + centerPos.x, Input.mousePosition.y +centerPos.y, Input.mousePosition.z);
       transform.position = pos;
    }
}
