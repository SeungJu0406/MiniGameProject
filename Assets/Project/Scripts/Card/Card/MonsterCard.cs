using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    protected override void Start() 
    {
        if (!isInitInStack)
        {
            model.TopCard = this;
            model.BottomCard = this;
        }
        Manager.Sound.PlaySFX(Manager.Sound.sfx.combine);
        isInitInStack = false;
    }
    protected override void OnDisable() { }
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


    void StartBattle()
    {
        if (model.BottomCard == model.Card) return;
        BattleField battleFields = Pool.BattleField.GetPool(transform.position);
        battleFields.AddBattleList(this);
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


}
