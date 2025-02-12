using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [SerializeField] Transform cardPos;
    [SerializeField] Card choiceCard;
    [SerializeField] public float dragSpeed;
    [HideInInspector] public Plane _backGroundPlane;
    Vector3 movePos;
    int cardLayer;
    int backGroundLayer;

    public bool isClick { get; private set; }
    bool canClick = true;
    public bool CanClick { get { return canClick; } set { canClick = value; } }
    bool canCamareMove;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenOrCloseMenu();
        }
        if (CanClick)
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Manager.Time.TimeScale > 0f)
            {
                Manager.Time.Pause();
            }
            else
            {
                Manager.Time.Normal();
            }
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
    }
    public void Drag()
    {

        Vector3 distanceFormCamera = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 15);
        Plane plane = new Plane(Vector3.forward, distanceFormCamera);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 point = ray.GetPoint(enter);
            movePos = new Vector3(point.x, point.y, 0);
        }
       // Debug.Log(Util.GetDeltaTime());
        if (cardPos != null)
        {
            cardPos.position = Vector3.Lerp(cardPos.position, movePos, dragSpeed * Time.deltaTime);          ;
        }
        else
        {
            // 레이를 찍은 곳에 반대 방향 으로 러프?
        }
    }
    public void UnClick()
    {
        isClick = false;
        canCamareMove = true;

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

    public void OpenOrCloseMenu()
    {
        if (Manager.UI.menuUI.isMenuUi) // 메뉴 열렸을때
        {
            Manager.Time.Normal();
            Manager.UI.HideMenuUI();
        }
        else if (Manager.UI.optionUI.isOptineUi)
        {
            Manager.Time.Normal();
            Manager.UI.HideOptionUI();
        }
        else
        {
            Manager.Time.Pause();
            Manager.UI.ShowMenuUI();
        }
    }

}
