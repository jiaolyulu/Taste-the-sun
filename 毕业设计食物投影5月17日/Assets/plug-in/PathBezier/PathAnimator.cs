using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PathBezier))]
public class PathAnimator : MonoBehaviour
{
    public enum Modes
    {
        Once,       //一次.
        Loop,       //循环.
        Reverse,    //逆向.
        ReverseLoop,//逆向循环.
        PingPong,   //来回.
    }

    #region Attribute.
    public bool isCamera = false;
    public bool playOnStart = false;
    public PathAnimator nextAnimation = null;
    public Transform animationTarget = null;
    public Modes mode = Modes.Once;
    public AudioSource source;
    /// <summary>
    /// 是否逆向.
    /// </summary>
    public bool isReversed
    {
        get
        {
            return (mode == Modes.Reverse || mode == Modes.ReverseLoop || pingPongDirection < 0);
        }
    }
    public float pathTime = 10;
    public float pathSpeed
    {
        get
        {
            return bezier.storedTotalArcLength / pathTime;
        }
        set
        {
            float newPathSpeed = value;
            pathTime = bezier.storedTotalArcLength / Mathf.Max(newPathSpeed, 0.000001f);
        }
    }
    PathBezier _bezier;
    public PathBezier bezier
    {
        get
        {
            if (!_bezier)
            {
                _bezier = GetComponent<PathBezier>();
            }
            return _bezier;
        }
    }
    bool playing = false;
    public bool isPlaying
    {
        get
        {
            return playing;
        }
    }
    float usePercentage;
    public float percentage
    {
        get
        {
            return usePercentage;
        }
    }
    float pingPongDirection = 1;
    public bool pingPongGoingForward
    {
        get
        {
            return pingPongDirection == 1;
        }
    }
    #endregion

    #region Event.
    public delegate void AnimationDelegate();
    public delegate void AnimationPointReachedWithNumberEventHandler(int pointNumber);
    public event AnimationDelegate AnimationStarted = null;
    public event AnimationDelegate AnimationPaused = null;
    public event AnimationDelegate AnimationStopped = null;
    public event AnimationDelegate AnimationLooped = null;
    public const AnimationDelegate AnimationPingPong = null;
    public event AnimationDelegate AnimationFinished = null;
    public event AnimationPointReachedWithNumberEventHandler AnimationPointReached = null;
    #endregion

    #region Virtual.
    protected virtual void Awake()
    {
        if (isCamera && !animationTarget.GetComponent<Camera>())
            isCamera = false;
        if (isReversed)
        {
            usePercentage = 1;

        }
        else
        {
            usePercentage = 0;
        }
    }
    protected virtual void Start()
    {
        if (playOnStart)
            Play();
    }
    protected virtual void Update()
    {
        if (!isCamera)
        {
            if (playing)
            {
                UpdateAnimationTime();
                UpdateAnimation();
            }
            else
            {
                if (nextAnimation != null && usePercentage >= 1)
                {
                    nextAnimation.Play();
                    nextAnimation = null;
                }
            }
        }
    }
    protected virtual void LateUpdate()
    {
        if (isCamera)
        {
            if (playing)
            {
                UpdateAnimationTime();
                UpdateAnimation();
            }
            else
            {
                if (nextAnimation != null && usePercentage >= 1)
                {
                    nextAnimation.Play();
                    nextAnimation = null;
                }
            }
        }
    }
    #endregion

    #region Public.
    public void Play()
    {
        playing = true;
        if (isReversed)
        {
            if (usePercentage == 0 && AnimationStarted != null)
                AnimationStarted();
        }
        else
        {
            if (usePercentage == 1 && AnimationStarted != null)
                AnimationStarted();
        }
    }
    public void Pause()
    {
        playing = false;
        if (AnimationPaused != null)
            AnimationPaused();
    }
    /// <summary>
    /// 设置Animation time;
    /// </summary>
    /// <param name="value"></param>
    public void Seek(float value)
    {
        usePercentage = Mathf.Clamp01(value);
        UpdateAnimationTime(false);
        bool p = playing;
        playing = true;
        UpdateAnimation();
        playing = p;
    }
    public void Stop()
    {
        playing = false;
        if (AnimationStopped != null)
            AnimationStopped();
    }
    /// <summary>
    /// 反向.
    /// </summary>
    public void Reverse()
    {
        switch (mode)
        {
            case Modes.Once:
                {
                    mode = Modes.Reverse;
                }
                break;
            case Modes.Reverse:
                {
                    mode = Modes.Once;
                }
                break;
            case Modes.PingPong:
                {
                    pingPongDirection = pingPongDirection == -1 ? 1 : -1;
                }
                break;
            case Modes.Loop:
                {
                    mode = Modes.ReverseLoop;
                }
                break;
            case Modes.ReverseLoop:
                {
                    mode = Modes.Loop;
                }
                break;
        }
    }
    #endregion

    #region Private.
    void UpdateAnimation()
    {
        if(animationTarget == null)
        {
            Stop();
            return;
        }
        if (!playing)
            return;
        animationTarget.position = bezier.GetPathPosition(usePercentage);
        if (isCamera)
            animationTarget.GetComponent<Camera>().fieldOfView = bezier.GetPathFieldOfView(usePercentage);
        animationTarget.rotation = bezier.GetPathRotation(usePercentage);
    }
    void UpdateAnimationTime()
    {
        UpdateAnimationTime(true);
    }
    void UpdateAnimationTime(bool advance)
    {
        if(advance)
        {
            switch(mode)
            {
                case Modes.Once:
                    {
                        if (usePercentage >= 1)
                        {
                            playing = false;
                            if (AnimationFinished != null)
                            {
                                AnimationFinished();
                            }
                            if(source != null)
                            {
                                source.Play();
                            }
                        }
                        else
                        {
                            usePercentage += Time.deltaTime * (1.0f / pathTime);
                        }
                    }
                    break;
                case Modes.Loop:
                    {
                        if (usePercentage >= 1)
                        {
                            usePercentage = 0;
                            if (AnimationLooped != null) AnimationLooped();
                        }
                        usePercentage += Time.deltaTime * (1.0f / pathTime);
                    }
                    break;
                case Modes.ReverseLoop:
                    {
                        if (usePercentage <= 0)
                        {
                            usePercentage = 1;
                            if (AnimationLooped != null) AnimationLooped();
                        }
                        usePercentage += -Time.deltaTime * (1.0f / pathTime);
                    }
                    break;
                case Modes.Reverse:
                    {
                        if (usePercentage <= 0)
                        {
                            playing = false;
                            if (AnimationFinished != null) AnimationFinished();
                        }
                        else
                        {
                            usePercentage += -Time.deltaTime * (1.0f / pathTime);
                        }
                    }
                    break;
                case Modes.PingPong:
                    {
                        usePercentage += Time.deltaTime * (1.0f / pathTime) * pingPongDirection;
                        if (usePercentage >= 1)
                        {
                            usePercentage = 0.99f;
                            pingPongDirection = -1;
                            if (AnimationPingPong != null) AnimationPingPong();
                        }
                        if (usePercentage <= 0)
                        {
                            usePercentage = 0.01f;
                            pingPongDirection = 1;
                            if (AnimationPingPong != null) AnimationPingPong();
                        }
                    }
                    break;
            }
        }
        usePercentage = Mathf.Clamp01(usePercentage);
    }
    #endregion
}
