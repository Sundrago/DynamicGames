using System;
using System.Collections.Generic;
using System.Linq;
using AssetKits.ParticleImage.Enumerations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using PlayMode = AssetKits.ParticleImage.Enumerations.PlayMode;

namespace AssetKits.ParticleImage
{
    [AddComponentMenu("UI/Particle Image/Particle Image")]
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ParticleImage : MaskableGraphic
    {
        [SerializeField] private ParticleImage _main;

        [SerializeField] private ParticleImage[] _children;

        [SerializeField] private Simulation _space = Simulation.Local;

        [SerializeField] private TimeScale _timeScale = TimeScale.Normal;

        [SerializeField] private Module _emitterConstraintEnabled = new(false);

        [SerializeField] private Transform _emitterConstraintTransform;

        [SerializeField] private EmitterShape _shape = EmitterShape.Circle;

        [SerializeField] private SpreadType _spread = SpreadType.Random;

        [SerializeField] private float _spreadLoop = 1;

        [SerializeField] private float _startDelay;

        [SerializeField] private float _radius = 50;

        [SerializeField] private float _width = 100;

        [SerializeField] private float _height = 100;

        [SerializeField] private float _angle = 45;

        [SerializeField] private float _length = 100f;

        [SerializeField] private bool _fitRect;

        [SerializeField] private bool _emitOnSurface = true;

        [SerializeField] private float _emitterThickness;

        [SerializeField] private bool _loop = true;

        [SerializeField] private float _duration = 5f;

        [SerializeField] private PlayMode _playMode = PlayMode.OnAwake;

        [SerializeField] private SeparatedMinMaxCurve _startSize = new(40f);

        [SerializeField] private ParticleSystem.MinMaxGradient _startColor = new(Color.white);

        [SerializeField] private ParticleSystem.MinMaxCurve _lifetime = new(1f);

        [SerializeField] private ParticleSystem.MinMaxCurve _startSpeed = new(2f);

        [SerializeField] private ParticleSystem.MinMaxGradient _colorOverLifetime = new(new Gradient());

        [SerializeField] private ParticleSystem.MinMaxGradient _colorBySpeed = new(new Gradient());

        [SerializeField] private SpeedRange _colorSpeedRange = new(0f, 1f);

        [SerializeField] private SeparatedMinMaxCurve _sizeOverLifetime =
            new(new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f)));

        [SerializeField]
        private SeparatedMinMaxCurve _sizeBySpeed = new(new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f)));

        [SerializeField] private SpeedRange _sizeSpeedRange = new(0f, 1f);

        [SerializeField] private SeparatedMinMaxCurve _startRotation = new(0f);

        [SerializeField] private SeparatedMinMaxCurve _rotationOverLifetime = new(0f);

        [SerializeField] private SeparatedMinMaxCurve _rotationBySpeed =
            new(new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f)));

        [SerializeField] private SpeedRange _rotationSpeedRange = new(0f, 1f);

        [SerializeField] private ParticleSystem.MinMaxCurve _speedOverLifetime = new(1f);

        [SerializeField] private bool _alignToDirection;

        [SerializeField] private ParticleSystem.MinMaxCurve _gravity = new(-9.81f);

        [SerializeField] private Module _targetModule = new(false);

        [SerializeField] private Transform _attractorTarget;

        [SerializeField] private ParticleSystem.MinMaxCurve _toTarget = new(1f,
            new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f)));

        [SerializeField] private AttractorType _targetMode = AttractorType.Pivot;

        [SerializeField] private Module _noiseModule = new(false);

        [SerializeField] private Module _gravityModule = new(false);

        [SerializeField] private Module _vortexModule = new(false);

        [SerializeField] private Module _velocityModule = new(false);

        [SerializeField] private Simulation _velocitySpace;

        [SerializeField] private SeparatedMinMaxCurve _velocityOverLifetime = new(0f, true, false);

        [SerializeField] private ParticleSystem.MinMaxCurve _vortexStrength;

        [SerializeField] private Module _sheetModule = new(false);

        [SerializeField] private Vector2Int _textureTile = Vector2Int.one;

        [SerializeField] private SheetType _sheetType = SheetType.FPS;

        [SerializeField] private ParticleSystem.MinMaxCurve _frameOverTime;

        [SerializeField] private ParticleSystem.MinMaxCurve _startFrame = new(0f);

        [SerializeField] private SpeedRange _frameSpeedRange = new(0f, 1f);

        [SerializeField] private int _textureSheetFPS = 25;

        [SerializeField] private int _textureSheetCycles = 1;

        [SerializeField] private float _rate = 50;

        [SerializeField] private float _rateOverLifetime;

        [SerializeField] private float _rateOverDistance;

        [SerializeField] private List<Burst> _bursts = new();

        [FormerlySerializedAs("_trailRenderer")] [SerializeField]
        private ParticleTrailRenderer _particleTrailRenderer;

        [SerializeField] private Module _trailModule;

        [SerializeField] private ParticleSystem.MinMaxCurve _trailWidth = new(1f,
            new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f)));

        [SerializeField] private float _trailLifetime = 1f;

        [SerializeField] private float _minimumVertexDistance = 10f;

        [SerializeField] private ParticleSystem.MinMaxGradient _trailColorOverLifetime = new(Color.white);

        [SerializeField] private ParticleSystem.MinMaxGradient _trailColorOverTrail = new(Color.white);


        [SerializeField] private bool _inheritParticleColor;

        [SerializeField] private bool _dieWithParticle;

        [Range(0f, 1f)] [SerializeField] private float _trailRatio = 1f;

        [SerializeField] private int _noiseOctaves = 1;

        [SerializeField] private float _noiseFrequency = 1f;

        [SerializeField] private float _noiseStrength = 1f;

        [SerializeField] private UnityEvent _onStart = new();

        [SerializeField] private UnityEvent _onFirstParticleFinish = new();

        [SerializeField] private UnityEvent _onParticleFinish = new();

        [SerializeField] private UnityEvent _onLastParticleFinish = new();

        [SerializeField] private UnityEvent _onStop = new();

        public bool moduleEmitterFoldout;
        public bool moduleParticleFoldout;
        public bool moduleMovementFoldout;
        public bool moduleEventsFoldout;

        [SerializeField] private Texture m_Texture;

        private float _burstTimer;

        private Camera _camera;

        private Vector3 _deltaPosition;


        private bool _firstParticleFinished;

        private Vector3 _lastPosition;

        private float _loopTimer;

        private int _orderOverDistance;
        private int _orderOverLife;

        private int _orderPerSec;

        private Vector2 _position;

        private float _t;
        private float _t2;

        /// <summary>
        ///     Child emitters of this emitter.
        /// </summary>
        public ParticleImage[] children
        {
            get => _children;
            private set => _children = value;
        }

        /// <summary>
        ///     Root emitter of this system.
        /// </summary>
        public ParticleImage main
        {
            get
            {
                if (_main == null) _main = GetMain();
                return _main;
            }
            private set => _main = value;
        }

        /// <summary>
        ///     Returns true if this emitter is the root emitter of this system.
        /// </summary>
        public bool isMain => main == this;

        public RectTransform canvasRect { get; set; }

        public Simulation space
        {
            get => _space;
            set => _space = value;
        }

        public TimeScale timeScale
        {
            get => _timeScale;
            set => _timeScale = value;
        }

        public bool emitterConstraintEnabled
        {
            get => _emitterConstraintEnabled.enabled;
            set => _emitterConstraintEnabled.enabled = value;
        }

        public Transform emitterConstraintTransform
        {
            get => _emitterConstraintTransform;
            set => _emitterConstraintTransform = value;
        }

        /// <summary>
        ///     <para>The type of shape to emit particles from.</para>
        /// </summary>
        public EmitterShape shape
        {
            get => _shape;
            set => _shape = value;
        }

        /// <summary>
        ///     <para>The type of spread to use when emitting particles.</para>
        /// </summary>
        public SpreadType spreadType
        {
            get => _spread;
            set => _spread = value;
        }

        /// <summary>
        ///     Loop count for spread.
        /// </summary>
        public float spreadLoop
        {
            get => _spreadLoop;
            set => _spreadLoop = value;
        }

        /// <summary>
        ///     <para>Start delay in seconds.</para>
        /// </summary>
        public float startDelay
        {
            get => _startDelay;
            set => _startDelay = value;
        }

        /// <summary>
        ///     <para>Radius of the circle shape to emit particles from.</para>
        /// </summary>
        public float circleRadius
        {
            get => _radius;
            set => _radius = value;
        }

        /// <summary>
        ///     <para>Width of the rectangle shape to emit particles from.</para>
        /// </summary>
        public float rectWidth
        {
            get => _width;
            set => _width = value;
        }

        /// <summary>
        ///     <para>Height of the rectangle shape to emit particles from.</para>
        /// </summary>
        public float rectHeight
        {
            get => _height;
            set => _height = value;
        }

        /// <summary>
        ///     <para>Angle of the directional shape to emit particles from.</para>
        /// </summary>
        public float directionAngle
        {
            get => _angle;
            set => _angle = value;
        }

        /// <summary>
        ///     <para>Length of the line shape to emit particles from.</para>
        /// </summary>
        public float lineLength
        {
            get => _length;
            set => _length = value;
        }

        public bool fitRect
        {
            get => _fitRect;
            set
            {
                _fitRect = value;
                if (value)
                    FitRect();
            }
        }

        /// <summary>
        ///     <para>Emit on the whole surface of the current shape.</para>
        /// </summary>
        public bool emitOnSurface
        {
            get => _emitOnSurface;
            set => _emitOnSurface = value;
        }

        /// <summary>
        ///     <para>Thickness of the shape's edge from which to emit particles if emitOnSurface is disabled.</para>
        /// </summary>
        public float emitterThickness
        {
            get => _emitterThickness;
            set => _emitterThickness = value;
        }

        /// <summary>
        ///     <para>Determines whether the Particle Image is looping.</para>
        /// </summary>
        public bool loop
        {
            get => _loop;
            set => _loop = value;
        }

        /// <summary>
        ///     <para>The duration of the Particle Image in seconds</para>
        /// </summary>
        public float duration
        {
            get => _duration;
            set => _duration = value;
        }

        public PlayMode PlayMode
        {
            get => _playMode;
            set
            {
                _playMode = value;
                if (isMain && children != null)
                    foreach (var particleImage in children)
                        particleImage._playMode = value;
                else if (!isMain) main.PlayMode = value;
            }
        }

        public SeparatedMinMaxCurve startSize
        {
            get => _startSize;
            set => _startSize = value;
        }

        public ParticleSystem.MinMaxGradient startColor
        {
            get => _startColor;
            set => _startColor = value;
        }

        public ParticleSystem.MinMaxCurve lifetime
        {
            get => _lifetime;
            set => _lifetime = value;
        }

        public ParticleSystem.MinMaxCurve startSpeed
        {
            get => _startSpeed;
            set => _startSpeed = value;
        }

        public ParticleSystem.MinMaxGradient colorOverLifetime
        {
            get => _colorOverLifetime;
            set => _colorOverLifetime = value;
        }

        public ParticleSystem.MinMaxGradient colorBySpeed
        {
            get => _colorBySpeed;
            set => _colorBySpeed = value;
        }

        public SpeedRange colorSpeedRange
        {
            get => _colorSpeedRange;
            set => _colorSpeedRange = value;
        }

        public SeparatedMinMaxCurve sizeOverLifetime
        {
            get => _sizeOverLifetime;
            set => _sizeOverLifetime = value;
        }

        public SeparatedMinMaxCurve sizeBySpeed
        {
            get => _sizeBySpeed;
            set => _sizeBySpeed = value;
        }

        public SpeedRange sizeSpeedRange
        {
            get => _sizeSpeedRange;
            set => _sizeSpeedRange = value;
        }

        public SeparatedMinMaxCurve startRotation
        {
            get => _startRotation;
            set => _startRotation = value;
        }

        public SeparatedMinMaxCurve rotationOverLifetime
        {
            get => _rotationOverLifetime;
            set => _rotationOverLifetime = value;
        }

        public SeparatedMinMaxCurve rotationBySpeed
        {
            get => _rotationBySpeed;
            set => _rotationBySpeed = value;
        }

        public SpeedRange rotationSpeedRange
        {
            get => _rotationSpeedRange;
            set => _rotationSpeedRange = value;
        }

        public ParticleSystem.MinMaxCurve speedOverLifetime
        {
            get => _speedOverLifetime;
            set => _speedOverLifetime = value;
        }

        /// <summary>
        ///     <para>Align particles based on their direction of travel.</para>
        /// </summary>
        public bool alignToDirection
        {
            get => _alignToDirection;
            set => _alignToDirection = value;
        }

        public ParticleSystem.MinMaxCurve gravity
        {
            get => _gravity;
            set => _gravity = value;
        }

        public bool attractorEnabled
        {
            get => _targetModule.enabled;
            set => _targetModule.enabled = value;
        }

        public Transform attractorTarget
        {
            get => _attractorTarget;
            set => _attractorTarget = value;
        }

        public ParticleSystem.MinMaxCurve attractorLerp
        {
            get => _toTarget;
            set => _toTarget = value;
        }

        public AttractorType attractorType
        {
            get => _targetMode;
            set => _targetMode = value;
        }

        public bool noiseEnabled
        {
            get => _noiseModule.enabled;
            set => _noiseModule.enabled = value;
        }

        public bool gravityEnabled
        {
            get => _gravityModule.enabled;
            set => _gravityModule.enabled = value;
        }

        public bool vortexEnabled
        {
            get => _vortexModule.enabled;
            set => _vortexModule.enabled = value;
        }

        public bool velocityEnabled
        {
            get => _velocityModule.enabled;
            set => _velocityModule.enabled = value;
        }

        public Simulation velocitySpace
        {
            get => _velocitySpace;
            set => _velocitySpace = value;
        }

        public SeparatedMinMaxCurve velocityOverLifetime
        {
            get => _velocityOverLifetime;
            set => _velocityOverLifetime = value;
        }

        public ParticleSystem.MinMaxCurve vortexStrength
        {
            get => _vortexStrength;
            set => _vortexStrength = value;
        }

        public bool textureSheetEnabled
        {
            get => _sheetModule.enabled;
            set => _sheetModule.enabled = value;
        }

        public Vector2Int textureTile
        {
            get => _textureTile;
            set => _textureTile = value;
        }

        public SheetType textureSheetType
        {
            get => _sheetType;
            set => _sheetType = value;
        }

        public ParticleSystem.MinMaxCurve textureSheetFrameOverTime
        {
            get => _frameOverTime;
            set => _frameOverTime = value;
        }

        public ParticleSystem.MinMaxCurve textureSheetStartFrame
        {
            get => _startFrame;
            set => _startFrame = value;
        }

        public SpeedRange textureSheetFrameSpeedRange
        {
            get => _frameSpeedRange;
            set => _frameSpeedRange = value;
        }

        public int textureSheetFPS
        {
            get => _textureSheetFPS;
            set => _textureSheetFPS = value;
        }

        public int textureSheetCycles
        {
            get => _textureSheetCycles;
            set => _textureSheetCycles = value;
        }

        /// <summary>
        ///     List of particles in the system.
        /// </summary>
        public List<Particle> particles { get; } = new();

        /// <summary>
        ///     The rate at which the emitter spawns new particles per second.
        /// </summary>
        public float rateOverTime
        {
            get => _rate;
            set => _rate = value;
        }

        /// <summary>
        ///     The rate at which the emitter spawns new particles over emitter duration.
        /// </summary>
        public float rateOverLifetime
        {
            get => _rateOverLifetime;
            set => _rateOverLifetime = value;
        }

        /// <summary>
        ///     The rate at which the emitter spawns new particles over distance per pixel.
        /// </summary>
        public float rateOverDistance
        {
            get => _rateOverDistance;
            set => _rateOverDistance = value;
        }

        public ParticleTrailRenderer particleTrailRenderer
        {
            get
            {
                if (trailsEnabled)
                {
                    if (!_particleTrailRenderer)
                    {
                        _particleTrailRenderer = GetComponentInChildren<ParticleTrailRenderer>();

                        if (!_particleTrailRenderer)
                        {
                            var tr = new GameObject("Trails");
                            tr.transform.parent = transform;
                            tr.transform.localPosition = Vector3.zero;
                            tr.transform.localScale = Vector3.one;
                            tr.transform.localEulerAngles = Vector3.zero;
                            tr.AddComponent<CanvasRenderer>();
                            var r = tr.AddComponent<ParticleTrailRenderer>();
                            _particleTrailRenderer = r;
                        }
                    }

                    return _particleTrailRenderer;
                }

                return null;
            }
            set => _particleTrailRenderer = value;
        }

        /// <summary>
        ///     The trails enabled.
        /// </summary>
        public bool trailsEnabled
        {
            get => _trailModule.enabled;
            set => _trailModule.enabled = value;
        }

        /// <summary>
        ///     The width of the trail in pixels.
        /// </summary>
        public ParticleSystem.MinMaxCurve trailWidth
        {
            get => _trailWidth;
            set => _trailWidth = value;
        }

        /// <summary>
        ///     Trail lifetime in seconds
        /// </summary>
        public float trailLifetime
        {
            get => _trailLifetime;
            set => _trailLifetime = value;
        }

        /// <summary>
        ///     Vertex distance in canvas pixels
        /// </summary>
        public float minimumVertexDistance
        {
            get => _minimumVertexDistance;
            set => _minimumVertexDistance = value;
        }

        /// <summary>
        ///     The color of the trail over its lifetime.
        /// </summary>
        public ParticleSystem.MinMaxGradient trailColorOverLifetime
        {
            get => _trailColorOverLifetime;
            set => _trailColorOverLifetime = value;
        }

        /// <summary>
        ///     The color of the trail over the lifetime of the trail.
        /// </summary>
        public ParticleSystem.MinMaxGradient trailColorOverTrail
        {
            get => _trailColorOverTrail;
            set => _trailColorOverTrail = value;
        }

        public bool inheritParticleColor
        {
            get => _inheritParticleColor;
            set => _inheritParticleColor = value;
        }

        public bool dieWithParticle
        {
            get => _dieWithParticle;
            set => _dieWithParticle = value;
        }

        public float trailRatio
        {
            get => _trailRatio;
            set => _trailRatio = Mathf.Clamp01(value);
        }

        public float time { get; private set; }

        public Noise noise { get; set; } = new();

        public int noiseOctaves
        {
            get => _noiseOctaves;
            set
            {
                _noiseOctaves = value;
                noise.SetFractalOctaves(_noiseOctaves);
            }
        }

        public float noiseFrequency
        {
            get => _noiseFrequency;
            set
            {
                _noiseFrequency = value;
                noise.SetFrequency(_noiseFrequency);
            }
        }

        public float noiseStrength
        {
            get => _noiseStrength;
            set => _noiseStrength = value;
        }

        /// <summary>
        ///     Determines if the particle system is emitting.
        /// </summary>
        public bool isEmitting { get; private set; }

        /// <summary>
        ///     Determines if the particle system is playing.
        /// </summary>
        public bool isPlaying { get; private set; }

        /// <summary>
        ///     Determines whether the Particle System is stopped.
        /// </summary>
        public bool isStopped { get; private set; }

        /// <summary>
        ///     Determines whether the Particle System is paused.
        /// </summary>
        public bool isPaused { get; private set; }

        /// <summary>
        ///     Called when the particle system starts.
        /// </summary>
        public UnityEvent onStart => _onStart;

        /// <summary>
        ///     Called when the first piece of a particle finishes.
        /// </summary>
        public UnityEvent onFirstParticleFinish => _onFirstParticleFinish;

        /// <summary>
        ///     Called when any piece of a particle finishes.
        /// </summary>
        public UnityEvent onParticleFinish => _onParticleFinish;

        /// <summary>
        ///     Called when the last piece of a particle finishes.
        /// </summary>
        public UnityEvent onLastParticleFinish => _onLastParticleFinish;

        /// <summary>
        ///     Called when the particle system is stopped.
        /// </summary>
        public UnityEvent onStop => _onStop;

        /// <summary>
        ///     Delta position of the particle system.
        /// </summary>
        public Vector3 deltaPosition => _deltaPosition;

        private Camera camera
        {
            get
            {
                if (_camera == null) _camera = Camera.main;

                return _camera;
            }
        }

        public Material material
        {
            get => m_Material;
            set
            {
                if (m_Material == value) return;

                m_Material = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        public Texture texture
        {
            get => m_Texture;
            set
            {
                if (m_Texture == value) return;

                m_Texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        public override Texture mainTexture => m_Texture == null ? s_WhiteTexture : m_Texture;

        private void Awake()
        {
            noise.SetNoiseType(Noise.NoiseType.OpenSimplex2);
            noise.SetFrequency(_noiseFrequency / 100f);
            noise.SetFractalOctaves(_noiseOctaves);

            Clear();

            if (PlayMode == PlayMode.OnAwake && Application.isPlaying) Play();
        }

        private void Update()
        {
            Animate();
        }

        private void LateUpdate()
        {
            for (var i = 0; i < particles.Count; i++)
                if (particles[i].TimeSinceBorn > particles[i].Lifetime && particles[i].trailPoints.Count <= 3)
                {
                    onParticleFinish.Invoke();
                    particles.RemoveAt(i);
                    if (_firstParticleFinished == false)
                    {
                        _firstParticleFinished = true;
                        onFirstParticleFinish.Invoke();
                    }

                    if (particles.Count < 1) onLastParticleFinish.Invoke();
                }
        }

        public void OnEnable()
        {
            if (isMain) children = GetChildren();

            if (fitRect) FitRect();

            main = GetMain();
            main.children = main.GetChildren();

            _lastPosition = transform.position;

            if (canvas) canvasRect = canvas.gameObject.GetComponent<RectTransform>();

            if (PlayMode == PlayMode.OnEnable && Application.isPlaying)
            {
                Stop(true);
                Clear();
                Play();
            }

            RecalculateMasking();
            RecalculateClipping();

            SetAllDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            if (fitRect) FitRect();

            if (!_emitOnSurface)
                switch (_shape)
                {
                    case EmitterShape.Circle:
                        _emitterThickness = Mathf.Clamp(_emitterThickness, 0f, _radius);
                        break;
                    case EmitterShape.Rectangle:
                        _emitterThickness = Mathf.Clamp(_emitterThickness, 0f,
                            rectTransform.sizeDelta.x < rectTransform.sizeDelta.y ? _width : _height);
                        break;
                    case EmitterShape.Line:
                        _emitterThickness = Mathf.Clamp(_emitterThickness, 0f, _radius);
                        break;
                }
        }

        private void OnTransformChildrenChanged()
        {
            main = GetMain();
            if (isMain)
                children = GetChildren();

            if (Application.isEditor)
            {
                Stop(true);
                Clear();
                Play();
            }
        }

        private void OnTransformParentChanged()
        {
            main = GetMain();
            if (isMain)
                children = GetChildren();

            if (Application.isEditor)
            {
                Stop(true);
                Clear();
                Play();
            }
        }

        public ParticleImage GetMain()
        {
            if (transform.parent)
                if (transform.parent.TryGetComponent(out ParticleImage p))
                    return p.GetMain();

            return this;
        }

        /// <summary>
        ///     Get all children of this Particle Image.
        /// </summary>
        /// <returns>
        ///     A list of all children ParticleImage.
        /// </returns>
        public ParticleImage[] GetChildren()
        {
            if (transform.childCount <= 0) return null;

            var ch = GetComponentsInChildren<ParticleImage>().Where(t => t != this);

            if (ch.Any()) return ch.ToArray();

            return null;
        }

        /// <summary>
        ///     Starts the particle system.
        /// </summary>
        public void Play()
        {
            main.DoPlay();
        }

        private void DoPlay()
        {
            if (isMain && children != null)
                foreach (var particleImage in children)
                    particleImage.DoPlay();
            onStart.Invoke();
            time = 0f;
            _burstTimer = 0f;
            for (var i = 0; i < _bursts.Count; i++) _bursts[i].used = false;
            isEmitting = true;
            isPlaying = true;
            isPaused = false;
            isStopped = false;
        }

        /// <summary>
        ///     Pauses the particle system.
        /// </summary>
        public void Pause()
        {
            main.DoPause();
        }

        private void DoPause()
        {
            if (isMain && children != null)
                foreach (var particleImage in children)
                    particleImage.DoPause();

            isEmitting = false;
            isPlaying = false;
            isPaused = true;
        }

        /// <summary>
        ///     Stops playing the Particle System.
        /// </summary>
        public void Stop()
        {
            Stop(false);
        }

        /// <summary>
        ///     Stops playing the Particle System using the supplied stop behaviour.
        /// </summary>
        /// <param name="stopAndClear">
        ///     If true, the particle system will be cleared and all emitted particles will be destroyed.
        /// </param>
        public void Stop(bool stopAndClear)
        {
            main.DoStop(stopAndClear);
        }

        private void DoStop(bool stopAndClear)
        {
            if (isMain && children != null)
                foreach (var particleImage in children)
                    particleImage.DoStop(stopAndClear);

            _orderPerSec = 0;
            _orderOverLife = 0;
            _orderOverDistance = 0;
            for (var i = 0; i < _bursts.Count; i++) _bursts[i].used = false;

            if (stopAndClear)
            {
                isStopped = true;
                isPlaying = false;
                Clear();
            }

            isEmitting = false;
            if (isPaused)
            {
                isPaused = false;
                isStopped = true;
                isPlaying = false;
                Clear();
            }

            for (var i = 0; i < _bursts.Count; i++) _bursts[i].used = false;
            _firstParticleFinished = false;
            SetVerticesDirty();
            SetMaterialDirty();
            if (particleTrailRenderer)
            {
                particleTrailRenderer.SetVerticesDirty();
                particleTrailRenderer.SetMaterialDirty();
            }
        }

        /// <summary>
        ///     Remove all particles from the Particle System.
        /// </summary>
        public void Clear()
        {
            main.DoClear();
        }

        private void DoClear()
        {
            if (isMain && children != null)
                foreach (var particleImage in children)
                    particleImage.DoClear();
            for (var i = 0; i < _bursts.Count; i++) _bursts[i].used = false;
            time = 0;
            _burstTimer = 0;
            particles.Clear();
            SetVerticesDirty();
            SetMaterialDirty();
            if (particleTrailRenderer)
            {
                particleTrailRenderer.SetVerticesDirty();
                particleTrailRenderer.SetMaterialDirty();
            }
        }

        private void Animate()
        {
            if (isPlaying)
            {
                _deltaPosition = transform.position - _lastPosition;

                if (_emitterConstraintTransform && _emitterConstraintEnabled.enabled)
                {
                    if (_emitterConstraintTransform is RectTransform)
                    {
                        transform.position = _emitterConstraintTransform.position;
                    }
                    else
                    {
                        Vector3 canPos;
                        var viewportPos = camera.WorldToViewportPoint(_emitterConstraintTransform.position);

                        canPos = new Vector3(viewportPos.x.Remap(0.5f, 1.5f, 0f, canvasRect.rect.width),
                            viewportPos.y.Remap(0.5f, 1.5f, 0f, canvasRect.rect.height), 0);

                        canPos = canvasRect.transform.TransformPoint(canPos);

                        canPos = transform.parent.InverseTransformPoint(canPos);

                        transform.localPosition = canPos;
                    }
                }

                if (isMain)
                    time += _timeScale == TimeScale.Normal ? Time.deltaTime : Time.unscaledDeltaTime;
                else
                    time = main.time;

                _loopTimer += _timeScale == TimeScale.Normal ? Time.deltaTime : Time.unscaledDeltaTime;

                _burstTimer += _timeScale == TimeScale.Normal ? Time.deltaTime : Time.unscaledDeltaTime;


                SetVerticesDirty();
                SetMaterialDirty();

                if (particleTrailRenderer)
                {
                    particleTrailRenderer.SetVerticesDirty();
                    particleTrailRenderer.SetMaterialDirty();
                }
            }


            if (isEmitting)
            {
                //Emit per second
                if (_rate > 0)
                    if ((time < _duration + _startDelay || _loop) && time > _startDelay)
                    {
                        var dur = 1f / _rate;
                        _t += _timeScale == TimeScale.Normal ? Time.deltaTime : Time.unscaledDeltaTime;
                        while (_t >= dur)
                        {
                            _t -= dur;
                            _orderPerSec++;
                            particles.Insert(0, GenerateParticle(_orderPerSec, 1, null));
                        }
                    }

                //Emit over lifetime
                if (_rateOverLifetime > 0)
                    if ((time < _duration + _startDelay || _loop) && time > _startDelay)
                    {
                        var dur = _duration / _rateOverLifetime;
                        _t2 += _timeScale == TimeScale.Normal ? Time.deltaTime : Time.unscaledDeltaTime;
                        while (_t2 >= dur)
                        {
                            _t2 -= dur;
                            _orderOverLife++;
                            particles.Insert(0, GenerateParticle(_orderOverLife, 2, null));
                        }
                    }

                //Emit over distance
                if (_rateOverDistance > 0)
                    if (_deltaPosition.magnitude > 1f / _rateOverDistance)
                    {
                        _orderOverDistance++;
                        particles.Insert(0, GenerateParticle(_orderOverDistance, 3, null));
                        _lastPosition = transform.position;
                    }

                //Emit bursts
                if (_bursts != null)
                    for (var i = 0; i < _bursts.Count; i++)
                        if (_burstTimer > _bursts[i].time + _startDelay && _bursts[i].used == false)
                        {
                            for (var j = 0; j < _bursts[i].count; j++)
                                particles.Insert(0, GenerateParticle(j, 0, _bursts[i]));

                            _bursts[i].used = true;
                        }

                if (_loop && _burstTimer >= _duration)
                {
                    _burstTimer = 0;
                    for (var i = 0; i < _bursts.Count; i++) _bursts[i].used = false;
                }

                if (time >= _duration + _startDelay && !_loop) isEmitting = false;

                if (_loop && _loopTimer >= _duration + _startDelay)
                {
                    _loopTimer = 0;
                    _orderPerSec = 0;
                    _orderOverLife = 0;
                    _orderOverDistance = 0;
                }
            }

            if (isPlaying && particles.Count <= 0 && !isEmitting && isMain)
                if (canStop())
                {
                    onStop.Invoke();
                    Stop(true);
                }
        }

        /// <summary>
        ///     Add a burst to the particle system
        /// </summary>
        public void AddBurst(float time, int count)
        {
            _bursts.Add(new Burst(time, count));
        }

        /// <summary>
        ///     Remove burst at index
        /// </summary>
        public void RemoveBurst(int index)
        {
            _bursts.RemoveAt(index);
        }

        /// <summary>
        ///     Set particle burst at index
        /// </summary>
        public void SetBurst(int index, float time, int count)
        {
            if (_bursts.Count > index) _bursts[index] = new Burst(time, count);
        }

        private bool canStop()
        {
            if (children != null)
                return children.All(x => x.isEmitting == false && x.particles.Count <= 0);
            return true;
        }

        private Vector2 GetPointOnRect(float angle, float w, float h)
        {
            // Calculate the sine and cosine of the angle
            var sine = Mathf.Sin(angle);
            var cosine = Mathf.Cos(angle);

            // Calculate the x and y coordinates of the point
            // based on the sign of sine and cosine.
            // If sine is positive, the y coordinate is half the height of the rectangle.
            // If sine is negative, the y coordinate is negative half the height of the rectangle.
            // Similarly, if cosine is positive, the x coordinate is half the width of the rectangle.
            // If cosine is negative, the x coordinate is negative half the width of the rectangle.
            var dy = sine > 0 ? h / 2 : h / -2;
            var dx = cosine > 0 ? w / 2 : w / -2;

            // Check if the slope of the line between the origin and the point is steeper in
            // the x direction or in the y direction. If it is steeper in the x direction,
            // adjust the y coordinate so that the point is on the edge of the rectangle.
            // If it is steeper in the y direction, adjust the x coordinate instead.
            if (Mathf.Abs(dx * sine) < Mathf.Abs(dy * cosine))
                dy = dx * sine / cosine;
            else
                dx = dy * cosine / sine;

            // Return the point as a Vector2 object
            return new Vector2(dx, dy);
        }

        private Particle GenerateParticle(int order, int source, Burst burst)
        {
            float angle = 0;
            if (source == 0) //Burst
                angle = order * (360f / burst.count) * _spreadLoop;
            else if (source == 1) //Rate per Sec
                angle = order * (360f / _rate) / _duration * _spreadLoop;
            else if (source == 2) //Rate over Life
                angle = order * (360f / _rateOverLifetime) * _spreadLoop;
            else if (source == 3) //Rate over Distance
                angle = order * (360f / _rateOverDistance) / _duration * _spreadLoop;

            // Create new particle at system's starting position
            var p = Vector2.zero;
            switch (_shape)
            {
                case EmitterShape.Point:
                    p = _position;
                    break;
                case EmitterShape.Circle:
                    if (_emitOnSurface)
                    {
                        if (_spread == SpreadType.Random)
                            p = _position + Random.insideUnitCircle * _radius;
                        else
                            p = RotateOnAngle(new Vector3(0, Random.Range(0f, 1f), 0), angle) * _radius;
                    }
                    else
                    {
                        if (_spread == SpreadType.Random)
                        {
                            var r = Random.insideUnitCircle.normalized;
                            p = _position + Vector2.Lerp(r * _radius, r * (_radius - _emitterThickness), Random.value);
                        }
                        else
                        {
                            p = RotateOnAngle(new Vector3(0, 1f, 0), angle) *
                                Random.Range(_radius, _radius - _emitterThickness);
                        }
                    }

                    break;
                case EmitterShape.Rectangle:
                    if (_emitOnSurface)
                    {
                        if (_spread == SpreadType.Uniform)
                            p = Vector2.Lerp(GetPointOnRect(angle * Mathf.Deg2Rad, _width, _height), Vector2.one,
                                Random.value);
                        else
                            p = _position + new Vector2(Random.Range(-_width / 2, _width / 2),
                                Random.Range(-_height / 2, _height / 2));
                    }
                    else
                    {
                        var a = Random.Range(0f, 360f);

                        if (_spread == SpreadType.Uniform) a = angle;

                        p = Vector2.Lerp(GetPointOnRect(a * Mathf.Deg2Rad, _width, _height),
                            GetPointOnRect(a * Mathf.Deg2Rad, _width - _emitterThickness, _height - _emitterThickness),
                            Random.value);
                    }

                    break;
                case EmitterShape.Line:
                    if (_spread == SpreadType.Uniform)
                        p = _position + new Vector2(Mathf.Repeat(angle, 361).Remap(0, 360, -_length / 2, _length / 2),
                            0);
                    else
                        p = _position + new Vector2(Random.Range(-_length / 2, _length / 2), 0);

                    break;
                case EmitterShape.Directional:
                    p = _position;
                    break;
            }

            if (space == Simulation.World) p = Quaternion.Euler(transform.eulerAngles) * p;

            var v = Vector2.zero;
            switch (_shape)
            {
                case EmitterShape.Point:
                    if (_spread == SpreadType.Uniform)
                    {
                        v = RotateOnAngle(new Vector3(0, 1f, 0), angle) *
                            _startSpeed.Evaluate(Random.value, Random.value);
                        ;
                    }
                    else
                    {
                        v = Random.insideUnitCircle.normalized * _startSpeed.Evaluate(Random.value, Random.value);
                    }

                    break;
                case EmitterShape.Circle:
                    v = p.normalized * _startSpeed.Evaluate(Random.value, Random.value);
                    break;
                case EmitterShape.Rectangle:
                    v = p.normalized * _startSpeed.Evaluate(Random.value, Random.value);
                    break;
                case EmitterShape.Line:
                    v = (space == Simulation.World ? transform.up : Vector3.up) *
                        _startSpeed.Evaluate(Random.value, Random.value);
                    break;
                case EmitterShape.Directional:
                    float a = 0;
                    if (space == Simulation.World)
                    {
                        if (_spread == SpreadType.Uniform)
                            a = Mathf.Repeat(angle, 361).Remap(0, 360, -_angle / 2, _angle / 2) -
                                transform.eulerAngles.z;
                        else
                            a = Random.Range(-_angle / 2, _angle / 2) - transform.eulerAngles.z;
                    }
                    else
                    {
                        if (_spread == SpreadType.Uniform)
                            a = Mathf.Repeat(angle, 361).Remap(0, 360, -_angle / 2, _angle / 2);
                        else
                            a = Random.Range(-_angle / 2, _angle / 2);
                    }

                    v = RotateOnAngle(a) * _startSpeed.Evaluate(Random.value, Random.value);
                    break;
            }

            var sLerp = Random.value;

            var part = new Particle(
                this,
                p,
                _startRotation.separated
                    ? new Vector3(_startRotation.xCurve.Evaluate(Random.value, Random.value),
                        _startRotation.yCurve.Evaluate(Random.value, Random.value),
                        _startRotation.zCurve.Evaluate(Random.value, Random.value))
                    : new Vector3(0, 0, _startRotation.mainCurve.Evaluate(Random.value, Random.value)),
                v,
                _startColor.Evaluate(Random.value, Random.value),
                _startSize.separated
                    ? new Vector3(_startSize.xCurve.Evaluate(Random.value, Random.value),
                        _startSize.yCurve.Evaluate(Random.value, Random.value),
                        _startSize.zCurve.Evaluate(Random.value, Random.value))
                    : new Vector3(_startSize.mainCurve.Evaluate(sLerp, sLerp),
                        _startSize.mainCurve.Evaluate(sLerp, sLerp), _startSize.mainCurve.Evaluate(sLerp, sLerp)),
                _lifetime.Evaluate(Random.value, Random.value));

            return part;
        }

        private void FitRect()
        {
            switch (_shape)
            {
                // If the emitter has a circle shape, set the radius to half of the smaller
                // of the width and height of the emitter's RectTransform.
                case EmitterShape.Circle:
                    if (rectTransform.rect.width > rectTransform.rect.height)
                        _radius = rectTransform.rect.height / 2;
                    else
                        _radius = rectTransform.rect.width / 2;
                    break;

                // If the emitter has a rectangle shape, set the width and height of the emitter
                // to the width and height of the RectTransform.
                case EmitterShape.Rectangle:
                    _width = rectTransform.rect.width;
                    _height = rectTransform.rect.height;
                    break;

                // If the emitter has a line shape, set the length of the emitter to the width
                // of the RectTransform.
                case EmitterShape.Line:
                    _length = rectTransform.rect.width;
                    break;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            for (var i = 0; i < particles.Count; i++)
            {
                particles[i].Animate();
                particles[i].Render(vh);
            }
        }

        private Vector3 RotateOnAngle(float angle)
        {
            var rad = angle * Mathf.Deg2Rad;
            var position = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0);
            return position * 1f;
        }

        private Vector3 RotateOnAngle(Vector3 p, float angle)
        {
            return Quaternion.Euler(new Vector3(0, 0, angle)) * p;
        }

        /// <summary>
        ///     Converts world position to viewport position using the current camera
        /// </summary>
        /// <param name="position">World position</param>
        /// <returns>Viewport position</returns>
        public Vector3 WorldToViewportPoint(Vector3 position)
        {
            var pos = camera.WorldToViewportPoint(position);
            return pos;
        }
    }

    [Serializable]
    public class Burst
    {
        public float time;
        public int count = 1;
        public bool used;

        public Burst(float time, int count)
        {
            this.time = time;
            this.count = count;
        }
    }

    [Serializable]
    public struct SpeedRange
    {
        public float from;
        public float to;

        public SpeedRange(float from, float to)
        {
            this.from = from;
            this.to = to;
        }
    }

    [Serializable]
    public struct Module
    {
        public bool enabled;

        public Module(bool enabled)
        {
            this.enabled = enabled;
        }
    }

    [Serializable]
    public struct SeparatedMinMaxCurve
    {
        [SerializeField] private bool separable;

        public bool separated;
        public ParticleSystem.MinMaxCurve mainCurve;
        public ParticleSystem.MinMaxCurve xCurve;
        public ParticleSystem.MinMaxCurve yCurve;
        public ParticleSystem.MinMaxCurve zCurve;

        public SeparatedMinMaxCurve(float startValue, bool separated = false, bool separable = true)
        {
            mainCurve = new ParticleSystem.MinMaxCurve(startValue);
            xCurve = new ParticleSystem.MinMaxCurve(startValue);
            yCurve = new ParticleSystem.MinMaxCurve(startValue);
            zCurve = new ParticleSystem.MinMaxCurve(startValue);
            this.separated = separated;
            this.separable = separable;
        }

        public SeparatedMinMaxCurve(AnimationCurve startValue, bool separated = false, bool separable = true)
        {
            mainCurve = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(startValue.keys));
            xCurve = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(startValue.keys));
            yCurve = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(startValue.keys));
            zCurve = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(startValue.keys));
            this.separated = separated;
            this.separable = separable;
        }
    }

    public static class Extensions
    {
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            var v = (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            if (float.IsNaN(v) || float.IsInfinity(v))
                return 0;
            return v;
        }
    }
}

namespace AssetKits.ParticleImage.Enumerations
{
    public enum EmitterShape
    {
        Point,
        Circle,
        Rectangle,
        Line,
        Directional
    }

    public enum SpreadType
    {
        Random,
        Uniform
    }

    public enum Simulation
    {
        Local,
        World
    }

    public enum AttractorType
    {
        Pivot,
        Surface
    }

    public enum PlayMode
    {
        None,
        OnEnable,
        OnAwake
    }

    public enum SheetType
    {
        Lifetime,
        Speed,
        FPS
    }

    public enum TimeScale
    {
        Unscaled,
        Normal
    }
}