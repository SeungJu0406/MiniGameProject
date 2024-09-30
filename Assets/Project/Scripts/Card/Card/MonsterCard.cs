using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MonsterCard : Card
{
    [SerializeField] BattleField battleField;

    [SerializeField] float moveDistance;
    [SerializeField] float jumpInterval;
    List<Card> notBattles = new List<Card>();
    WaitForSeconds jumpDelay;

    Coroutine idleRoutine;
    protected override void Awake()
    {
        base.Awake();
        model.OnChangeBottom += StartBattle;
        jumpDelay = new WaitForSeconds(jumpInterval);
    }

    protected override void Update()
    {
        if (model.CanGetChild)
        {
            if (idleRoutine == null)
            {
                idleRoutine = StartCoroutine(MoveIdleRoutine());
            }
        }
        else
        {
            if (idleRoutine != null)
            {
                StopCoroutine(idleRoutine);
                idleRoutine = null;
            }
        }
    }
    public override void Die()
    {
        base.Die();
    }
    IEnumerator MoveIdleRoutine()
    {
        while (true)
        {
            yield return jumpDelay;
            Vector3 dir = Random.insideUnitCircle * moveDistance;
            Vector3 pos = transform.position + dir;
            float timer = 0;
            while (Vector3.Distance(transform.position, pos) > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, pos, CardManager.Instance.moveSpeed * Time.deltaTime);
                timer += Time.deltaTime;
                if (timer > 1f)
                    break;
                yield return null;
            }          
        }
    }

    void StartBattle()
    {
        if (model.BottomCard == model.Card) return;
        BattleField battleFields = Instantiate(battleField,transform.position, transform.rotation);
        battleFields.AddBattleList(this);
    }
}
