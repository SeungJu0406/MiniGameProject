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
    [SerializeField] float completeResultMoveSpeed;
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
            CraftingRecipe result = Dic.Recipe.GetValue(key);
            CreateResultCard(result);
            return true;
        }
        else
        {
            return false;
        }
    }
    void CreateResultCard(CraftingRecipe result)
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
    IEnumerator CreateRoutine(CraftingRecipe result)
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
        StartCoroutine(MoveCardRoutine(instanceCard, SelectRandomPos()));
    }

    IEnumerator MoveCardRoutine(Card instanceCard, Vector3 pos)
    {
        while (true)
        {
            instanceCard.transform.position = Vector3.Lerp(instanceCard.transform.position, pos, completeResultMoveSpeed * Time.deltaTime);
            if(Vector3.Distance(instanceCard.transform.position, pos) < 0.01f)
            {
                Debug.Log("��");
                yield break;
            }
            yield return null;
        }
    }
    void StartCreate(CraftingRecipe result)
    {
        if (createRoutine == null)
        {
            createRoutine = StartCoroutine(CreateRoutine(result));
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

    const float CreatePos = 2;
    Vector3[] directions =
    {
        new Vector3(-CreatePos,CreatePos,0),    // �»�
        new Vector3(0,CreatePos,0),     // ��
        new Vector3(CreatePos,CreatePos,0),     // ���
        new Vector3(-CreatePos,0,0),    // ��
        new Vector3(CreatePos,0,0),     // ��
        new Vector3(-CreatePos,-CreatePos,0),   // ����
        new Vector3(CreatePos,-CreatePos,0),    // ����
    };
    Vector3 SelectRandomPos()
    {
        Vector3 dir = directions[Util.Random(0,directions.Length-1)];
        return new Vector3(transform.position.x + dir.x, transform.position.y + dir.y, 0);
    }
}
