using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCard : Card
{
    [Space(10)]
    [SerializeField] float moveCardPosY = 1;
    [SerializeField] float moveCardPosX = 2;

    List<List<Card>> lists = new List<List<Card>>();
    List<Card> coins = new List<Card>();
    List<Card> unsellables = new List<Card>();

    Collider[] hits;


    protected override void Start()
    {
        if (!isInitInStack)
        {
            model.TopCard = this;
            model.BottomCard = this;
        }
        isInitInStack = false;
        lists.Add(coins);
        lists.Add(unsellables);
    }
    protected override void OnDisable() { }

    protected override void OnTriggerEnter(Collider other) { }

    public override void IgnoreStack(Card card)
    {
        StartCoroutine(ChangeISAccessIgnoreStack(card));
        CellCards(card);
    }

    void CellCards(Card card)
    {
        // 본인이 코인 팔 수 있는지 없는지 체크하는 bool 변수
        bool isSellableTop;
        // 카드 본인에 대해 먼저 수행
        if(card.model.data.price > 0)
        {
            //Debug.Log(name);
            for(int i = 0; i< card.model.data.price; i++)
            {
                CardData coin = Dic.Card.GetValue((int)CardKey.Coin);
                Card instance = Instantiate(coin.prefab, transform.position, transform.rotation);
                coins.Add(instance);
            }
            isSellableTop = true;
        }
        else
        {
            // 리스트에 본인 추가
            unsellables.Add(card);
            isSellableTop = false;
        }

        // 카드의 마지막 자식까지 반복
        while (card.model.ChildCard != null)
        {
            // 카드의 자식의 가격이 0이 아닌경우
            if (card.model.ChildCard.model.data.price > 0)
            {
                // 가격만큼 코인 생성
                for (int i = 0; i < card.model.ChildCard.model.data.price; i++)
                {
                    CardData coin = Dic.Card.GetValue((int)CardKey.Coin);
                    Card instanceCard = Instantiate(coin.prefab, transform.position, transform.rotation);
                    coins.Add(instanceCard);
                }
                // 자식의 자식 캐싱
                Card child = card.model.ChildCard.model.ChildCard;
                // 현재 자식 삭제
                Destroy(card.model.ChildCard.gameObject);
                // 자식 교체
                card.model.ChildCard = child;
            }
            else
            {
                // 리스트에 자식 추가
                unsellables.Add(card.model.ChildCard);
                // 자식의 자식으로 본인 자식 교체
                card.model.ChildCard = card.model.ChildCard.model.ChildCard;
            }
        }
        // 매개변수가 코인 이었으면 본인도 삭제
        if (isSellableTop)
        {
            Destroy(card.gameObject);
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
        if (coins.Count > 0)
        {
            bool canStack = Manager.Card.InsertStackResultCard(coins[0].model.TopCard); // 주변에 코인있으면 코인쪽으로
            if (!canStack)
            {
                StartCoroutine(MoveCardRoutine(coins[0].model.TopCard, 0)); // 코인리스트는 살짝 아래
            }
        }
        if(unsellables.Count > 0) 
        {
            StartCoroutine(MoveCardRoutine(unsellables[0].model.TopCard, moveCardPosX)); // 못파는 리스트는 살짝 오른쪽으로
        }
        // 본인의 자식을 null 바텀을 본인으로 교체한 후 리스트 인덱스 비운 뒤 마무리
        model.ChildCard = null;
        model.BottomCard = model.Card;
        coins.Clear();
        unsellables.Clear();
    }
    WaitForSeconds moveDelay = new WaitForSeconds(0.11f);
    IEnumerator MoveCardRoutine(Card instanceCard, float moveCardPosX)
    {
        yield return moveDelay;
        Vector3 pos = new Vector3(
            transform.position.x + moveCardPosX,
            transform.position.y - (Manager.Card.createPosDistance + moveCardPosY),
            transform.position.z);
        while (true)
        {
            instanceCard.transform.position = Vector3.Lerp(instanceCard.transform.position, pos, Manager.Card.moveSpeed * Time.deltaTime);
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
    WaitForSeconds shortDelay = new WaitForSeconds(0.05f);
    IEnumerator ChangeISAccessIgnoreStack(Card card)
    {
        if (card.model.Card != null) 
        {
            card.model.IsAccessIgnoreStack = true;
        }   
        yield return shortDelay;
        if (card.model.Card != null) 
        {
            card.model.IsAccessIgnoreStack = false;
        }
    }
}
