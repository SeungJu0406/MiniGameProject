using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalCard : MonsterCard
{
    protected override void Start()
    {
        base.Start();

        StartCoroutine(ProduceCardRoutine());
    }

    IEnumerator ProduceCardRoutine()
    {
        while (true)
        {
            float delayTime = 0;
            while (true)
            {
                delayTime += Util.GetDeltaTime();
                if (delayTime > model.data.produceCard.time)
                    break;
                yield return null;
            }

            Card produceCard = Instantiate(model.data.produceCard.card.prefab, transform.position, transform.rotation);
            if (!Manager.Card.InsertStackResultCard(produceCard))
            {
                Manager.Card.RandomSpawnCard(transform.position, produceCard);
            }
        }
    }
}
