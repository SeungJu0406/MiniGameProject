using System.Collections;
using System.Linq;
using UnityEngine;

public class FactoryCombine : CardCombine
{
    protected override void Awake()
    {
        base.Awake();
        model.OnChangeBottom += AddFactoryList;
        model.OnChangeCanFactoryCombine += StopFactoryCombine;
    }
    protected override void Start()
    {
        base.Start();
    }
    protected void AddFactoryList()
    {
        model.ingredients.Clear();
        AddFactoryCombineListAllChild(model.Card);
    }

    protected void StopFactoryCombine()
    {
        if (!model.CanFactoryCombine)
        {          
            if (createRoutine != null)
            {
                StopCoroutine(createRoutine);
                createRoutine = null;
                timerBar.gameObject.SetActive(false);
            }
        }
    }

    protected override void AddCombineList() { }
    protected override void RemoveCombineList() { }

    public override void AddIngredient(CardData data)
    {
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
        else
        {
            model.IsFactory = false;
        }
    }
    public override void RemoveIngredient(CardData data) { }
    protected override bool TryCombine()
    {
        if (model.ingredients.Count <= 0) return false;
        if (model.ingredients.Count == 1 && model.ingredients[0].count <= 1) return false;

        model.ingredients.Sort((s1, s2) => s1.item.id.CompareTo(s2.item.id));
        string key = Dic.Recipe.GetKey(model.ingredients.ToArray());

        if (Dic.Recipe.dic.ContainsKey(key))
        {
            if (!model.TopCard.model.CanFactoryCombine)
            {
                result = Dic.Recipe.GetValue(key);
                StartCreate(result);
            }         
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override IEnumerator CreateRoutine(RecipeData result)
    {
        // ���� Ÿ�̸�
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
        for (int i = 0; i < result.resultItem.Length; i++) // ��� ī�� �ε��� ����
        {
            for (int j = 0; j < result.resultItem[i].count; j++) // �ش� �ε����� ī�� count��ŭ ����
            {
                Card instanceCard = Instantiate(result.resultItem[i].item.prefab, transform.position, transform.rotation);
                Manager.Card.MoveResultCard(transform.position, instanceCard);
            }
        }

        // ���� �� �������� ó��
        model.BottomCard = model.FactoryBottom; // ���丮������ ���丮���Һ��� ���ش�
        model.BottomCard.combine.CompleteCreateAllParent();
    }

    public override void CompleteCreate()
    {
        // ���� �������� 0�� �ƴ� ��쿡�� �ı� �۾�, 0�� ������ ����
        if (model.data.durability != 0)
        {
            model.Durability--;
            // ������ 0���Ͽ��� ������Ʈ �ı� �۾�
            if (model.Durability <= 0)
            {
                // ���� ����Ʈ ����
                // ���� �ڽ� ����Ʈ�� �ʱ�ȭ �Ŀ� �ڽ� ����Ʈ�� ���� ���õ��� �ִ´�
                if (model.ChildCard != null)
                {
                    model.ChildCard.model.TopCard = model.ChildCard;    //���ڽ��� ž�� ���ڽ� �������� ����

                    model.ChildCard.ChangeTopAllChild(model.ChildCard);    //���ڽ��� �ڽĵ��� ž�� ����

                    model.ChildCard.model.ingredients.Clear(); // ����Ʈ �ʱ�ȭ

                    AddCombineListAllChild(model.ChildCard); // �ڽ��� ����Ʈ�� ��� �ڽ� ���� ����Ʈ ����

                    model.ChildCard.model.ParentCard = model.ParentCard; // �ڽ��� �θ� ������ �θ�� ��ü    
                }

                // �ı� �� �Լ� ���� ����
                Destroy(gameObject);
                return;
            }
        }
        StartCoroutine(CompleteRoutine());
    }
    WaitForSeconds restartDelay = new WaitForSeconds(0.1f);
    IEnumerator CompleteRoutine()
    {
        yield return restartDelay;
        model.IsFactory = false;
        model.CanFactoryCombine = false;
        AddFactoryList();
    }
}
