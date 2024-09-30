using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class CardCombine : MonoBehaviour
{
    [SerializeField] protected CardModel model;
    [SerializeField] protected Slider timerBar;
    [SerializeField] protected float craftingCurTime;
    protected RecipeData result;

    public float CraftingCurTime { get { return craftingCurTime; } set { craftingCurTime = value; OnChangeTimerBar?.Invoke(); } }
    public event UnityAction OnChangeTimerBar;


    protected virtual void Awake()
    {
        model = GetComponent<CardModel>();
        timerBar = GetComponentInChildren<Slider>();
        model.OnChangeTopBefore += RemoveCombineList;
        model.OnChangeTopAfter += AddCombineList;
        OnChangeTimerBar += UpdateTimerBar;
    }
    protected virtual void Start()
    {
        timerBar.gameObject.SetActive(false);
    }

    public virtual void Delete()
    {
        if (model.ChildCard != null)
        {
            model.ChildCard.model.ParentCard = model.ParentCard; // 자식의 부모를 본인의 부모로 교체    
        }
        if (model.ParentCard != null)
        {
            model.ParentCard.model.ChildCard = model.ChildCard; //부모의 자식을 본인의 자식으로 교체
                                                                // 본인이 바텀일경우
            if (model.BottomCard.combine == this)
            {
                model.ParentCard.model.BottomCard = model.ParentCard; // 맞부모의 바텀을 맞부모 자신으로 교체
            }
            // 본인이 바텀이 아닐경우
            else
            {
                model.ParentCard.model.BottomCard = model.BottomCard; // 맞부터의 바텀을 본인의 바텀으로 교체
            }
        }

        // 본인이 탑일 경우, 또한 자식카드가 있는 경우
        if (model.TopCard.combine == this && model.ChildCard != null)
        {
            // 본인의 조합 리스트를 맞자식의 조합리스트에 복사
            model.ChildCard.model.ingredients.Clear();
            for (int i = 0; i < model.ingredients.Count; i++)
            {
                model.ChildCard.model.ingredients.Add(model.ingredients[i]);
            }

            model.ChildCard.model.TopCard = model.ChildCard;    //맞자식의 탑을 맞자식 본인으로 변경
            model.ChildCard.ChangeTopAllChild(model.ChildCard);    //맞자식의 자식들의 탑을 변경
            model.TopCard = model.ChildCard; // 본인도 탑을 자식으로 변경
            //본인을 조합리스트에서 제거
            RemoveCombineList();
        }

        Destroy(gameObject);
    }

    protected virtual void AddCombineList()
    {
        if (!model.CanCombine) return;
        if (model.TopCard == null) return;
        model.TopCard.combine.AddIngredient(model.data);
    }

    protected virtual void RemoveCombineList()
    {
        if (!model.CanCombine) return;
        if (model.TopCard == null) return;
        
        model.TopCard.combine.RemoveIngredient(model.data);
    }
    public virtual void AddIngredient(CardData data)
    {
        StopCreate();
        if (model.ingredients.Any(ingredients => ingredients.item.Equals(data)))
        {
            int index = model.ingredients.FindIndex(ingredients => ingredients.item.Equals(data));
            CraftingItemInfo findCard = model.ingredients[index];
            findCard.count++;
            model.ingredients[index] = findCard;
        }
        else
        {
            model.ingredients.Add(new CraftingItemInfo(data, 1));
        }
        bool success = TryCombine();
    }
    public virtual void RemoveIngredient(CardData data)
    {
        StopCreate();
        int index = model.ingredients.FindIndex(ingredients => ingredients.item.Equals(data));
        if (model.ingredients[index].count <= 1)
        {
            model.ingredients.RemoveAt(index);
        }
        else
        {
            CraftingItemInfo findCard = model.ingredients[index];
            findCard.count--;
            model.ingredients[index] = findCard;
        }
        bool success = TryCombine();
    }
    protected virtual bool TryCombine()
    {
        if (model.ingredients.Count <= 0) return false;
        if (model.ingredients.Count == 1 && model.ingredients[0].count <= 1) return false;

        model.ingredients.Sort((s1, s2) => s1.item.id.CompareTo(s2.item.id));
        string key = Dic.Recipe.GetKey(model.ingredients.ToArray());
        if (Dic.Recipe.dic.ContainsKey(key))
        {
            result = Dic.Recipe.GetValue(key);
            StartCreate(result);
            return true;
        }
        else
        {
            return false;
        }
    }
    protected const float DelayTime = 0.1f;
    protected WaitForSeconds delay = new WaitForSeconds(DelayTime);
    protected Coroutine createRoutine;
    protected virtual IEnumerator CreateRoutine(RecipeData result)
    {
        // 조합 타이머
        timerBar.gameObject.SetActive(true);
        timerBar.maxValue = result.craftingTime;
        CraftingCurTime = result.craftingTime;
        while (true)
        {
            CraftingCurTime -= DelayTime;
            if (CraftingCurTime < 0) break;
            yield return delay;
        }
        timerBar.gameObject.SetActive(false);
        // 생성
        for (int i = 0; i < result.resultItem.Length; i++) // 결과 카드 인덱스 선택
        {
            for (int j = 0; j < result.resultItem[i].count; j++) // 해당 인덱스의 카드 count만큼 생성
            {
                Card instanceCard = Instantiate(result.resultItem[i].item.prefab, transform.position, transform.rotation);
                Manager.Card.MoveResultCard(transform.position ,instanceCard);
            }
        }
        // 생성 후 재료아이템 처리
        model.BottomCard.combine.PostProcessingAllParent();
    }
    protected void StartCreate(RecipeData result)
    {
        if (createRoutine == null)
        {
            createRoutine = StartCoroutine(CreateRoutine(result));
        }
    }
    protected void StopCreate()
    {
        if (createRoutine != null)
        {
            StopCoroutine(createRoutine);
            createRoutine = null;
            timerBar.gameObject.SetActive(false);
        }
    }
    protected void UpdateTimerBar()
    {
        timerBar.value = CraftingCurTime;
    }

    protected Vector3 SelectRandomPos()
    {
        Vector2 dir = Random.insideUnitCircle * Manager.Card.createPosDistance;

        return transform.position + (Vector3)dir;
    }

    public abstract void PostProcessing();

    public void PostProcessingAllParent()
    {
        PostProcessing();
        if (model.ParentCard != null)
        {
            model.ParentCard.combine.PostProcessingAllParent();
        }
    }
    protected void AddFactoryCombineListAllChild(Card reqCard)
    {
        model.TopCard.combine.AddIngredient(reqCard.model.data);
        if (model.TopCard.model.IsFactory)
        {
            model.TopCard.model.FactoryBottom = model.Card;
            model.TopCard.model.CanFactoryCombine = true;
            return;
        }
        else
        {
            if (model.ChildCard != null)
            {
                model.ChildCard.combine.AddFactoryCombineListAllChild(model.ChildCard);
            }
            else
            {
                model.TopCard.model.CanFactoryCombine = false;
            }
        }
    }
    protected void AddCombineListAllChild(Card card)
    {
        card.combine.AddCombineList();
        if (card.model.ChildCard != null)
        {
            card.model.ChildCard.combine.AddCombineListAllChild(card.model.ChildCard);
        }
    }


}
