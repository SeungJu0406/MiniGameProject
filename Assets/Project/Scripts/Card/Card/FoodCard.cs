using TMPro;
using UnityEngine;

public class FoodCard : Card
{
    [SerializeField] TextMeshProUGUI foodDurationText;
    protected override void Awake()
    {
        base.Awake();
        model.OnChangeDuration += UpdateFoodDuration;
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
    
    public void Use(Card villager)
    {
        villager.model.Satiety--;
        model.Durability--;
        Manager.Card.FoodCount--;
    }

    void UpdateFoodDuration()
    {
        sb.Clear();
        sb.Append(model.Durability);
        foodDurationText.SetText(sb);
    }
}
