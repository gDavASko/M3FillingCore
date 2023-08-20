public interface IDestroyAnimator
{
    System.Action OnDestroyComplete { get; set; }
    void AnimatedDestroy();
}