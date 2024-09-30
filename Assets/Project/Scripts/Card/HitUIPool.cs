using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public struct HitUI
{
    public GameObject canvas;
    public TextMeshProUGUI damageText;
}
public class HitUIPool : MonoBehaviour
{
    public static HitUIPool Instance;


    [SerializeField] GameObject hitCanvas;
    [SerializeField] Queue<HitUI> hitUIPool;
    [SerializeField] int size = 2;

    StringBuilder sb = new StringBuilder();
    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
        
        hitUIPool = new Queue<HitUI>(size);
        for(int i = 0; i< size; i++)
        {
            GameObject instanceUI = Instantiate(hitCanvas);
            instanceUI.gameObject.SetActive(false);
            instanceUI.transform.SetParent(transform);
            TextMeshProUGUI instanceText = instanceUI.GetComponentInChildren<TextMeshProUGUI>();
            HitUI instanceHitUI = new HitUI();
            instanceHitUI.canvas = instanceUI;
            instanceHitUI.damageText = instanceText;
            hitUIPool.Enqueue(instanceHitUI);
        }
    }

    public HitUI GetPool(Vector3 pos, int damage)
    {
        sb.Clear();
        sb.Append($"-{damage}");
        if (hitUIPool.Count > 0)
        {
            HitUI instance = hitUIPool.Dequeue();
            instance.canvas.transform.position = pos;
            instance.damageText.SetText(sb);
            instance.canvas.gameObject.SetActive(true);
            return instance;
        }
        else
        {
            GameObject instanceUI = Instantiate(hitCanvas);
            instanceUI.transform.SetParent(transform);
            TextMeshProUGUI instanceText = instanceUI.GetComponentInChildren<TextMeshProUGUI>();
            HitUI instance = new HitUI();
            instance.canvas = instanceUI;
            instance.damageText = instanceText;
            instance.canvas.transform.position = pos;
            instance.damageText.SetText(sb);
            return instance;
        }
    }

    public void ReturnPool(HitUI instance)
    {
        if (instance.canvas != null)
        {
            instance.canvas.gameObject.SetActive(false);
            hitUIPool.Enqueue(instance);
        }
    }
}
