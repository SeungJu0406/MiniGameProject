using System.Collections;
using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    public static DragNDrop Instance;

    [SerializeField] Transform cardPos;
    [SerializeField] Card choiceCard;
    [SerializeField] public float dragSpeed;
    [SerializeField] public float dragHeight;
    Vector3 movePos;
    int cardLayer;
    int backGroundLayer;

    public bool isClick { get; private set; }
    bool canClick = true;
    bool canCamareMove;
    public bool CanClick { get { return canClick; } set { canClick = value; } }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        cardLayer = LayerMask.GetMask("Card");
        backGroundLayer = LayerMask.GetMask("BackGround");
    }
    private void Start()
    {
        Manager.Game.OnDefeat += StopClick;
    }
    private void Update()
    {
        if (!CanClick) return;
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
    public void Click()
    {
        isClick = true;
        canCamareMove = true;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 100f, cardLayer))
        {
            choiceCard = hit.collider.gameObject.GetComponent<Card>();
            if (choiceCard.model.data.cantMove) return;
            cardPos = hit.transform;
            choiceCard.Click();           
        }

        dragRoutine = dragRoutine == null ? StartCoroutine(DragRoutine()) : dragRoutine;
    }
    public void Drag()
    {
        if (cardPos != null)
        {
            cardPos.position = Vector3.Lerp(cardPos.position, movePos, dragSpeed * Time.deltaTime);
        }
        else
        {
            // 레이를 찍은 곳에 반대 방향 으로 러프?
        }
    }
    Coroutine dragRoutine;
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

    public void UnClick()
    {
        isClick = false;
        canCamareMove = true;
        if (dragRoutine != null)
        {
            StopCoroutine(dragRoutine);
            dragRoutine = null;
        }
        if (cardPos != null)
        {
            choiceCard.UnClick();
            cardPos.position = new Vector3(movePos.x, movePos.y, 0);
            cardPos = null;
            choiceCard = null;
        }
    }

    void StartClick()
    {
        canClick = true;
    }
    void StopClick()
    {
        canClick = false;
    }

}
