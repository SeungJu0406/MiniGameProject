using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    protected void AddCombineList()
    {
        if (model.TopCard == null) return;
        model.TopCard.combine.AddIngredient(model.data);
    }

    protected void RemoveCombineList()
    {
        if (model.TopCard == null) return;
        model.TopCard.combine.RemoveIngredient(model.data);
    }
    protected void AddIngredient(CardData data)
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
        TryCombine();
    }
    protected void RemoveIngredient(CardData data)
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
        TryCombine();
    }
    protected bool TryCombine()
    {
        if (model.ingredients.Count <= 0) return false;
        if (model.ingredients.Count == 1 && model.ingredients[0].count <= 1)
            return false;

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
            if(CraftingCurTime < 0) break;
            yield return delay;          
        }
        timerBar.gameObject.SetActive(false);    
        Card instanceCard = Instantiate(result.resultItem.item.prefab, transform.position, transform.rotation);
        CardManager.Instance.MoveResultCard(instanceCard, SelectRandomPos());
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

    protected const float CreatePos = 2;
    protected Vector3[] directions =
    {
        new Vector3(-CreatePos,CreatePos,0),    // ÁÂ»ó
        new Vector3(0,CreatePos,0),     // »ó
        new Vector3(CreatePos,CreatePos,0),     // ¿ì»ó
        new Vector3(-CreatePos,0,0),    // ÁÂ
        new Vector3(CreatePos,0,0),     // ¿ì
        new Vector3(-CreatePos,-CreatePos,0),   // ÁÂÇÏ
        new Vector3(CreatePos,-CreatePos,0),    // ¿ìÇÏ
    };
    protected Vector3 SelectRandomPos()
    {
        Vector3 dir = directions[Util.Random(0,directions.Length-1)];
        return new Vector3(transform.position.x + dir.x, transform.position.y + dir.y, 0);
    }

    protected abstract void CompleteCreate();

    protected void CompleteCreateParent()
    {
        CompleteCreate();
        if(model.parentCard != null)
        {
            model.parentCard.combine.CompleteCreateParent();
        }
    }
}
