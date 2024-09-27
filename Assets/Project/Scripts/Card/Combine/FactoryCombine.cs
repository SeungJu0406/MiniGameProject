using System.Collections;
using UnityEngine;

public class FactoryCombine : CardCombine
{
    protected override void Awake()
    {
        base.Awake();
        model.OnChangeBottom += AddFactoryList;
        model.OnChangeCanFactoryCombine += CombineControll;
    }
    protected override void Start()
    {
        base.Start();
    }
    protected void AddFactoryList()
    {
        model.ingredients.Clear();
        AddFactoryCombineListAllChild(model.Card);
    }

    protected void CombineControll()
    {
        if (!model.CanFactoryCombine)
        {
            if (createRoutine != null)
            {
                StopCoroutine(createRoutine);
                createRoutine = null;
                timerBar.gameObject.SetActive(false);
            }
        }
    }


    public override void CompleteCreate()
    {
        // ���� �������� 0�� �ƴ� ��쿡�� �ı� �۾�, 0�� ������ ����
        if (model.data.durability != 0)
        {
            model.Durability--;
            // ������ 0���Ͽ��� ������Ʈ �ı� �۾�
            if (model.Durability <= 0)
            {
                // ���� ����Ʈ ����
                // ���� �ڽ� ����Ʈ�� �ʱ�ȭ �Ŀ� �ڽ� ����Ʈ�� ���� ���õ��� �ִ´�
                if (model.ChildCard != null)
                {
                    model.ChildCard.model.TopCard = model.ChildCard;    //���ڽ��� ž�� ���ڽ� �������� ����

                    model.ChildCard.ChangeTopChild(model.ChildCard);    //���ڽ��� �ڽĵ��� ž�� ����

                    model.ChildCard.model.ingredients.Clear(); // ����Ʈ �ʱ�ȭ

                    AddCombineListAllChild(model.ChildCard); // �ڽ��� ����Ʈ�� ��� �ڽ� ���� ����Ʈ ����

                    model.ChildCard.model.ParentCard = model.ParentCard; // �ڽ��� �θ� ������ �θ�� ��ü    
                }


                // �ı� �� �Լ� ���� ����
                Destroy(gameObject);
                return;
            }
        }
        StartCoroutine(CompleteRoutine());
    }
    WaitForSeconds restartDelay = new WaitForSeconds(0.1f);
    IEnumerator CompleteRoutine()
    {
        yield return restartDelay;
        model.IsFactory = false;
        model.CanFactoryCombine = false;
        AddFactoryList();
    }
}
