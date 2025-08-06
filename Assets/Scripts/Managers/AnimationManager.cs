
using UnityEngine;


public class AnimationManager : Singleton<AnimationManager>
{

    [SerializeField] RuntimeAnimatorController _PlayerAnimator;

    public RuntimeAnimatorController GetPlayerAnimator() { return _PlayerAnimator; }


}
