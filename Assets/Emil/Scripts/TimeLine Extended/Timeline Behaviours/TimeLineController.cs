using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;

namespace LandRocker_Behavior
{
    public enum TimeLineWrapType
    {
        Reverse, Wrap
    }

    public class TimeLineController : MonoBehaviour
    {
        [Header("Make Sure playable Director is Played or PlayOnAwake is set")]
        [Space]
        [Space]

        public PlayableDirector playableDirector;

        public TimeLineWrapType timelineBackMode = TimeLineWrapType.Reverse;
        public float lerpSpeed = 5f;
        public float reverseSpeed = 5f;
        
        [Header("Moving Forward Amount , Change this to move slower or quicker")]
        public float timeDeviation = 0.5f;
        
        public uint degreesToScrollWheelToReverse = 90;
        public uint degreesToScrollWheelToStopReverse = 90;
        public uint amountToSchrollWheelToReverse = 10;
        public uint amountToSchrollWheelToStopReverse = 10;

        [Tooltip("When Reversed You Can Apply Reverse Speed")]
        public bool applyReverseSpeed = true;

        protected uint currentScrollWheelDegreeToReverse = 0;
        protected uint currentScrollWheelDegreeToStopReverse = 0;
        protected bool hasReachedTheEnd = false;
        protected bool isReversing = false;

        private float timeChangeValue = 0;
        public float timeLerpedValue = 0;
        private float tempSpeed = 0;

        private Vector2 previousTouch;
        private Vector2 currentTouch;
        private float moveTouch;

        void Update()
        {
            TranverseTimeLine();
        }

        public virtual void TranverseTimeLine(float _startTime = 0, bool _isSetting = false)
        {
            if (_isSetting)
            {
                playableDirector.time = _startTime;
                timeChangeValue = _startTime;
                timeLerpedValue = _startTime;
                return;
            }

#if UNITY_ANDROID && UNITY_IOS
            Touch touch = new Touch();
            Vector2 touchDir = Vector2.zero;

            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                    previousTouch = touch.position;
                else if (touch.phase == TouchPhase.Moved)
                    currentTouch = touch.position;
                else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                    currentTouch = previousTouch = Vector2.zero;
            }

            touchDir = (currentTouch - previousTouch).normalized;

            if ((touchDir.x > 0 && touchDir.y > 0) || (touchDir.x < 0 && touchDir.y > 0))
                moveTouch = 1;//up
            else if ((touchDir.x > 0 && touchDir.y < 0) || (touchDir.x < 0 && touchDir.y < 0))
                moveTouch = -1; //down
            else
                moveTouch = 0; //0

#endif
            //Stopping The Reverse Action
            if (isReversing && timeLerpedValue < 5)
            {
                if (applyReverseSpeed)
                    lerpSpeed = tempSpeed;

                isReversing = false;
            }
            else if (isReversing)
            {
                if (Input.mouseScrollDelta.y < 0 || moveTouch > 0)
                    currentScrollWheelDegreeToStopReverse += amountToSchrollWheelToStopReverse;

                if (degreesToScrollWheelToStopReverse <= currentScrollWheelDegreeToStopReverse)
                {
                    if (applyReverseSpeed)
                        lerpSpeed = tempSpeed;

                    currentScrollWheelDegreeToStopReverse = 0;
                    isReversing = false;
                }
            }

            if (!isReversing && (Mathf.Abs(Input.mouseScrollDelta.y) > 0 || Mathf.Abs(moveTouch) > 0))
            {
                if (Input.mouseScrollDelta.y < 0 || moveTouch > 0)
                {
                    timeChangeValue += timeDeviation;

                    if (timeLerpedValue > playableDirector.duration)
                    {
                        hasReachedTheEnd = true;
                        timeChangeValue = (float)playableDirector.duration;

                        if (Input.mouseScrollDelta.y < 0 && timelineBackMode != TimeLineWrapType.Wrap)
                            currentScrollWheelDegreeToReverse += amountToSchrollWheelToReverse;

                        if (degreesToScrollWheelToReverse <= currentScrollWheelDegreeToReverse || timelineBackMode == TimeLineWrapType.Wrap)
                        {
                            switch (timelineBackMode)
                            {
                                case TimeLineWrapType.Reverse:
                                    timeChangeValue = 0;
                                    currentScrollWheelDegreeToReverse = 0;
                                    hasReachedTheEnd = false;

                                    if (applyReverseSpeed)
                                    {
                                        tempSpeed = lerpSpeed;
                                        lerpSpeed = reverseSpeed;
                                    }

                                    isReversing = true;
                                    break;

                                case TimeLineWrapType.Wrap:
                                    hasReachedTheEnd = false;
                                    TranverseTimeLine(0, true);
                                    return;
                            }
                        }
                    }
                }
                else
                {
                    if (hasReachedTheEnd)
                    {
                        currentScrollWheelDegreeToReverse = 0;
                        hasReachedTheEnd = false;
                        return;
                    }

                    timeChangeValue -= timeDeviation;

                    if (timeChangeValue < 0)
                    {
                        timeChangeValue = 0;
                        playableDirector.time = 0;
                    }
                }
            }

            timeLerpedValue = Mathf.Lerp(timeLerpedValue, timeChangeValue, Time.deltaTime * lerpSpeed);
            playableDirector.time = timeLerpedValue;
        }

        //Setters Used For Event Handling
        public void SetSpeedChange(float value)
        {
            lerpSpeed = value;
        }
        public void SetReverseSpeedChange(float value)
        {
            reverseSpeed = value;
        }
        public void SetTimeDev(float value)
        {
            timeDeviation = value;
        }
        public void SetSetSWTR(float value)
        {
            degreesToScrollWheelToReverse = (uint)value;
        }
        public void SetSWTSR(float value)
        {
            degreesToScrollWheelToStopReverse = (uint)value;
        }
        public void SetASWTR(float value)
        {
            amountToSchrollWheelToReverse = (uint)value;
        }
        public void SetASWTSR(float value)
        {
            amountToSchrollWheelToStopReverse = (uint)value;
        }

        public void SetApplyReverseSpeed(bool value)
        {
            applyReverseSpeed = value;
        }

        public void SetReverseType(Int32 value)
        {
            switch (value)
            {
                case 0: timelineBackMode = TimeLineWrapType.Wrap; break;

                case 1:
                    timelineBackMode = TimeLineWrapType.Reverse; break;

            }
        }
    }
}