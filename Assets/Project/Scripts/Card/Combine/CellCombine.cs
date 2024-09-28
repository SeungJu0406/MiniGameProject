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
                    CreateCoin();
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
        Debug.Log("1");
        // ����Ʈ�� ī��Ʈ�� 0�� �ƴҶ�
        if (unsellables.Count > 0)
        {
            // ����Ʈ �ε��� 0��°�� �ڽ����� ����
            model.ChildCard = unsellables[0];
            // ����Ʈ �ε��� 0��°�� ž�� �������� ����
            unsellables[0].model.TopCard = unsellables[0];
            // žī���� ��ġ�� �� ������ ���� �� �ְ� ��, �Ʒ������� ���� Ʈ������ �̵�
            Debug.Log("2");
            StartCoroutine(MoveCardRoutine(model.ChildCard));

            // ����Ʈ �ε��� 0��°�� �θ� null�� ���� �� ���� �ε����� ī�带 �ڽ����� ���� (������ null)
            // ���� �ε����� ��ü �� �� �ε����� �θ�� ���� ���� �ε����� �ڽ����� ����(������ null)
            for (int i = 0; i < unsellables.Count; i++)
            {
                unsellables[i].model.ParentCard = i - 1 >= 0 ? unsellables[i - 1] : null; // 0 ���� ���� �ε����� ������ �� ����
                unsellables[i].model.ChildCard = i + 1 < unsellables.Count ? unsellables[i + 1] : null; // ī��Ʈ �̻��� �ε����� ������ �� ����
            }
            // �ڽ�(žī��)���� �ش� �ڽĵ��� žī�� ��ü
            model.ChildCard.ChangeTopAllChild(model.ChildCard);
            // ������ �ε����� ī��� �θ�� ���� ��ü
            unsellables[unsellables.Count - 1].ChangeBottomAllParent(unsellables[unsellables.Count - 1]);
        }

        // ������ �ڽ��� null ������ �������� ��ü�� �� ����Ʈ �ε��� ��� �� ������
        model.ChildCard = null;
        model.BottomCard = model.Card;
        unsellables.Clear(); 
    }
    void CreateCoin()
    {
        // ���� ����
        Card instanceCard = Instantiate(coin, transform.position, transform.rotation);

        // ������ ��ġ�� �� ������ ���� �� �ְ� ��, �Ʒ������� ���� Ʈ������ �̵�
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
