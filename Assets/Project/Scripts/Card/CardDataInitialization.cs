using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDataInitialization : MonoBehaviour
{
    [SerializeField] CardData data;
    [SerializeField] GameObject nameTag;
    [SerializeField] SpriteRenderer cardIcon;
    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] CardModel model;

    [ContextMenu("Init")]
    void Init()
    {
        nameTag.name = $"{data.id}. {data.engName}";
        cardIcon.sprite = data.cardIcon;
        cardName.text = data.cardName;
        model = GetComponent<CardModel>();
        model.data = data;
    }
}
