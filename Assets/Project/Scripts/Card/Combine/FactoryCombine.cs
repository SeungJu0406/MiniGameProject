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
        // 최초 내구도가 0이 아닌 경우에만 파괴 작업, 0은 내구도 무한
        if (model.data.durability != 0)
        {
            model.Durability--;
            // 내구도 0이하에서 오브젝트 파괴 작업
            if (model.Durability <= 0)
            {
                // 조합 리스트 정리
                // 본인 자식 리스트를 초기화 후에 자식 리스트에 하위 스택들을 넣는다
                if (model.ChildCard != null)
                {
                    model.ChildCard.model.TopCard = model.ChildCard;    //맞자식의 탑을 맞자식 본인으로 변경

                    model.ChildCard.ChangeTopChild(model.ChildCard);    //맞자식의 자식들의 탑을 변경

                    model.ChildCard.model.ingredients.Clear(); // 리스트 초기화

                    AddCombineListAllChild(model.ChildCard); // 자식의 리스트에 모든 자식 조합 리스트 설정

                    model.ChildCard.model.ParentCard = model.ParentCard; // 자식의 부모를 본인의 부모로 교체    
                }


                // 파괴 후 함수 강제 종료
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
