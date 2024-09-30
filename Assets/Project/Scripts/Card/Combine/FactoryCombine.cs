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
                Manager.Card.MoveResultCard(transform.position, instanceCard);
            }
        }

        // 생성 후 재료아이템 처리
        model.BottomCard = model.FactoryBottom; // 팩토리에서는 팩토리바텀부터 없앤다
        model.BottomCard.combine.CompleteCreateAllParent();
    }

    public override void CompleteCreate()
    {
        // 최초 내구도가 0이 아닌 경우에만 파괴 작업, 0은 내구도 무한
        if (model.data.durability != 0)
        {
            model.Durability--;
            // 내구도 0이하에서 오브젝트 파괴 작업
            if (model.Durability <= 0)
            {
                // 조합 리스트 정리
                // 본인 자식 리스트를 초기화 후에 자식 리스트에 하위 스택들을 넣는다
                if (model.ChildCard != null)
                {
                    model.ChildCard.model.TopCard = model.ChildCard;    //맞자식의 탑을 맞자식 본인으로 변경

                    model.ChildCard.ChangeTopAllChild(model.ChildCard);    //맞자식의 자식들의 탑을 변경

                    model.ChildCard.model.ingredients.Clear(); // 리스트 초기화

                    AddCombineListAllChild(model.ChildCard); // 자식의 리스트에 모든 자식 조합 리스트 설정

                    model.ChildCard.model.ParentCard = model.ParentCard; // 자식의 부모를 본인의 부모로 교체    
                }

                // 파괴 후 함수 강제 종료
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
