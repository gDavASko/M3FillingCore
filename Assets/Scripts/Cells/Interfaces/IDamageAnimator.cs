public interface IDamageAnimator
{
    System.Action OnDamageComplete { get; set; }
    void AnimatedDealDamage(float percentHealth);
}