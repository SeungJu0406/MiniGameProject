public static class Manager
{
    public static GameManager Game { get { return GameManager.Instance; } }
    public static CardManager Card { get { return CardManager.Instance; } }
    public static UIManager UI { get { return UIManager.Instance; } }
    public static DayManager Day { get { return DayManager.Instance; } }
    public static CameraManager Camera { get { return CameraManager.Instance; }}
    public static SoundManager Sound { get { return SoundManager.Instance; } }
    public static InputManager Input { get { return InputManager.Instance; } }
}
