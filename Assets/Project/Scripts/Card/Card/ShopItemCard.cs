using System.Collections;
using UnityEngine;

public class ShopItemCard : Card
{
    [SerializeField] float clickTime;
    [SerializeField] float dragTransTime;

    protected override void Start()
    {
        if (!isInitInStack)
        {
            model.TopCard = this;
            model.BottomCard = this;
        }
        Manager.Sound.PlaySFX(Manager.Sound.sfx.combine);
        isInitInStack = false;
    }
    protected override void OnDisable() { }

    public override void Click()
    {
        base.Click();

        // Ŭ���� Ÿ�̸� ����
        if (clickRoutine == null)
            clickRoutine = StartCoroutine(ClickRoutine());
    }

    Coroutine clickRoutine;
    IEnumerator ClickRoutine()
    {
        while (true)
        {
            clickTime += Time.deltaTime;
            yield return null;
        }
    }


    public override void UnClick()
    {
        base.UnClick();

        if (clickRoutine != null)
        {
            StopCoroutine(clickRoutine);
            clickRoutine = null;
        }
        // ��Ŭ�� �� ���� �ð��� ª�Ҵٸ�
        if(clickTime <= dragTransTime)
        {
            CreateRandomCard();
        }
        // Ŭ���ð� �ʱ�ȭ
        clickTime = 0;
    }

    void CreateRandomCard()
    {
        // 1. ���������� ����ī�� ����Ʈ���� �������� �ϳ��� ����
        int randomIndex = Util.Random(0, model.data.randomCards.Count - 1);
        CardData randomCard = model.data.randomCards[randomIndex];

        // 2. �ش� ī�� ����
        Card instanceCard = Instantiate(randomCard.prefab, transform.position, transform.rotation);
        bool canStack = Manager.Card.InsertStackResultCard(instanceCard);
        if (!canStack)
        {
            Manager.Card.RandomSpawnCard(transform.position, instanceCard);
        }

        // 3. ī�� ������ 1����
        model.Durability--;

        // 4. ī�� �������� 0�� �ɶ� �ش� ī�� ����
        if (model.Durability <= 0)
        {
            Destroy(gameObject);
        }
    }



}
