using UnityEngine;

public class FoodCard : Card
{
    protected override void Awake()
    {
        base.Awake();
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
    protected override void OnTriggerEnter(Collider other)
    {
        if (!model.CanGetParent) return;
        if (DragNDrop.Instance.isClick) return;
        if (!IsChoice) return;
        if (model.ParentCard != null) return;
        if (model.IsFight) return;
        if (model.IsAccessIgnoreStack) return;
        if (other.gameObject.layer == cardLayer)
        {
            Card parent = other.gameObject.GetComponent<Card>();
            if (parent.model.data.isIgnoreStack)
            {
                parent.IgnoreStack(this);
                return;
            }
            if (!parent.model.CanGetChild) return;
            if (model.TopCard == parent.model.TopCard) return;
            if (parent.model.ChildCard != null) return;
            // �θ� �ڽ� ī�� ����
            if (parent.model.data.isVillager && parent.model.Satiety > 0)
            {
                Use(parent);
            }
            else
            {
                model.ParentCard = parent;
                parent.model.ChildCard = this;
                ChangeOrderLayerAllChild();
                ChangeTopAllChild(parent.model.TopCard); // ���� + �ڽĿ��� top ����           
                ChangeBottomAllParent(model.BottomCard); // ���� + �θ𿡰� bottom ����
                parent.rb.velocity = Vector3.zero;
            }
        }
    }
    public void Use(Card villager)
    {
        villager.model.Satiety -= model.data.foodAmount;
        if (villager.model.Satiety < 0)
        {
            villager.model.Satiety = 0;
        }
        combine.Delete();
    }

}
