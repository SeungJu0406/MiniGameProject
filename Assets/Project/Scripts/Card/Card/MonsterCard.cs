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
        // �ο� ���� ���� �� ����
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
        // �ֹ��� �ƴ� ī��� �������;� ��
        // ��� �Ʒ� ����ī�带 notBattles ���ÿ� �߰�
        if (model.ChildCard == null) return;
        while (model.ChildCard != null)
        {
            // �ֹ��� �ֹ� ����Ʈ�� �߰�
            if (model.ChildCard.model.data.isVillager)
            {
                villagers.Add(model.ChildCard);
            }
            // ���ʹ� ���� ����Ʈ�� �߰�
            else if (model.ChildCard.model.data.isMonster)
            {
                monsters.Add(model.ChildCard);
            }
            // �ο�� ���°� ���� ����Ʈ�� �߰�
            else
            {
                notBattles.Add(model.ChildCard);
            }
            model.ChildCard = model.ChildCard.model.ChildCard;
        }

        // �ο�� ���� ����Ʈ�� ���Ͽ� ���� ����
        if (notBattles.Count > 0)
        {
            // ����Ʈ �ε��� 0��°�� �ڽ����� ����
            model.ChildCard = notBattles[0];
            // ����Ʈ �ε��� 0��°�� ž�� �������� ����
            notBattles[0].model.TopCard = notBattles[0];
            // žī���� ��ġ�� �� ������ ���� �� �ְ� ��, �Ʒ������� ���� Ʈ������ �̵�
            // ��¦ �������̸� ������
            StartCoroutine(MoveCardRoutine(model.ChildCard));

            // ����Ʈ �ε��� 0��°�� �θ� null�� ���� �� ���� �ε����� ī�带 �ڽ����� ���� (������ null)
            // ���� �ε����� ��ü �� �� �ε����� �θ�� ���� ���� �ε����� �ڽ����� ����(������ null)
            for (int i = 0; i < notBattles.Count; i++)
            {
                notBattles[i].model.ParentCard = i - 1 >= 0 ? notBattles[i - 1] : null; // 0 ���� ���� �ε����� ������ �� ����
                notBattles[i].model.ChildCard = i + 1 < notBattles.Count ? notBattles[i + 1] : null; // ī��Ʈ �̻��� �ε����� ������ �� ����
            }
            // �ڽ�(žī��)���� �ش� �ڽĵ��� žī�� ��ü
            model.ChildCard.ChangeTopAllChild(model.ChildCard);
            // ������ �ε����� ī��� �θ�� ���� ��ü
            notBattles[notBattles.Count - 1].ChangeBottomAllParent(notBattles[notBattles.Count - 1]);
            model.ChildCard.InitOrderLayerAllChild(0);
        }
        //�ش� ����Ʈ ���� ������
        model.ChildCard = null;
        model.BottomCard = this;
        notBattles.Clear();
        // �߰��� ��� �ε����� ���� žī��� ����ī�� ���� ���� �� �θ� �ڽ� null ��ü
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
            // �ֹ�ī��� �ڽ��� ������ ������ ����
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
