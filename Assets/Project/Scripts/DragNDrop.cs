using System.Collections;
using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    [SerializeField] Transform objectPos; 
    [SerializeField] float dragSpeed;
    Vector3 movePos;
    int objectLayer;


    private void Awake()
    {
        objectLayer = LayerMask.GetMask("Object");
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
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 0.5f);
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
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 0.5f);
            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 100f))
            {
                movePos = hit.point;
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
        objectPos = null;
    }

}
