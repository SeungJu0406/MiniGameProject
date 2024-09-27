using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpriteController : MonoBehaviour
{
    [SerializeField] CardModel model;
    [SerializeField] SpriteRenderer[] renders;
    private void Awake()
    {
        model = GetComponent<CardModel>();
        renders = GetComponentsInChildren<SpriteRenderer>();

        model.OnChangeSortOrder += SetOrderInLayer;
    }

    public void SetOrderInLayer()
    {
        foreach(SpriteRenderer render in renders)
        {
            render.sortingOrder = model.SortOrder;
        }
    }
}
