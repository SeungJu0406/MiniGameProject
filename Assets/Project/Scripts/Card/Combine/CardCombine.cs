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

    protected virtual void AddCombineList()
    {
        if (!model.CanCombine) return;
        if (model.TopCard == null) return;
        if (model.TopCard.model.IsFactory) return;
        model.TopCard.combine.AddIngredient(model.data);
    }

    protected virtual void RemoveCombineList()
    {
        if (!model.CanCombine) return;
        if (model.TopCard == null) return;
        model.TopCard.combine.RemoveIngredient(model.data);
    }
    protected virtual void AddIngredient(CardData data)
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
        if (TryCombine())
        {
            model.IsFactory = true;
        }
    }
    protected virtual void RemoveIngredient(CardData data)
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
        if (TryCombine())
        {
            model.IsFactory = true;
        }
    }
    protected bool TryCombine()
    {
        if (model.ingredients.Count <= 0) return false;
        if (model.ingredients.Count == 1 && model.ingredients[0].count <= 1) return false;


        model.ingredients.Sort((s1, s2) => s1.item.id.CompareTo(s2.item.id));
        string key = RecipeDic.Instance.GetKey(model.ingredients.ToArray());

        if (Dic.Recipe.dic.ContainsKey(key))
        {
            CraftingRecipe result = Dic.Recipe.GetValue(key);
            CreateResultCard(result);
            return true;
        }
        else
        {
            return false;
        }
    }
    protected void CreateResultCard(CraftingRecipe result)
    {
        if (result.resultItem.item == null) return;
        for (int i = 0; i < result.resultItem.count; i++)
        {
            StartCreate(result);
        }
    }
    const float DelayTime = 0.1f;
    WaitForSeconds delay = new WaitForSeconds(DelayTime);
    Coroutine createRoutine;
    protected IEnumerator CreateRoutine(CraftingRecipe result)
    {
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
        // ����
        for (int i = 0; i < result.resultItem.count; i++)
        {
            Card instanceCard = Instantiate(result.resultItem.item.prefab, transform.position, transform.rotation);
            CardManager.Instance.MoveResultCard(instanceCard, SelectRandomPos());
        }

        // ���� �� �������� ó��
        if (model.data.isFactory) model.BottomCard = model.ChildCard;
        model.BottomCard.combine.CompleteCreateParent();
    }
    protected void StartCreate(CraftingRecipe result)
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
        Vector2 dir = Random.insideUnitCircle * CardManager.Instance.createPosDistance;

        return transform.position + (Vector3)dir;
    }

    public abstract void CompleteCreate();

    protected void CompleteCreateParent()
    {
        CompleteCreate();
        if (model.ParentCard != null)
        {
            model.ParentCard.combine.CompleteCreateParent();
        }
    }
}
