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

        // 클릭시 타이머 시작
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
        // 언클릭 시 누른 시간이 짧았다면
        if(clickTime <= dragTransTime)
        {
            CreateRandomCard();
        }
        // 클릭시간 초기화
        clickTime = 0;
    }

    void CreateRandomCard()
    {
        // 1. 상점아이템 랜덤카드 리스트에서 랜덤으로 하나를 선택
        int randomIndex = Util.Random(0, model.data.randomCards.Count - 1);
        CardData randomCard = model.data.randomCards[randomIndex];

        // 2. 해당 카드 생성
        Card instanceCard = Instantiate(randomCard.prefab, transform.position, transform.rotation);
        bool canStack = Manager.Card.InsertStackResultCard(instanceCard);
        if (!canStack)
        {
            Manager.Card.RandomSpawnCard(transform.position, instanceCard);
        }

        // 3. 카드 내구도 1감소
        model.Durability--;

        // 4. 카드 내구도가 0이 될때 해당 카드 삭제
        if (model.Durability <= 0)
        {
            Destroy(gameObject);
        }
    }



}
