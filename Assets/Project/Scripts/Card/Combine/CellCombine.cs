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
        // ���� ī�尡 ������ ���� ����
        if (model.BottomCard == model.Card) return;

        // ������ �ڽı��� �ݺ�
        while (model.ChildCard != null)
        {
            // �ڽ��� ������ 0�� �ƴѰ��
            if (model.ChildCard.model.data.price > 0)
            {
                // ���ݸ�ŭ ���� ����
                for (int i = 0; i < model.ChildCard.model.data.price; i++)
                {
                    Card instanceCard = Instantiate(coin, transform.position, transform.rotation);
                    coins.Add(instanceCard);
                }
                // �ڽ��� �ڽ� ĳ��
                Card child = model.ChildCard.model.ChildCard;
                // ���� �ڽ� ����
                Destroy(model.ChildCard.gameObject);
                // �ڽ� ��ü
                model.ChildCard = child;
            }
            else
            {
                // ����Ʈ�� �ڽ� �߰�
                unsellables.Add(model.ChildCard);
                // �ڽ��� �ڽ����� ���� �ڽ� ��ü
                model.ChildCard = model.ChildCard.model.ChildCard;
            }
        }
        //���� ����Ʈ ������ , ���Ĵ� ����Ʈ ����
        foreach (List<Card> cards in lists)
        {
            if (cards.Count > 0)
            {
                // ����Ʈ �ε��� 0��°�� �ڽ����� ����
                model.ChildCard = cards[0];
                // ����Ʈ �ε��� 0��°�� ž�� �������� ����
                cards[0].model.TopCard = cards[0];
                // žī���� ��ġ�� �� ������ ���� �� �ְ� ��, �Ʒ������� ���� Ʈ������ �̵�
                // ��¦ �������̸� ������
                if(cards == coins)
                {
                    StartCoroutine(MoveCardRoutine(model.ChildCard, 0)); // ���θ���Ʈ�� �ٷ� �Ʒ�
                }
                else
                {
                    StartCoroutine(MoveCardRoutine(model.ChildCard, moveCardPosX)); // ���Ĵ� ����Ʈ�� ��¦ ����������
                }
                
                // ����Ʈ �ε��� 0��°�� �θ� null�� ���� �� ���� �ε����� ī�带 �ڽ����� ���� (������ null)
                // ���� �ε����� ��ü �� �� �ε����� �θ�� ���� ���� �ε����� �ڽ����� ����(������ null)
                for (int i = 0; i < cards.Count; i++)
                {
                    cards[i].model.ParentCard = i - 1 >= 0 ? cards[i - 1] : null; // 0 ���� ���� �ε����� ������ �� ����
                    cards[i].model.ChildCard = i + 1 < cards.Count ? cards[i + 1] : null; // ī��Ʈ �̻��� �ε����� ������ �� ����
                }
                // �ڽ�(žī��)���� �ش� �ڽĵ��� žī�� ��ü
                model.ChildCard.ChangeTopAllChild(model.ChildCard);
                // ������ �ε����� ī��� �θ�� ���� ��ü
                cards[cards.Count - 1].ChangeBottomAllParent(cards[cards.Count - 1]);
                model.ChildCard.InitOrderLayerAllChild(0);
            }
        }
        // ������ �ڽ��� null ������ �������� ��ü�� �� ����Ʈ �ε��� ��� �� ������
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
