using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    [SerializeField] Transform objectPos;
    [SerializeField] float dragSpeed;
    [SerializeField] float dragHeight;
    Vector3 movePos;
    int objectLayer;
    int backGroundLayer;

    private void Awake()
    {
        objectLayer = LayerMask.GetMask("Card");
        backGroundLayer = LayerMask.GetMask("BackGround");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Click();
        }
        else if (Input.GetMouseButton(0))
        {
            Drag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            UnClick();
        }
    }
    void Click()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 100f, objectLayer))
        {
            objectPos = hit.transform;
        }
        if (dragRoutine == null)
        {
            StartCoroutine(DragRoutine());
        }
    }

    Coroutine dragRoutine;
    private void Drag()
    {
        if (objectPos != null)
        {
            objectPos.position = Vector3.Lerp(objectPos.position, movePos, dragSpeed * Time.deltaTime);
        }
    }
    WaitForSeconds delay = new WaitForSeconds(0.05f);
    IEnumerator DragRoutine()
    {
        while (true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 100f, backGroundLayer))
            {
                movePos = new Vector3(hit.point.x, hit.point.y, hit.point.z - dragHeight);
            }
            yield return delay;
        }
    }

    void UnClick()
    {
        if (dragRoutine != null)
        {
            StopCoroutine(dragRoutine);
            dragRoutine = null;
        }
        if (objectPos != null)
        {
            objectPos.position = new Vector3(movePos.x, movePos.y, 0);
            objectPos = null;
        }
    }

}
