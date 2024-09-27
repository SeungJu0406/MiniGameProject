using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpriteController : MonoBehaviour
{
    [SerializeField] CardModel model;
    [SerializeField] SpriteRenderer[] renders;
    [SerializeField] Canvas[] canvases;
    private void Awake()
    {
        model = GetComponent<CardModel>();
        renders = GetComponentsInChildren<SpriteRenderer>();
        canvases = GetComponentsInChildren<Canvas>();

        model.OnChangeSortOrder += SetOrderInLayer;
    }

    public void SetOrderInLayer()
    {
        foreach(SpriteRenderer render in renders)
        {
            render.sortingOrder = model.SortOrder;
        }
        foreach(Canvas render in canvases)
        {
            render.sortingOrder = model.SortOrder;
        }
    }
}
