using System.Collections;
using System.Collections.Generic;
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
        if ( value.Equals( "idle" ) ) {
            HandAnimator.speed = 0;
        } else {
            HandAnimator.speed = Speed;
        }
    }

}
