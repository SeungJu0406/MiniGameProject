public class ShopItemCard : Card
{
    public override void UnClick()
    {
        base.UnClick();
        CreateRandomCard();
    }

    void CreateRandomCard()
    {
        // 1. 상점아이템 랜덤카드 리스트에서 랜덤으로 하나를 선택
        int randomIndex = Util.Random(0, model.data.randomCards.Count - 1 );
        CardData randomCard = model.data.randomCards[randomIndex];

        // 2. 해당 카드 생성
        Card instanceCard = Instantiate(randomCard.prefab, transform.position, transform.rotation);
        CardManager.Instance.MoveResultCard(transform.position, instanceCard);

        // 3. 카드 내구도 1감소
        model.Durability--;

        // 4. 카드 내구도가 0이 될때 해당 카드 삭제
        if(model.Durability <= 0)
        {
            Destroy(gameObject);
        }
    }

}
