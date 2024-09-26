public class DisposableCombine : CardCombine
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void CompleteCreate()
    {

        if (model.ChildCard != null)
        {
            model.ChildCard.model.ParentCard = model.ParentCard; // �ڽ��� �θ� ������ �θ�� ��ü    
        }
        if (model.ParentCard != null)
        {
            model.ParentCard.model.ChildCard = model.ChildCard; //�θ��� �ڽ��� ������ �ڽ����� ��ü
            // ������ �����ϰ��
            if (model.BottomCard.combine == this)
            {
                model.ParentCard.model.BottomCard = model.ParentCard; // �ºθ��� ������ �ºθ� �ڽ����� ��ü
            }
            // ������ ������ �ƴҰ��
            else
            {
                model.ParentCard.model.BottomCard = model.BottomCard; // �º����� ������ ������ �������� ��ü
            }
        }
        RemoveCombineList();    //������ ���ո���Ʈ���� ����
                                // ������ ž�� ���, ���� �ڽ�ī�尡 �ִ� ���
        if (model.TopCard.combine == this && model.ChildCard != null)
        {
            // ������ ���� ����Ʈ�� ���ڽ��� ���ո���Ʈ�� ����
            model.ChildCard.model.ingredients.Clear();
            for (int i = 0; i < model.ingredients.Count; i++)
            {
                model.ChildCard.model.ingredients.Add(model.ingredients[i]);
            }

            model.ChildCard.model.TopCard = model.ChildCard;    //���ڽ��� ž�� ���ڽ� �������� ����
            model.ChildCard.ChangeTopChild(model.ChildCard);    //���ڽ��� �ڽĵ��� ž�� ����
        }
        Destroy(gameObject); //���� ������Ʈ ����
    }
}
