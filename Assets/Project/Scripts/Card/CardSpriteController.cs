using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpriteController : MonoBehaviour
{
    [SerializeField] CardModel model;
    [SerializeField] SpriteRenderer[] renders;
    [SerializeField] Canvas[] canvases;

    int timerBarLayer;
    private void Awake()
    {
        model = GetComponent<CardModel>();
        renders = GetComponentsInChildren<SpriteRenderer>();
        canvases = GetComponentsInChildren<Canvas>();

        timerBarLayer = LayerMask.NameToLayer("TimerBar");
        string timerBarObjectName = "TimerBar";
        foreach (Canvas canvas in canvases)
        {
            if(canvas.name == timerBarObjectName)
            {
                canvas.gameObject.layer = timerBarLayer;
                canvas.sortingOrder = 5000;
            }               
        }

        model.OnChangeSortOrder += SetOrderInLayer;
    }

    public void SetOrderInLayer()
    {
        foreach(SpriteRenderer render in renders)
        {
            render.sortingOrder = model.SortOrder;
        }
        foreach(Canvas canvas in canvases)
        {
            if(canvas.gameObject.layer != timerBarLayer)
            {
                canvas.sortingOrder = model.SortOrder;
            }           
        }
    }
}
