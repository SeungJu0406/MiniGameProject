using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardCombine : MonoBehaviour
{
    [SerializeField] CardModel model;
    [SerializeField] Slider timerBar;
    [SerializeField] float craftingCurTime;
    public float CraftingCurTime { get { return craftingCurTime; } set { craftingCurTime = value; OnChangeTimerBar?.Invoke(); } }
    public event UnityAction OnChangeTimerBar;


    private void Awake()
    {
        model = GetComponent<CardModel>();
        timerBar = GetComponentInChildren<Slider>();
        model.OnChangeTopBefore += RemoveCombineList;
        model.OnChangeTopAfter += AddCombineList;
        OnChangeTimerBar += UpdateTimerBar;
    }
    private void Start()
    {
        timerBar.gameObject.SetActive(false);
    }
    public void AddCombineList()
    {
        if (model.TopCard == null) return;
        model.TopCard.combine.AddIngredient(model.data);
    }

    public void RemoveCombineList()
    {
        if (model.TopCard == null) return;
        model.TopCard.combine.RemoveIngredient(model.data);
    }
    void AddIngredient(CardData data)
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
    void RemoveIngredient(CardData data)
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
    public bool TryCombine()
    {
        if (model.ingredients.Count <= 0) return false;
        if (model.ingredients.Count == 1 && model.ingredients[0].count <= 1)
            return false;

        model.ingredients.Sort((s1, s2) => s1.item.type.CompareTo(s2.item.type));
        string key = RecipeDic.Instance.GetKey(model.ingredients.ToArray());

        if (Dic.Recipe.dic.ContainsKey(key))
        {
            CraftingItemInfo result = Dic.Recipe.GetValue(key);
            CreateResultCard(result, 5 );
            return true;
        }
        else
        {
            return false;
        }
    }
    void CreateResultCard(CraftingItemInfo result, float craftingTime)
    {
        if (result.item == null) return;
        for (int i = 0; i < result.count; i++)
        {
            StartCreate(result, craftingTime);
        }
    }
    const float DelayTime = 0.1f;
    WaitForSeconds delay = new WaitForSeconds(DelayTime);
    Coroutine createRoutine;
    IEnumerator CreateRoutine(CraftingItemInfo result, float craftingTime)
    {
        timerBar.gameObject.SetActive(true);
        timerBar.maxValue = craftingTime;
        CraftingCurTime = craftingTime;    
        while (true) 
        {
            CraftingCurTime -= DelayTime;
            if(CraftingCurTime < 0) break;
            yield return delay;          
        }
        timerBar.gameObject.SetActive(false);
        Vector3 randomPos = new Vector3(transform.position.x + Random.Range(-10, 10), transform.position.y + Random.Range(-10, 10), 0);
        Instantiate(result.item.prefab, randomPos, transform.rotation);
    }
    void StartCreate(CraftingItemInfo result, float craftingTime)
    {
        if (createRoutine == null)
        {
            createRoutine = StartCoroutine(CreateRoutine(result, craftingTime));
        }
    }
    void StopCreate()
    {
        if (createRoutine != null)
        {
            StopCoroutine(createRoutine);
            createRoutine = null;
            timerBar.gameObject.SetActive(false);
        }
    }
    public void UpdateTimerBar()
    {
        timerBar.value = CraftingCurTime;
    }
}
