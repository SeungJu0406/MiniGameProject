using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDataInitialization : MonoBehaviour
{
    [SerializeField] bool isInit;
    [SerializeField] CardData data;
    [SerializeField] GameObject nameTag;
    [SerializeField] SpriteRenderer cardIcon;
    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] TextMeshProUGUI price;
    [SerializeField] TextMeshProUGUI hp;
    [SerializeField] CardModel model;
 
    [ContextMenu("Init")]
    void Init()
    {
        float fontSize = 0.2f;
        if (model != null)
        {
            model = GetComponent<CardModel>();
            model.data = data;
        }
        
        if (nameTag != null) nameTag.name = $"{data.id}. {data.engName}";
        if (cardIcon != null) cardIcon.sprite = data.cardIcon;
        if (cardName != null) 
        {
            cardName.fontStyle = FontStyles.Bold;
            cardName.fontSize = fontSize;
            cardName.text = data.cardName;            
        }
        if (price != null) 
        {
            price.fontStyle = FontStyles.Bold;
            price.fontSize = fontSize;
            price.text = $"{data.price}";
        }
        if(hp != null)
        {
            hp.fontStyle = FontStyles.Bold;
            hp.fontSize = fontSize - 0.05f;
            hp.text = $"{data.maxHp}";
        }
    }
}
