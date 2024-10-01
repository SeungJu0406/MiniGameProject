public static class Manager
{
    public static GameManager Game { get { return GameManager.Instance; } }
    public static CardManager Card { get { return CardManager.Instance; } }
    public static UIManager UI { get { return UIManager.Instance; } }
    public static DayManager Day { get { return DayManager.Instance; } }
}
