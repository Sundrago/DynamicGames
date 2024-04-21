// Copyright (c) Meta Platforms, Inc. and affiliates. 

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations
{
    /// <summary>
    ///     A component to handle switches
    /// </summary>
    public class MMSwitch : MMTouchButton
    {
        /// the possible states of the switch
        public enum SwitchStates
        {
            Off,
            On
        }

        [Header("Switch")]
        /// a SpriteReplace to represent the switch knob
        public Image SwitchKnob;

        [Header("Knob")]
        /// the state the switch should start in
        public SwitchStates InitialState = SwitchStates.Off;

        public Transform OffPosition;
        public Transform OnPosition;
        public AnimationCurve KnobMovementCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public float KnobMovementDuration = 0.2f;

        [Header("Binding")]
        /// the methods to call when the switch is turned on
        public UnityEvent SwitchOn;

        /// the methods to call when the switch is turned off
        public UnityEvent SwitchOff;

        protected float _knobMovementStartedAt = -50f;

        /// the current state of the switch
        public SwitchStates CurrentSwitchState { get; set; }

        protected override void Update()
        {
            base.Update();
            if (Time.time - _knobMovementStartedAt < KnobMovementDuration)
            {
                var time = Remap(Time.time - _knobMovementStartedAt, 0f, KnobMovementDuration, 0f, 1f);
                var value = KnobMovementCurve.Evaluate(time);

                if (CurrentSwitchState == SwitchStates.Off)
                    SwitchKnob.transform.position = Vector3.Lerp(OnPosition.transform.position,
                        OffPosition.transform.position, value);
                else
                    SwitchKnob.transform.position = Vector3.Lerp(OffPosition.transform.position,
                        OnPosition.transform.position, value);
            }
        }

        /// <summary>
        ///     On init, we set our current switch state
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            CurrentSwitchState = InitialState;
            InitializeState();
        }

        public virtual void InitializeState()
        {
            if (CurrentSwitchState == SwitchStates.Off)
            {
                if (_animator != null) _animator.Play("RollLeft");
                SwitchKnob.transform.position = OffPosition.transform.position;
            }
            else
            {
                if (_animator != null) _animator.Play("RollRight");
                SwitchKnob.transform.position = OnPosition.transform.position;
            }
        }

        /// <summary>
        ///     Use this method to go from one state to the other
        /// </summary>
        public virtual void SwitchState()
        {
            _knobMovementStartedAt = Time.time;
            if (CurrentSwitchState == SwitchStates.Off)
            {
                CurrentSwitchState = SwitchStates.On;
                if (_animator != null) _animator?.SetTrigger("Right");
                if (SwitchOn != null) SwitchOn.Invoke();
            }
            else
            {
                CurrentSwitchState = SwitchStates.Off;
                if (_animator != null) _animator?.SetTrigger("Left");
                if (SwitchOff != null) SwitchOff.Invoke();
            }
        }
    }
}