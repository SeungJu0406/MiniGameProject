using System.Collections;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [SerializeField] public float completeResultMoveSpeed;

    [SerializeField] public float createPosDistance;

    Collider[] hits;

    public int cardLayer;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 배열 초과시 배열 두배 만드는 로직 추가
        hits = new Collider[50];
        cardLayer = LayerMask.GetMask("Card");
    }

    public void MoveResultCard(Card instanceCard, Vector3 pos)
    {
        int hitCount = Physics.OverlapSphereNonAlloc(instanceCard.transform.position, createPosDistance, hits, cardLayer);
        for (int i = 0; i < hitCount; i++)
        {
            if (hits[i] == null) break;
            Card other = hits[i].GetComponent<Card>();
            if (other.model.BottomCard.model.data == instanceCard.model.data)
            {
                instanceCard.InitInStack(other.model.BottomCard);
                return;
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
