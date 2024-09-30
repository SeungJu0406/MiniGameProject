public class DisposableCombine : CardCombine
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }

    public override void PostProcessing()
    {

        model.Durability--;
        // 내구도 0이하에서 오브젝트 파괴 작업
        if (model.Durability <= 0)
        {
           
            if (model.ChildCard != null)
            {
                model.ChildCard.model.ParentCard = model.ParentCard; // 자식의 부모를 본인의 부모로 교체    
            }
            if (model.ParentCard != null)
            {
                model.ParentCard.model.ChildCard = model.ChildCard; //부모의 자식을 본인의 자식으로 교체
                                                                    // 본인이 바텀일경우
                if (model.BottomCard.combine == this)
                {
                    model.ParentCard.model.BottomCard = model.ParentCard; // 맞부모의 바텀을 맞부모 자신으로 교체
                }
                // 본인이 바텀이 아닐경우
                else
                {
                    model.ParentCard.model.BottomCard = model.BottomCard; // 맞부터의 바텀을 본인의 바텀으로 교체
                }
            }

            //본인을 조합리스트에서 제거
            RemoveCombineList();

            // 본인이 탑일 경우, 또한 자식카드가 있는 경우
            if (model.TopCard.combine == this && model.ChildCard != null)
            {
                // 본인의 조합 리스트를 맞자식의 조합리스트에 복사
                model.ChildCard.model.ingredients.Clear();
                for (int i = 0; i < model.ingredients.Count; i++)
                {
                    model.ChildCard.model.ingredients.Add(model.ingredients[i]);
                }

                model.ChildCard.model.TopCard = model.ChildCard;    //맞자식의 탑을 맞자식 본인으로 변경
                model.ChildCard.ChangeTopAllChild(model.ChildCard);    //맞자식의 자식들의 탑을 변경
            }

            Destroy(gameObject);
        }
        // 내구도가 남았을때 소모되지 않는 오브젝트처럼 처리
        else
        {
            if (model.ParentCard != null)
            {
                model.ParentCard.model.BottomCard = model.BottomCard;
            }
        }
    }

}
