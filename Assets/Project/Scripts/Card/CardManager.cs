using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [SerializeField] float completeResultMoveSpeed;

    [SerializeField] public float createPosDistance;

    Collider[] hits;

    public int cardLayer;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        hits = new Collider[30];
        cardLayer = LayerMask.GetMask("Card");
    }

    public void MoveResultCard(Card instanceCard, Vector3 pos)
    {
        int hitCount = Physics.OverlapSphereNonAlloc(instanceCard.transform.position, createPosDistance, hits, cardLayer);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null) break;
            if (hits[i].isTrigger == false)
            {
                Card other = hits[i].GetComponent<Card>();
                if (other.model.data == instanceCard.model.data) 
                {
                    instanceCard.InitInStack(other);
                    return;
                }
            }
        }
        StartCoroutine(MoveCardRoutine(instanceCard, pos));
    }

    IEnumerator MoveCardRoutine(Card instanceCard, Vector3 pos)
    {
        while (true)
        {
            instanceCard.transform.position = Vector3.Lerp(instanceCard.transform.position, pos, completeResultMoveSpeed * Time.deltaTime);
            if (Vector3.Distance(instanceCard.transform.position, pos) < 0.01f)
            {
                yield break;
            }
            yield return null;
        }
    }
}
