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
        // ������ ���� �� �� �ִ��� ������ üũ�ϴ� bool ����
        bool isSellableTop;
        // ī�� ���ο� ���� ���� ����
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
            // ����Ʈ�� ���� �߰�
            unsellables.Add(card);
            isSellableTop = false;
        }

        // ī���� ������ �ڽı��� �ݺ�
        while (card.model.ChildCard != null)
        {
            // ī���� �ڽ��� ������ 0�� �ƴѰ��
            if (card.model.ChildCard.model.data.price > 0)
            {
                // ���ݸ�ŭ ���� ����
                for (int i = 0; i < card.model.ChildCard.model.data.price; i++)
                {
                    CardData coin = Dic.Card.GetValue((int)CardKey.Coin);
                    Card instanceCard = Instantiate(coin.prefab, transform.position, transform.rotation);
                    coins.Add(instanceCard);
                }
                // �ڽ��� �ڽ� ĳ��
                Card child = card.model.ChildCard.model.ChildCard;
                // ���� �ڽ� ����
                Destroy(card.model.ChildCard.gameObject);
                // �ڽ� ��ü
                card.model.ChildCard = child;
            }
            else
            {
                // ����Ʈ�� �ڽ� �߰�
                unsellables.Add(card.model.ChildCard);
                // �ڽ��� �ڽ����� ���� �ڽ� ��ü
                card.model.ChildCard = card.model.ChildCard.model.ChildCard;
            }
        }
        // �Ű������� ���� �̾����� ���ε� ����
        if (isSellableTop)
        {
            Destroy(card.gameObject);
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
        if (coins.Count > 0)
        {
            bool canStack = Manager.Card.InsertStackResultCard(coins[0].model.TopCard); // �ֺ��� ���������� ����������
            if (!canStack)
            {
                StartCoroutine(MoveCardRoutine(coins[0].model.TopCard, 0)); // ���θ���Ʈ�� ��¦ �Ʒ�
            }
        }
        if(unsellables.Count > 0) 
        {
            StartCoroutine(MoveCardRoutine(unsellables[0].model.TopCard, moveCardPosX)); // ���Ĵ� ����Ʈ�� ��¦ ����������
        }
        // ������ �ڽ��� null ������ �������� ��ü�� �� ����Ʈ �ε��� ��� �� ������
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
