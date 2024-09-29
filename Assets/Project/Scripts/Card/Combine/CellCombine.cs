using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCombine : CardCombine
{
    [SerializeField] Card coin;
    
    [SerializeField] float moveCardPosY;
    [SerializeField] float moveCardPosX;

    List<List<Card>> lists = new List<List<Card>>();
    List<Card> coins = new List<Card>();
    List<Card> unsellables = new List<Card>();

    Collider[] hits;

    protected override void Awake()
    {
        base.Awake();
        model.OnChangeBottom += CellCards;
        lists.Add(coins);
        lists.Add(unsellables);
    }
    public override void CompleteCreate() { }

    protected override void AddCombineList() { }
    protected override void RemoveCombineList() { }
    public override void AddIngredient(CardData data) { }
    public override void RemoveIngredient(CardData data) { }

    void CellCards()
    {
        // 바텀 카드가 본인인 경우는 제외
        if (model.BottomCard == model.Card) return;

        // 마지막 자식까지 반복
        while (model.ChildCard != null)
        {
            // 자식의 가격이 0이 아닌경우
            if (model.ChildCard.model.data.price > 0)
            {
                // 가격만큼 코인 생성
                for (int i = 0; i < model.ChildCard.model.data.price; i++)
                {
                    Card instanceCard = Instantiate(coin, transform.position, transform.rotation);
                    coins.Add(instanceCard);
                }
                // 자식의 자식 캐싱
                Card child = model.ChildCard.model.ChildCard;
                // 현재 자식 삭제
                Destroy(model.ChildCard.gameObject);
                // 자식 교체
                model.ChildCard = child;
            }
            else
            {
                // 리스트에 자식 추가
                unsellables.Add(model.ChildCard);
                // 자식의 자식으로 본인 자식 교체
                model.ChildCard = model.ChildCard.model.ChildCard;
            }
        }
        //코인 리스트 설정과 , 못파는 리스트 설정
        foreach (List<Card> cards in lists)
        {
            if (cards.Count > 0)
            {
                // 리스트 인덱스 0번째를 자식으로 지정
                model.ChildCard = cards[0];
                // 리스트 인덱스 0번째의 탑을 본인으로 지정
                cards[0].model.TopCard = cards[0];
                // 탑카드의 위치가 맵 안으로 들어올 수 있게 끔, 아래쪽으로 강제 트랜스폼 이동
                // 살짝 오른쪽이면 좋을듯
                if(cards == coins)
                {
                    StartCoroutine(MoveCardRoutine(model.ChildCard, 0)); // 코인리스트는 바로 아래
                }
                else
                {
                    StartCoroutine(MoveCardRoutine(model.ChildCard, moveCardPosX)); // 못파는 리스트는 살짝 오른쪽으로
                }
                
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
        }
        // 본인의 자식을 null 바텀을 본인으로 교체한 후 리스트 인덱스 비운 뒤 마무리
        model.ChildCard = null;
        model.BottomCard = model.Card;
        lists[0].Clear();
        lists[1].Clear();
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
            instanceCard.transform.position = Vector3.Lerp(instanceCard.transform.position, pos, CardManager.Instance.completeResultMoveSpeed * Time.deltaTime);
            if (instanceCard.isChoice)
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
