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
        // ���ո���Ʈ�� ������ �߰�
        AddShopIngredient(model.data);

        // �ڽĺ��� �� �Ʒ����� ����Ʈ�� �߰�
        AddListAllChild();
        // ���������� �����ΰ� �ƴѰ� üũ
        int index = 0;
        bool combineSccess = false;
        foreach (Card card in cards)
        {
            if (card.model.data.isCoin)
            {
                //�����̸� ���ո���Ʈ�� ���� �ش� ������ �ִ´�
                combineSccess = AddShopIngredient(card.model.data);
                if (combineSccess)
                    break;
            }
            // ������ �ƴ϶�� �ݺ����� ���� ����
            else
                break;
            index++;
        }
        //���տ� ���������� �ش� ���� ���� ����
        if (combineSccess)
        {
            for (int i = 0; i <= index; i++)
            {
                Destroy(cards[i].gameObject);
            }
            cards.RemoveRange(0, index + 1);
        }
        // ����Ʈ�� ���Ͽ� ���� ����
        if (cards.Count > 0)
        {
            // ����Ʈ �ε��� 0��°�� �ڽ����� ����
            model.ChildCard = cards[0];
            // ����Ʈ �ε��� 0��°�� ž�� �������� ����
            cards[0].model.TopCard = cards[0];
            // žī���� ��ġ�� �� ������ ���� �� �ְ� ��, �Ʒ������� ���� Ʈ������ �̵�
            // ��¦ �������̸� ������
            StartCoroutine(MoveCardRoutine(model.ChildCard, moveCardPosX));

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

        //������ �ڽ��� null ������ �������� ��ü�� �� ����Ʈ�� ���ո���Ʈ ��� �� ������
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

                // �ٷ� �Ʒ��ʿ� ����
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
