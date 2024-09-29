public class ShopItemCard : Card
{
    public override void UnClick()
    {
        base.UnClick();
        CreateRandomCard();
    }

    void CreateRandomCard()
    {
        // 1. ���������� ����ī�� ����Ʈ���� �������� �ϳ��� ����
        int randomIndex = Util.Random(0, model.data.randomCards.Count - 1 );
        CardData randomCard = model.data.randomCards[randomIndex];

        // 2. �ش� ī�� ����
        Card instanceCard = Instantiate(randomCard.prefab, transform.position, transform.rotation);
        CardManager.Instance.MoveResultCard(transform.position, instanceCard);

        // 3. ī�� ������ 1����
        model.Durability--;

        // 4. ī�� �������� 0�� �ɶ� �ش� ī�� ����
        if(model.Durability <= 0)
        {
            Destroy(gameObject);
        }
    }

}
