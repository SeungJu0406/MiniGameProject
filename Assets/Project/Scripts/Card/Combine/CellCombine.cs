using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCombine : CardCombine
{
    [SerializeField] Card coin;

    [SerializeField] List<Card> unsellables = new List<Card>();

    [SerializeField] float moveCardPos;

    Collider[] hits;

    protected override void Awake()
    {
        base.Awake();
        model.OnChangeBottom += CellCards;
        hits = new Collider[20];
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
                    CreateCoin();
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
        Debug.Log("1");
        // 리스트의 카운트가 0이 아닐때
        if (unsellables.Count > 0)
        {
            // 리스트 인덱스 0번째를 자식으로 지정
            model.ChildCard = unsellables[0];
            // 리스트 인덱스 0번째의 탑을 본인으로 지정
            unsellables[0].model.TopCard = unsellables[0];
            // 탑카드의 위치가 맵 안으로 들어올 수 있게 끔, 아래쪽으로 강제 트랜스폼 이동
            Debug.Log("2");
            StartCoroutine(MoveCardRoutine(model.ChildCard));

            // 리스트 인덱스 0번째의 부모를 null로 설정 후 다음 인덱스의 카드를 자식으로 지정 (없으면 null)
            // 다음 인덱스는 교체 및 전 인덱스를 부모로 지정 다음 인덱스를 자식으로 지정(없으면 null)
            for (int i = 0; i < unsellables.Count; i++)
            {
                unsellables[i].model.ParentCard = i - 1 >= 0 ? unsellables[i - 1] : null; // 0 보다 작은 인덱스는 존재할 수 없음
                unsellables[i].model.ChildCard = i + 1 < unsellables.Count ? unsellables[i + 1] : null; // 카운트 이상인 인덱스는 존재할 수 없음
            }
            // 자식(탑카드)으로 해당 자식들의 탑카드 교체
            model.ChildCard.ChangeTopAllChild(model.ChildCard);
            // 마지막 인덱스의 카드로 부모들 바텀 교체
            unsellables[unsellables.Count - 1].ChangeBottomAllParent(unsellables[unsellables.Count - 1]);
        }

        // 본인의 자식을 null 바텀을 본인으로 교체한 후 리스트 인덱스 비운 뒤 마무리
        model.ChildCard = null;
        model.BottomCard = model.Card;
        unsellables.Clear(); 
    }
    void CreateCoin()
    {
        // 코인 생성
        Card instanceCard = Instantiate(coin, transform.position, transform.rotation);

        // 코인의 위치가 맵 안으로 들어올 수 있게 끔, 아래쪽으로 강제 트랜스폼 이동
        int hitCount = Physics.OverlapSphereNonAlloc(instanceCard.transform.position, CardManager.Instance.createPosDistance, hits, CardManager.Instance.cardLayer);
        for (int i = 0; i < hitCount; i++)
        {
            if (hits[i] == null) break;
            Card other = hits[i].GetComponent<Card>();
            if (other.model.BottomCard.model.data == instanceCard.model.data)
            {
                instanceCard.InitInStack(other.model.BottomCard);
                return;
            }
        }
        StartCoroutine(MoveCardRoutine(instanceCard));       
    }
    WaitForSeconds moveDelay = new WaitForSeconds(0.11f);
    IEnumerator MoveCardRoutine(Card instanceCard)
    {
        yield return moveDelay;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y - moveCardPos, transform.position.z);
        while (true)
        {
            Debug.Log("3");
            instanceCard.transform.position = Vector3.Lerp(instanceCard.transform.position, pos, CardManager.Instance.completeResultMoveSpeed * Time.deltaTime);
            if (instanceCard.isChoice)
            {
                Debug.Log("4");
                yield break;
            }
            if (Vector3.Distance(instanceCard.transform.position, pos) < 0.01f)
            {
                Debug.Log("5");
                yield break;
            }
            yield return null;
        }
    }
}
