using UnityEngine;

public class HandAnimatorEventHandler : MonoBehaviour {

    private Animator animator;
    [Range( 0f, 1f )] public float Speed;
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;

    private Animator HandAnimator {
        get { return animator == null ? animator = GetComponent<Animator>() : animator; }
    }

    private void Awake() {
        animator = GetComponent<Animator>();
        animator.speed = Speed;
    }

    public void ActionFinished() {
    }

    public void ActiveState() {
    }

    public void SetTrigger( string value ) {
        HandAnimator.SetTrigger( value );
        HandAnimator.speed = value.Equals( "idle" ) ? 0 : Speed;
    }

}
