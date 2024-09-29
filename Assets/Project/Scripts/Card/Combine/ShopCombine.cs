using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopCombine : CardCombine
{
    [SerializeField] List<Card> cards;
    [SerializeField] float moveCardPosY;
    [SerializeField] float moveCardPosX;
    protected override void Awake()
    {
        base.Awake();
        model.OnChangeBottom += BuyCard;
    }

    public override void CompleteCreate() { }
    protected override void AddCombineList() { }
    protected override void RemoveCombineList() { }
    public override void AddIngredient(CardData data) { }
    public override void RemoveIngredient(CardData data) { }

    void BuyCard()
    {
        if (model.BottomCard == model.Card) return;
        // 조합리스트에 본인을 추가
        AddShopIngredient(model.data);

        // 자식부터 맨 아래까지 리스트에 추가
        AddListAllChild();
        // 위에서부터 코인인가 아닌가 체크
        int index = 0;
        bool combineSccess = false;
        foreach (Card card in cards)
        {
            if (card.model.data.isCoin)
            {
                //코인이면 조합리스트에 코인 해당 코인을 넣는다
                combineSccess = AddShopIngredient(card.model.data);
                if (combineSccess)
                    break;
            }
            // 코인이 아니라면 반복끊고 다음 진행
            else
                break;
            index++;
        }
        //조합에 성공했을때 해당 코인 삭제 연산
        if (combineSccess)
        {
            for (int i = 0; i <= index; i++)
            {
                Destroy(cards[i].gameObject);
            }
            cards.RemoveRange(0, index + 1);
        }
        // 리스트에 대하여 설정 진행
        if (cards.Count > 0)
        {
            // 리스트 인덱스 0번째를 자식으로 지정
            model.ChildCard = cards[0];
            // 리스트 인덱스 0번째의 탑을 본인으로 지정
            cards[0].model.TopCard = cards[0];
            // 탑카드의 위치가 맵 안으로 들어올 수 있게 끔, 아래쪽으로 강제 트랜스폼 이동
            // 살짝 오른쪽이면 좋을듯
            StartCoroutine(MoveCardRoutine(model.ChildCard, moveCardPosX));

            // 리스트 인덱스 0번째의 부모를 null로 설정 후 다음 인덱스의 카드를 자식으로 지정 (없으면 null)
            // 다음 인덱스는 교체 및 전 인덱스를 부모로 지정 다음 인덱스를 자식으로 지정(없으면 null)
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].model.ParentCard = i - 1 >= 0 ? cards[i - 1] : null; // 0 보다 작은 인덱스는 존재할 수 없음
                cards[i].model.ChildCard = i + 1 < cards.Count ? cards[i + 1] : null; // 카운트 이상인 인덱스는 존재할 수 없음
            }
            // 자식(탑카드)으로 해당 자식들의 탑카드 교체
            model.ChildCard.ChangeTopAllChild(model.ChildCard);
            // 마지막 인덱스의 카드로 부모들 바텀 교체
            cards[cards.Count - 1].ChangeBottomAllParent(cards[cards.Count - 1]);
            model.ChildCard.InitOrderLayerAllChild(0);
        }

        //본인의 자식을 null 바텀을 본인으로 교체한 후 리스트와 조합리스트 비운 뒤 마무리
        model.ChildCard = null;
        model.BottomCard = model.Card;
        cards.Clear();
        model.ingredients.Clear();
    }
    void AddListAllChild()
    {
        if (model.ChildCard == null) return;
        while (model.ChildCard != null)
        {
            cards.Add(model.ChildCard);
            model.ChildCard = model.ChildCard.model.ChildCard;
        }
        model.ChildCard = cards[0];
    }
    bool AddShopIngredient(CardData data)
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

                // 바로 아래쪽에 생성
                StartCoroutine(MoveCardRoutine(instanceCard, 0));
            }
        }
    }

    WaitForSeconds moveDelay = new WaitForSeconds(0.11f);
    IEnumerator MoveCardRoutine(Card instanceCard, float moveCardPosX)
    {
        yield return moveDelay;
        Vector3 pos = new Vector3(
            transform.position.x + moveCardPosX,
            transform.position.y - (CardManager.Instance.createPosDistance + moveCardPosY),
            transform.position.z);
        while (true)
        {
            if (instanceCard == null) break;
            instanceCard.transform.position = Vector3.Lerp(instanceCard.transform.position, pos, CardManager.Instance.moveSpeed * Time.deltaTime);
            if (instanceCard.IsChoice)
            {
                yield break;
            }
            if (Vector3.Distance(instanceCard.transform.position, pos) < 0.01f)
            {
                yield break;
            }
            yield return null;
        }
    }

}
