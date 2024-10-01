using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalCard : MonsterCard
{

    WaitForSeconds produceDelay;

    protected override void Start()
    {
        base.Start();

        produceDelay = new WaitForSeconds(model.data.produceCard.time);
        StartCoroutine(ProduceCardRoutine());
    }

    IEnumerator ProduceCardRoutine()
    {
        while (true)
        {
            yield return produceDelay;
            Card produceCard = Instantiate(model.data.produceCard.card.prefab, transform.position, transform.rotation);
            if (!Manager.Card.InsertStackResultCard(produceCard))
            {
                Manager.Card.RandomSpawnCard(transform.position, produceCard);
            }
        }
    }
}
