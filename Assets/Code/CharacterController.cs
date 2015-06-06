using RSG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class CharacterController : MonoBehaviour
    {
        enum CharacterMovementState
        {
            NONE,
            FORWARD,
            LEFT,
            RIGHT,
            BACK
        }

        private CharacterMovementState currentMovementState = CharacterMovementState.NONE;

        private Animation animation;

        private Rigidbody rigidbody;

        private float moveSpeed = 3f;

        bool doCharge = false;

        private PromiseTimer promiseTimer = new PromiseTimer();

        void Awake()
        {
            animation = GetComponent<Animation>();
            rigidbody = GetComponent<Rigidbody>();

            animation["walk"].wrapMode = WrapMode.Loop;
            animation["idle"].wrapMode = WrapMode.Loop;
            animation["run"].wrapMode = WrapMode.Loop;
        }

        void Update()
        {
            promiseTimer.Update(Time.deltaTime);

            if(doCharge)
            {
                rigidbody.MovePosition(transform.position + (transform.forward * moveSpeed * 2f * Time.deltaTime));
            }
            else if(Input.GetKey(KeyCode.W))
            {
                if(currentMovementState != CharacterMovementState.FORWARD)
                {
                    currentMovementState = CharacterMovementState.FORWARD;
                    animation["walk"].speed = 1;
                    animation.Play("walk");
                }
                rigidbody.MovePosition(transform.position + (transform.forward * moveSpeed * Time.deltaTime));
            }
            
            else if (Input.GetKey(KeyCode.S))
            {
                if (currentMovementState != CharacterMovementState.BACK)
                {
                    currentMovementState = CharacterMovementState.BACK;
                    animation["walk"].speed = -1;
                    animation.Play("walk");
                }
                rigidbody.MovePosition(transform.position + (transform.forward * -1 * moveSpeed * Time.deltaTime));
            }
            else if(Input.GetKey(KeyCode.A))
            {
                if(currentMovementState != CharacterMovementState.LEFT)
                {
                    currentMovementState = CharacterMovementState.LEFT;
                    animation["walk"].speed = 1;
                    animation.Play("walk");
                }
                rigidbody.MovePosition(transform.position + (transform.right * -1 * moveSpeed * Time.deltaTime));
            }
            else if (Input.GetKey(KeyCode.D))
            {
                if (currentMovementState != CharacterMovementState.RIGHT)
                {
                    currentMovementState = CharacterMovementState.RIGHT;
                    animation["walk"].speed = 1;
                    animation.Play("walk");
                }
                rigidbody.MovePosition(transform.position + (transform.right * moveSpeed * Time.deltaTime));
            }
            else if(Input.GetKey(KeyCode.Q))
            {
                transform.localEulerAngles = new Vector3(0, 270, 0);
                animation.Play("salute");
                promiseTimer.WaitWhile(_ => animation.IsPlaying("salute"))
                .Then(() =>transform.localEulerAngles = new Vector3(0, 90, 0))
                .Then(() =>
                {
                    animation.PlayQueued("run");
                    doCharge = true;
                })
                .Done();
            }
            else if (Input.GetKeyUp(KeyCode.W) ||
                    Input.GetKeyUp(KeyCode.S) ||
                    Input.GetKeyUp(KeyCode.A) ||
                    Input.GetKeyUp(KeyCode.D))
            {
                if (currentMovementState != CharacterMovementState.NONE)
                {
                    currentMovementState = CharacterMovementState.NONE;
                    animation.Play("idle");
                }
            }
        }
    }
}
