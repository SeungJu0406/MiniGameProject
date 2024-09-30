using UnityEngine;

public class FoodCard : Card
{
    protected override void Awake()
    {
        base.Awake();
        model.OnChangeParent += Use;
    }
    protected override void Start()
    {
        base.Start();
        Manager.Card.AddFoodList(this);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        Manager.Card.RemoveFoodList(this);
    }

    void Use()
    {
        Card villager = model.ParentCard;
        if (villager != null)
        {
            if (villager.model.data.isVillager)
            {
                if (model.ChildCard != null) return;
                if (villager.model.Satiety > 0)
                {
                    model.CanCombine = false;
                    villager.model.Satiety -= model.data.foodAmount;
                    if(villager.model.Satiety < 0)
                    {
                        villager.model.Satiety = 0;
                    }
                    combine.CompleteCreate();
                    return;
                }
            }
        }
        model.CanCombine = true;

    }

}
