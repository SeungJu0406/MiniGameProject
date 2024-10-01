using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField : Card
{
    [Space(10)]
    [SerializeField] List<Card> monsters = new List<Card>();
    [SerializeField] int monstersIndex;
    [SerializeField] List<Card> villagers = new List<Card>();
    [SerializeField] int villagersIndex;
    [Space(10)]
    [SerializeField] float attackInterval = 2;
    [SerializeField] float hitUIDuration = 1;

    List<Card> notBattles = new List<Card>();
    //Vector3 battlePos;
    WaitForSeconds battleDelay;
    WaitForSeconds hitUIDelay;

    Coroutine battleRoutine;

    HitUI hitUI;
    bool canAddList;
    protected override void Awake()
    {
        base.Awake();
        model.OnChangeBottom += AddBattleListAllChild;
        battleDelay = new WaitForSeconds(attackInterval);
        hitUIDelay = new WaitForSeconds(hitUIDuration);
    }
    protected override void Start()
    {
        if (!isInitInStack)
        {
            model.TopCard = this;
            model.BottomCard = this;
        }
    }
    private void OnEnable()
    {
        monstersIndex = 0;
        villagersIndex = 0;
    }
    protected override void OnDisable() { }
    protected override void Update()
    {
        if (CheckIsFight())
        {
            if (battleRoutine == null)
            {
                battleRoutine = StartCoroutine(BattleRoutine());
            }
            MoveBattle();
        }
        else
        {
            if (battleRoutine != null)
            {
                StopCoroutine(battleRoutine);
                battleRoutine = null;
            }
            EndBattle();
        }
    }

    bool CheckIsFight()
    {
        if (monsters.Count > 0)
        {
            if (villagers.Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    void MoveBattle()
    {
        int sizeX = monstersIndex * 2;
        int interval = sizeX / monsters.Count;
        for (int i = 0; i < monsters.Count; i++)
        {
            Vector3 pos = new Vector3(transform.position.x - (monstersIndex - 1) + (interval * i), transform.position.y + 1.5f, transform.position.z);
            monsters[i].transform.position = Vector3.Lerp(monsters[i].transform.position, pos, Manager.Card.moveSpeed * Time.deltaTime);
        }
        sizeX = villagersIndex * 2;
        interval = sizeX / villagers.Count;

        for (int i = 0; i < villagers.Count; i++)
        {
            Vector3 pos = new Vector3(transform.position.x - (villagersIndex - 1) + (interval * i), transform.position.y - 1.5f, transform.position.z);
            villagers[i].transform.position = Vector3.Lerp(villagers[i].transform.position, pos, Manager.Card.moveSpeed * Time.deltaTime);
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
        while (true)
        {
            attacker.transform.position = Vector3.Lerp(attacker.transform.position, hitCard.transform.position, Manager.Card.moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            if (attacker.IsChoice) break;
            if (timer > 0.1f)
                break;
            yield return null;
        }
        hitCard.model.CurHp -= attacker.model.Damage;
        StartCoroutine(HitUIRoutine(attacker, hitCard));
        if (hitCard.model.CurHp <= 0)
        {
            hitCard.Die();
        }
        while (Vector3.Distance(attacker.transform.position, originPos) > 0.01f)
        {
            attacker.transform.position = Vector3.Lerp(attacker.transform.position, originPos, Manager.Card.moveSpeed * Time.deltaTime);
            if (attacker.IsChoice) break;
            yield return null;
        }
        attacker.model.IsAttack = false;
    }
    IEnumerator HitUIRoutine(Card attacker, Card hitCard)
    {
        hitUI = HitUIPool.Instance.GetPool(hitCard.transform.position, attacker.model.Damage);
        yield return hitUIDelay;
        HitUIPool.Instance.ReturnPool(hitUI);
    }

    void AddBattleListAllChild()
    {
        if (model.BottomCard != this)
        {
            AddBattleList(model.ChildCard);
        }
    }

    public void AddBattleList(Card top)
    {

        if (top.model.data.isVillager)
        {
            villagers.Add(top);
        }
        else if (top.model.data.isMonster)
        {

            monsters.Add(top);
        }
        else
        {
            notBattles.Add(top);
        }

        // 주민이 아닌 카드는 빠져나와야 함
        // 모든 아래 스택카드를 notBattles 스택에 추가
        while (top.model.ChildCard != null)
        {

            // 주민은 주민 리스트에 추가
            if (top.model.ChildCard.model.data.isVillager)
            {
                villagers.Add(top.model.ChildCard);
            }
            // 몬스터는 몬스터 리스트에 추가
            else if (top.model.ChildCard.model.data.isMonster)
            {
                monsters.Add(top.model.ChildCard);
            }
            // 싸울수 없는건 따른 리스트에 추가
            else
            {
                notBattles.Add(top.model.ChildCard);
            }

            top.model.ChildCard = top.model.ChildCard.model.ChildCard;
        }


        // 싸울수 없는 리스트에 대하여 설정 진행
        if (notBattles.Count > 0)
        {
            // 리스트 인덱스 0번째를 자식으로 지정
            top.model.ChildCard = notBattles[0];
            // 리스트 인덱스 0번째의 탑을 본인으로 지정
            notBattles[0].model.TopCard = notBattles[0];

            // 리스트 인덱스 0번째의 부모를 null로 설정 후 다음 인덱스의 카드를 자식으로 지정 (없으면 null)
            // 다음 인덱스는 교체 및 전 인덱스를 부모로 지정 다음 인덱스를 자식으로 지정(없으면 null)
            for (int i = 0; i < notBattles.Count; i++)
            {
                notBattles[i].model.ParentCard = i - 1 >= 0 ? notBattles[i - 1] : null; // 0 보다 작은 인덱스는 존재할 수 없음
                notBattles[i].model.ChildCard = i + 1 < notBattles.Count ? notBattles[i + 1] : null; // 카운트 이상인 인덱스는 존재할 수 없음
            }
            // 자식(탑카드)으로 해당 자식들의 탑카드 교체
            top.ChangeTopAllChild(model.ChildCard);
            // 마지막 인덱스의 카드로 부모들 바텀 교체
            notBattles[notBattles.Count - 1].ChangeBottomAllParent(notBattles[notBattles.Count - 1]);
            top.InitOrderLayerAllChild(0);
        }
        //해당 리스트 비우고 마무리
        notBattles.Clear();
        // 추가된 모든 인덱스에 대해 탑카드와 바텀카드 본인 지정 후 부모 자식 null 교체
        for (int i = monstersIndex; i < monsters.Count; i++)
        {
            monsters[i].model.ParentCard = null;
            monsters[i].model.ChildCard = null;
            monsters[i].model.TopCard = monsters[i];
            monsters[i].model.BottomCard = monsters[i];
            monsters[i].boxCollider.isTrigger = true;
            monsters[i].InitOrderLayerAllChild(0);
            monsters[i].OnDie += RemoveVillagerList;
            monsters[i].model.CanGetChild = false;
            monsters[i].model.IsFight = true;
            monstersIndex++;
        }
        for (int i = villagersIndex; i < villagers.Count; i++)
        {

            villagers[i].model.ParentCard = null;
            villagers[i].model.ChildCard = null;
            villagers[i].model.TopCard = villagers[i];
            villagers[i].model.BottomCard = villagers[i];
            villagers[i].boxCollider.isTrigger = true;
            villagers[i].InitOrderLayerAllChild(0);
            villagers[i].OnClick += RemoveVillagerList;
            villagers[i].OnDie += RemoveVillagerList;
            villagers[i].model.CanGetChild = false;
            villagers[i].model.IsFight = true;
            villagersIndex++;
        }
        model.ChildCard = null;
        model.BottomCard = this;

        int maxCount = monstersIndex > villagersIndex ? monstersIndex : villagersIndex;
        transform.localScale = new Vector3(maxCount * 2 + 1, 6, 1);
    }

    void RemoveVillagerList(Card remover)
    {
        remover.OnDie -= RemoveVillagerList;
        remover.OnClick -= RemoveVillagerList;
        remover.model.CanGetChild = true;
        remover.model.IsFight = false;
        if (remover.boxCollider != null)
        {
            remover.boxCollider.isTrigger = false;
        }
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
        int maxCount = monstersIndex > villagersIndex ? monstersIndex : villagersIndex;
        transform.localScale = new Vector3(maxCount * 2 + 1, 6, 1);
    }

    void EndBattle()
    {
        if (hitUI.canvas != null)
        {
            Pool.HitUI.ReturnPool(hitUI);
        }

        for (int i = 0; i < monsters.Count; i++)
        {
            RemoveVillagerList(monsters[i]);
        }
        for (int i = 0; i < villagers.Count; i++)
        {
            RemoveVillagerList(villagers[i]);
        }
        monsters.Clear();
        villagers.Clear();
        notBattles.Clear();
        Pool.BattleField.ReturnPool(this);
    }
}
