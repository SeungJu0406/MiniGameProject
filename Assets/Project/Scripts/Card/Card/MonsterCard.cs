using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCard : Card
{
    [Space(10)]
    [SerializeField] List<Card> monsters = new List<Card>();
    [SerializeField] int monstersIndex;
    [SerializeField] List<Card> villagers = new List<Card>();
    [SerializeField] int villagersIndex;
    [Space(10)]
    [SerializeField] float attackInterval = 2;
    // [SerializeField] float attackRange = 2.5f;
    [SerializeField] float hitUIDuration = 1;
    [SerializeField] float moveDistance;
    [SerializeField] float jumpInterval;
    List<Card> notBattles = new List<Card>();
    Vector3 battlePos;
    WaitForSeconds battleDelay;
    WaitForSeconds hitUIDelay;
    WaitForSeconds jumpDelay;

    Coroutine battleRoutine;
    Coroutine idleRoutine;
    protected override void Awake()
    {
        base.Awake();
        monsters.Add(this);
        model.OnChangeBottom += AddBattleList;
        battleDelay = new WaitForSeconds(attackInterval);
        hitUIDelay = new WaitForSeconds(hitUIDuration);
        jumpDelay = new WaitForSeconds(jumpInterval);
    }

    protected override void Update()
    {
        if (CheckIsFight())
        {
            if (idleRoutine != null) 
            {
                StopCoroutine(idleRoutine);
                idleRoutine = null;
            }
            if (battleRoutine == null)
            {
                battlePos = transform.position;
                battleRoutine = StartCoroutine(BattleRoutine());
            }
            MoveBattle();
        }
        else
        {
            if (idleRoutine == null) 
            {
                idleRoutine = StartCoroutine(MoveIdleRoutine());
            }
            if (battleRoutine != null)
            {
                StopCoroutine(battleRoutine);
                battleRoutine = null;
            }
        }
    }
    public override void Die()
    {
        base.Die();
    }

    bool CheckIsFight()
    {
        // 싸울 적이 있을 떄 전투
        return villagers.Count > 0 ? true : false;
    }
    IEnumerator MoveIdleRoutine()
    {
        while (true)
        {
            yield return jumpDelay;
            Vector3 dir = Random.insideUnitCircle * moveDistance;
            Vector3 pos = transform.position + dir;
            while (Vector3.Distance(transform.position, pos) > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, pos, CardManager.Instance.moveSpeed * Time.deltaTime);
                yield return null;
            }          
        }
    }
    void MoveBattle()
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            Vector3 pos = new Vector3(battlePos.x + i, battlePos.y, battlePos.z);
            monsters[i].transform.position = Vector3.Lerp(monsters[i].transform.position, pos, CardManager.Instance.moveSpeed * Time.deltaTime);
        }
        for (int i = 0; i < villagers.Count; i++)
        {
            Vector3 pos = new Vector3(battlePos.x + i + (1 * i), battlePos.y - 3, battlePos.z);
            villagers[i].transform.position = Vector3.Lerp(villagers[i].transform.position, pos, CardManager.Instance.moveSpeed * Time.deltaTime);
        }
    }
   
    IEnumerator BattleRoutine()
    {
        yield return battleDelay;
        while (true)
        {
            for (int i = 0; i < Util.Random(0, villagers.Count); i++)
            {
                int targetIndex = Util.Random(0, monstersIndex - 1);
                StartCoroutine(AttackRoutine(villagers[Util.Random(0, villagers.Count - 1)], monsters[targetIndex]));
                yield return battleDelay;
            }
            for (int i = 0; i < Util.Random(0, monsters.Count); i++)
            {
                int targetIndex = Util.Random(0, villagersIndex - 1);
                StartCoroutine(AttackRoutine(monsters[Util.Random(0, monsters.Count - 1)], villagers[targetIndex]));
                yield return battleDelay;
            }
        }
    }

    IEnumerator AttackRoutine(Card attacker, Card hitCard)
    {
        if (attacker == null || hitCard == null) yield break;

        attacker.model.IsAttack = true;
        Vector3 originPos = attacker.transform.position;
        float timer = 0;
        //while (Vector3.Distance(attacker.transform.position, hitCard.transform.position) > attackRange)
        while (true)
        {
            attacker.transform.position = Vector3.Lerp(attacker.transform.position, hitCard.transform.position, CardManager.Instance.moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            if (attacker.isChoice) break;
            if (timer > 0.1f)
                break;
            yield return null;
        }
        hitCard.model.CurHp -= attacker.model.Damage;
        StartCoroutine(HitUIRoutine(attacker, hitCard));
        if (hitCard.model.CurHp <= 0)
        {
            UnPrintHitUI(attacker);
            hitCard.Die();
        }
        while (Vector3.Distance(attacker.transform.position, originPos) > 0.01f)
        {
            attacker.transform.position = Vector3.Lerp(attacker.transform.position, originPos, CardManager.Instance.moveSpeed * Time.deltaTime);
            if (attacker.isChoice) break;
            yield return null;
        }
        attacker.model.IsAttack = false;
    }
    IEnumerator HitUIRoutine(Card attacker, Card hitCard)
    {
        PrintHitUI(attacker, hitCard);
        yield return hitUIDelay;
        UnPrintHitUI(attacker);
    }

    void PrintHitUI(Card attacker, Card hitCard)
    {
        attacker.hitUI.transform.SetParent(null);
        attacker.hitUI.transform.position = hitCard.transform.position;
        attacker.hitUI.gameObject.SetActive(true);
    }
    void UnPrintHitUI(Card attacker)
    {
        attacker.hitUI.gameObject.SetActive(false);
        attacker.hitUI.transform.position = attacker.transform.position;
        attacker.hitUI.transform.SetParent(attacker.transform);
    }

    void AddBattleList()
    {
        // 주민이 아닌 카드는 빠져나와야 함
        // 모든 아래 스택카드를 notBattles 스택에 추가
        if (model.ChildCard == null) return;
        while (model.ChildCard != null)
        {
            // 주민은 주민 리스트에 추가
            if (model.ChildCard.model.data.isVillager)
            {
                villagers.Add(model.ChildCard);
            }
            // 몬스터는 몬스터 리스트에 추가
            else if (model.ChildCard.model.data.isMonster)
            {
                monsters.Add(model.ChildCard);
            }
            // 싸울수 없는건 따른 리스트에 추가
            else
            {
                notBattles.Add(model.ChildCard);
            }
            model.ChildCard = model.ChildCard.model.ChildCard;
        }

        // 싸울수 없는 리스트에 대하여 설정 진행
        if (notBattles.Count > 0)
        {
            // 리스트 인덱스 0번째를 자식으로 지정
            model.ChildCard = notBattles[0];
            // 리스트 인덱스 0번째의 탑을 본인으로 지정
            notBattles[0].model.TopCard = notBattles[0];
            // 탑카드의 위치가 맵 안으로 들어올 수 있게 끔, 아래쪽으로 강제 트랜스폼 이동
            // 살짝 오른쪽이면 좋을듯
            StartCoroutine(MoveCardRoutine(model.ChildCard));

            // 리스트 인덱스 0번째의 부모를 null로 설정 후 다음 인덱스의 카드를 자식으로 지정 (없으면 null)
            // 다음 인덱스는 교체 및 전 인덱스를 부모로 지정 다음 인덱스를 자식으로 지정(없으면 null)
            for (int i = 0; i < notBattles.Count; i++)
            {
                notBattles[i].model.ParentCard = i - 1 >= 0 ? notBattles[i - 1] : null; // 0 보다 작은 인덱스는 존재할 수 없음
                notBattles[i].model.ChildCard = i + 1 < notBattles.Count ? notBattles[i + 1] : null; // 카운트 이상인 인덱스는 존재할 수 없음
            }
            // 자식(탑카드)으로 해당 자식들의 탑카드 교체
            model.ChildCard.ChangeTopAllChild(model.ChildCard);
            // 마지막 인덱스의 카드로 부모들 바텀 교체
            notBattles[notBattles.Count - 1].ChangeBottomAllParent(notBattles[notBattles.Count - 1]);
            model.ChildCard.InitOrderLayerAllChild(0);
        }
        //해당 리스트 비우고 마무리
        model.ChildCard = null;
        model.BottomCard = this;
        notBattles.Clear();
        // 추가된 모든 인덱스에 대해 탑카드와 바텀카드 본인 지정 후 부모 자식 null 교체
        for (int i = monstersIndex; i < monsters.Count; i++)
        {
            monsters[i].model.ParentCard = null;
            monsters[i].model.ChildCard = null;
            monsters[i].model.TopCard = this;
            monsters[i].model.BottomCard = this;
            monsters[i].boxCollider.isTrigger = true;
            monsters[i].InitOrderLayerAllChild(0);
            monsters[i].OnDie += RemoveVillagerList;
            monstersIndex++;
        }
        for (int i = villagersIndex; i < villagers.Count; i++)
        {
            villagers[i].model.ParentCard = null;
            villagers[i].model.ChildCard = null;
            villagers[i].model.TopCard = this;
            villagers[i].model.BottomCard = this;
            villagers[i].boxCollider.isTrigger = true;
            villagers[i].InitOrderLayerAllChild(0);
            villagers[i].OnClick += RemoveVillagerList;
            villagers[i].OnDie += RemoveVillagerList;
            // 주민카드는 자식을 가질수 없도록 세팅
            villagers[i].model.CanGetChild = false;
            villagersIndex++;
        }
    }

    void RemoveVillagerList(Card remover)
    {

        remover.OnDie -= RemoveVillagerList;
        remover.OnClick -= RemoveVillagerList;

        remover.boxCollider.isTrigger = false;
        if (remover.model.data.isVillager)
        {
            villagersIndex--;
            villagers.Remove(remover);
        }
        else
        {
            monstersIndex--;
            monsters.Remove(remover);
        }
    }

    WaitForSeconds moveDelay = new WaitForSeconds(0.11f);
    IEnumerator MoveCardRoutine(Card instanceCard)
    {
        yield return moveDelay;
        Vector3 pos = new Vector3(
            transform.position.x - (CardManager.Instance.createPosDistance),
            transform.position.y,
            transform.position.z);
        while (true)
        {
            if (instanceCard == null) break;
            instanceCard.transform.position = Vector3.Lerp(instanceCard.transform.position, pos, CardManager.Instance.moveSpeed * Time.deltaTime);
            if (instanceCard.isChoice)
            {
                yield break;
            }
            if (Vector3.Distance(instanceCard.transform.position, pos) < 0.01f)
            {
                yield break;
            }
            yield return null;
        }
    }


}
