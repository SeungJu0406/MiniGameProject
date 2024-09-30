using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFieldPool : MonoBehaviour
{
    public static BattleFieldPool Instance;

    [SerializeField] BattleField battleField;
    [SerializeField] Queue<BattleField> pool;
    [SerializeField] int size;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        pool = new Queue<BattleField>(size);

        for(int i  = 0; i < size; i++)
        {
            BattleField instance = Instantiate(battleField);
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(transform);
            pool.Enqueue(instance);
        }
    }

    public BattleField GetPool(Vector3 pos)
    {
        if (pool.Count > 0) 
        {
            BattleField instance = pool.Dequeue();
            instance.transform.position = pos;
            instance.gameObject.SetActive(true);
            return instance;
        }
        else
        {
            BattleField instance = Instantiate(battleField);
            instance.transform.SetParent(transform);
            return instance;
        }
    }
    
    public void ReturnPool(BattleField instance)
    {
        instance.gameObject.SetActive(false);
        pool.Enqueue(instance);
    }
}
