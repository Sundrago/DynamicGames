// Version: 1.1.0

using System.Collections.Generic;
using AssetKits.ParticleImage.Enumerations;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AssetKits.ParticleImage
{
    public class Particle
    {
        private readonly float _attractorLerp;

        private readonly Vector2 _attractorTargetPoint;
        private readonly float _colorLerp;
        private Vector2 _deltaPosition;
        private Vector3 _deltaRotation;
        private Vector3 _direction;
        private Vector2 _finalVelocity;

        private float _frameDelta;
        private int _frameId;
        private readonly float _frameOverTimeLerp;
        private readonly float _gravityLerp;
        private Vector2 _gravityVelocity;

        private Vector2 _lastPoint;

        private Vector2 _lastPos;

        private Vector3 _lastPosition;
        private Quaternion _lastRotation;

        private Vector2 _modifiedPosition;
        private Vector2 _noiseVelocity;
        private Vector2 _position;
        private readonly float _ratioRandom;
        private readonly float _rotateLerp;
        private int _sheetId;
        private readonly List<SpriteSheet> _sheetsList;
        private Vector3 _size;

        private readonly float _sizeLerp;
        private readonly ParticleImage _source;
        private readonly float _speedLerp;
        private readonly Color _startColor;
        private readonly float _startFrameLerp;
        private readonly Vector3 _startRotation;
        private readonly Vector3 _startSize;
        private Vector2 _startVelocity;
        private Vector2 _trailDeltaPos;

        private readonly VertexHelper _trailHelper;

        private Vector2 _trailLastPos;

        private readonly Transform _transform;
        private readonly float _velocityXLerp;
        private readonly float _velocityYLerp;
        private Vector2 _veloVelocity;
        private readonly float _vortexLerp;

        private Vector3 rot;

        public Particle(ParticleImage source, Vector2 pos, Vector3 rot, Vector2 vel, Color col, Vector3 siz, float life)
        {
            _source = source;
            _transform = source.transform;
            _position = pos;
            _startVelocity = vel;
            _startColor = col;
            _startSize = siz;
            _startRotation = rot;

            _sizeLerp = Random.value;
            _colorLerp = Random.value;
            _rotateLerp = Random.value;
            _attractorLerp = Random.value;
            _gravityLerp = Random.value;
            _vortexLerp = Random.value;
            _startFrameLerp = Random.value;
            _frameOverTimeLerp = Random.value;
            _velocityXLerp = Random.value;
            _velocityYLerp = Random.value;
            _speedLerp = Random.value;
            _ratioRandom = Random.value;

            _attractorTargetPoint = new Vector2(Random.value, Random.value);

            Lifetime = life;

            _sheetsList = new List<SpriteSheet>();

            _lastPosition = _transform.position;

            if (source.textureSheetEnabled)
                for (var i = source.textureTile.y - 1; i > -1; i--)
                for (var j = 0; j < source.textureTile.x; j++)
                    _sheetsList.Add(new SpriteSheet(
                        new Vector2(1f / source.textureTile.x * (j + 1), 1f / source.textureTile.y * (i + 1)),
                        new Vector2(1f / source.textureTile.x * j, 1f / source.textureTile.y * i)));
            else
                _sheetsList.Add(new SpriteSheet(new Vector2(1f, 1f), new Vector2(0f, 0f)));

            _frameId += (int)source.textureSheetStartFrame.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                _startFrameLerp);

            _lastPoint = _position;
            _modifiedPosition = _position;
            _lastPos = _position;
            _trailLastPos = _position;
            _trailHelper = new VertexHelper();
            if (_source.trailsEnabled) trailPoints.Add(new TrailPoint(_position, 0f));
        }

        public List<TrailPoint> trailPoints { get; } = new();

        public Vector2 Position => _position;
        public Vector2 Velocity => _finalVelocity;
        public Vector2 Size => new(_size.x, _size.y);
        public float TimeSinceBorn { get; private set; }

        public float Lifetime { get; }

        public Color Color { get; private set; }

        public void Animate()
        {
            TimeSinceBorn += _source.timeScale == TimeScale.Normal ? Time.deltaTime : Time.unscaledDeltaTime;

            if (TimeSinceBorn > Lifetime) return;

            _finalVelocity = _startVelocity *
                             _source.speedOverLifetime.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f), _speedLerp);

            if (_source.space == Simulation.World)
            {
                _modifiedPosition += new Vector2(
                    _transform.InverseTransformPoint(_lastPosition).x,
                    _transform.InverseTransformPoint(_lastPosition).y);

                _deltaRotation = Quaternion.Inverse(_source.transform.rotation).eulerAngles -
                                 Quaternion.Inverse(_lastRotation).eulerAngles;

                _modifiedPosition = new Vector2(
                    RotatePointAroundCenter(_modifiedPosition, _deltaRotation).x,
                    RotatePointAroundCenter(_modifiedPosition, _deltaRotation).y);

                _startVelocity = new Vector2(
                    RotatePointAroundCenter(_startVelocity, _deltaRotation).x,
                    RotatePointAroundCenter(_startVelocity, _deltaRotation).y);

                _lastPosition = _transform.position;
                _lastRotation = _source.transform.rotation;
            }

            #region VELOCITY

            if (_source.velocityEnabled)
            {
                if (_source.velocitySpace == Simulation.World)
                {
                    _veloVelocity = new Vector2(
                        RotatePointAroundCenter(
                            new Vector2(
                                _source.velocityOverLifetime.xCurve.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                                    _velocityXLerp),
                                _source.velocityOverLifetime.yCurve.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                                    _velocityYLerp)), Quaternion.Inverse(_source.transform.rotation).eulerAngles).x,
                        RotatePointAroundCenter(
                            new Vector2(
                                _source.velocityOverLifetime.xCurve.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                                    _velocityXLerp),
                                _source.velocityOverLifetime.yCurve.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                                    _velocityYLerp)), Quaternion.Inverse(_source.transform.rotation).eulerAngles).y
                    );
                }
                else
                {
                    if (_source.velocityOverLifetime.separated)
                        _veloVelocity = new Vector2(
                            _source.velocityOverLifetime.xCurve.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                                _velocityXLerp),
                            _source.velocityOverLifetime.yCurve.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                                _velocityYLerp));
                    else
                        _veloVelocity =
                            new Vector2(
                                _source.velocityOverLifetime.mainCurve.Evaluate(
                                    TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f), _velocityXLerp), 0);
                }
            }

            #endregion

            #region GRAVITY

            if (_source.gravityEnabled)
                //Apply gravity
                _gravityVelocity += new Vector2(
                                        RotatePointAroundCenter(
                                            new Vector3(0,
                                                _source.gravity.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                                                    _gravityLerp), 0),
                                            Quaternion.Inverse(_source.transform.rotation).eulerAngles).x,
                                        RotatePointAroundCenter(
                                            new Vector3(0,
                                                _source.gravity.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                                                    _gravityLerp), 0),
                                            Quaternion.Inverse(_source.transform.rotation).eulerAngles).y) *
                                    (_source.timeScale == TimeScale.Normal ? Time.deltaTime : Time.unscaledDeltaTime);

            #endregion

            #region NOISE

            if (_source.noiseEnabled)
            {
                var noise = 0f;

                if (_source.space == Simulation.Local)
                    noise = _source.noise.GetNoise(_position.x, _position.y);
                else
                    noise = _source.noise.GetNoise(
                        (_position + new Vector2(_source.transform.localPosition.x, _source.transform.localPosition.y))
                        .x,
                        (_position + new Vector2(_source.transform.localPosition.x, _source.transform.localPosition.y))
                        .y);

                _noiseVelocity = new Vector2(
                    Mathf.Cos(noise * Mathf.PI),
                    Mathf.Sin(noise * Mathf.PI)) * _source.noiseStrength;
            }

            #endregion

            _finalVelocity += _veloVelocity + _noiseVelocity + _gravityVelocity;

            _modifiedPosition += _finalVelocity *
                                 ((_source.timeScale == TimeScale.Normal ? Time.deltaTime : Time.unscaledDeltaTime) *
                                  100);

            #region VORTEX

            if (_source.vortexEnabled)
                _modifiedPosition = RotatePointAroundCenter(_modifiedPosition,
                    new Vector3(0, 0,
                        _source.vortexStrength.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f), _vortexLerp) *
                        (_source.timeScale == TimeScale.Normal ? Time.deltaTime : Time.unscaledDeltaTime) * 100));

            #endregion

            #region ATTRACTOR

            if (_source.attractorTarget && _source.attractorEnabled)
            {
                var canPos = Vector3.zero;

                if (_source.attractorTarget is RectTransform)
                {
                    canPos = _transform.InverseTransformPoint(_source.attractorTarget.position);
                }
                else
                {
                    var viewportPos = _source.WorldToViewportPoint(_source.attractorTarget.position);
                    _source.attractorType = AttractorType.Pivot;

                    if (_source.canvas.renderMode == RenderMode.ScreenSpaceCamera)
                        canPos = new Vector3(
                            (viewportPos.x.Remap(0.5f, 1.5f, 0f, _source.canvasRect.rect.width) -
                             _source.canvasRect.InverseTransformPoint(_transform.position).x +
                             _source.canvasRect.localPosition.x) / _transform.lossyScale.x *
                            _source.canvasRect.localScale.x,
                            (viewportPos.y.Remap(0.5f, 1.5f, 0f, _source.canvasRect.rect.height) -
                             _source.canvasRect.InverseTransformPoint(_transform.position).y +
                             _source.canvasRect.localPosition.y) / _transform.lossyScale.y *
                            _source.canvasRect.localScale.y,
                            0);
                    else
                        canPos = new Vector3(
                            (viewportPos.x.Remap(0.5f, 1.5f, 0f, _source.canvasRect.rect.width) -
                             _source.canvasRect.InverseTransformPoint(_transform.position).x) /
                            _transform.lossyScale.x * _source.canvasRect.localScale.x,
                            (viewportPos.y.Remap(0.5f, 1.5f, 0f, _source.canvasRect.rect.height) -
                             _source.canvasRect.InverseTransformPoint(_transform.position).y) /
                            _transform.lossyScale.y * _source.canvasRect.localScale.y,
                            0);
                }


                if (_source.attractorType == AttractorType.Pivot)
                {
                    _position = Vector3.LerpUnclamped(_modifiedPosition, canPos,
                        _source.attractorLerp.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f), _attractorLerp));
                }
                else
                {
                    var rt = _source.attractorTarget as RectTransform;

                    _position = Vector3.LerpUnclamped(_modifiedPosition,
                        new Vector2(
                            canPos.x + _attractorTargetPoint.x.Remap(0f, 1f, -rt.sizeDelta.x / 2, rt.sizeDelta.x / 2),
                            canPos.y + _attractorTargetPoint.y.Remap(0f, 1f, -rt.sizeDelta.y / 2, rt.sizeDelta.y / 2)),
                        _source.attractorLerp.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f), _attractorLerp));
                }
            }
            else
            {
                _position = _modifiedPosition;
            }

            #endregion

            _deltaPosition = _position - _lastPos;
            _lastPos = _position;

            if (_source.trailsEnabled && _ratioRandom <= _source.trailRatio)
            {
                _trailDeltaPos = _trailLastPos - _position;
                if (_trailDeltaPos.magnitude > _source.minimumVertexDistance)
                {
                    _trailLastPos = _position;
                    trailPoints.Add(new TrailPoint(_position, TimeSinceBorn));
                }
            }

            var c = _source.colorOverLifetime.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f), _colorLerp);
            //_color = _startColor * c * _source.colorBySpeed.Evaluate(_finalVelocity.magnitude.Remap(_source.colorSpeedRange.from, _source.colorSpeedRange.to, 0f, 1f));
            var _normalizedSpeed = _deltaPosition.magnitude * (1f / Time.deltaTime) / 100f;
            Color = _startColor * c *
                    _source.colorBySpeed.Evaluate(_normalizedSpeed.Remap(_source.colorSpeedRange.from,
                        _source.colorSpeedRange.to, 0f, 1f));

            var sol = Vector3.one;

            if (_source.sizeOverLifetime.separated)
                sol = new Vector3(
                    _source.sizeOverLifetime.xCurve.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f), _sizeLerp),
                    _source.sizeOverLifetime.yCurve.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f), _sizeLerp),
                    _source.sizeOverLifetime.zCurve.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f), _sizeLerp));
            else
                sol = new Vector3(
                    _source.sizeOverLifetime.mainCurve.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f), _sizeLerp),
                    _source.sizeOverLifetime.mainCurve.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f), _sizeLerp),
                    _source.sizeOverLifetime.mainCurve.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f), _sizeLerp));

            var sbs = Vector3.one;

            if (_source.sizeBySpeed.separated)
                sbs = new Vector3(
                    _source.sizeBySpeed.xCurve.Evaluate(
                        _normalizedSpeed.Remap(_source.sizeSpeedRange.from, _source.sizeSpeedRange.to, 0f, 1f),
                        _sizeLerp),
                    _source.sizeBySpeed.yCurve.Evaluate(
                        _normalizedSpeed.Remap(_source.sizeSpeedRange.from, _source.sizeSpeedRange.to, 0f, 1f),
                        _sizeLerp),
                    _source.sizeBySpeed.zCurve.Evaluate(
                        _normalizedSpeed.Remap(_source.sizeSpeedRange.from, _source.sizeSpeedRange.to, 0f, 1f),
                        _sizeLerp));
            else
                sbs = new Vector3(
                    _source.sizeBySpeed.mainCurve.Evaluate(
                        _normalizedSpeed.Remap(_source.sizeSpeedRange.from, _source.sizeSpeedRange.to, 0f, 1f),
                        _sizeLerp),
                    _source.sizeBySpeed.mainCurve.Evaluate(
                        _normalizedSpeed.Remap(_source.sizeSpeedRange.from, _source.sizeSpeedRange.to, 0f, 1f),
                        _sizeLerp),
                    _source.sizeBySpeed.mainCurve.Evaluate(
                        _normalizedSpeed.Remap(_source.sizeSpeedRange.from, _source.sizeSpeedRange.to, 0f, 1f),
                        _sizeLerp));

            _size = _startSize;

            _size = new Vector3(_size.x * sol.x, _size.y * sol.y, _size.z * sol.z);

            _size = new Vector3(_size.x * sbs.x, _size.y * sbs.y, _size.z * sbs.z);
            //source.SizeBySpeed.Evaluate(m_Velocity.magnitude.Remap(source.SizeSpeedRange.start,source.SizeSpeedRange.to, 0f, 1f))
        }

        public void Render(VertexHelper vh)
        {
            if (_source.trailsEnabled && _ratioRandom <= _source.trailRatio)
            {
                _trailHelper.Clear();

                if (trailPoints.Count > 1)
                {
                    var tp = new TrailPoint(_position, trailPoints[trailPoints.Count - 1].time);
                    trailPoints[trailPoints.Count - 1] = tp;

                    if (TimeSinceBorn >= trailPoints[0].time + _source.trailLifetime) trailPoints.RemoveAt(0);
                }

                for (var j = 0; j < trailPoints.Count; j++)
                {
                    var v = trailPoints[j].point;

                    if (j < trailPoints.Count - 1) v = trailPoints[j + 1].point - trailPoints[j].point;

                    var mid = trailPoints[j].point; // the mid-point between start and end.
                    var perp = Vector2.Perpendicular(v.normalized); // vector of length 1 perpendicular to v.

                    var vertex = UIVertex.simpleVert;
                    if (_source.inheritParticleColor)
                        vertex.color =
                            Color *
                            _source.trailColorOverTrail.Evaluate(((float)j).Remap(0, trailPoints.Count, 1f, 0f)) *
                            _source.trailColorOverLifetime.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                                _colorLerp);
                    else
                        vertex.color =
                            _source.trailColorOverTrail.Evaluate(((float)j).Remap(0, trailPoints.Count, 1f, 0f)) *
                            _source.trailColorOverLifetime.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                                _colorLerp);

                    var width = _size.x * _source.trailWidth.Evaluate(((float)j).Remap(0, trailPoints.Count, 1f, 0f));

                    // move half the thickness away from the mid-point.
                    vertex.position = mid + perp * width / 2;
                    vertex.uv0 = new Vector2(0, 1f / trailPoints.Count * j);
                    _trailHelper.AddVert(vertex);

                    // move half the thickness away from the mid-point in the opposite direction.
                    vertex.position = mid - perp * width / 2;
                    vertex.uv0 = new Vector2(1, 1f / trailPoints.Count * j);
                    _trailHelper.AddVert(vertex);
                }

                for (var v = 0; v + 2 < _trailHelper.currentVertCount; v += 2)
                    _trailHelper.AddTriangle(v, v + 2, v + 1);

                for (var v = 0; v + 3 < _trailHelper.currentVertCount; v += 2)
                    _trailHelper.AddTriangle(v + 1, v + 2, v + 3);

                var verts = new List<UIVertex>();
                var verts2 = new List<UIVertex>();
                _trailHelper.GetUIVertexStream(verts);
                _source.particleTrailRenderer.vertexHelper.GetUIVertexStream(verts2);
                verts.AddRange(verts2);
                _source.particleTrailRenderer.vertexHelper.AddUIVertexTriangleStream(verts);
            }

            if (TimeSinceBorn > Lifetime)
            {
                if (_source.dieWithParticle || trailPoints.Count <= 1) trailPoints.Clear();

                return;
            }

            _direction = _position - _lastPoint;

            if (_direction.magnitude == 0f) _direction = _finalVelocity;

            _direction = _direction.normalized;

            _lastPoint = _position;

            var i = vh.currentVertCount;

            var vert = new UIVertex();
            //sheetId = Random.Range(0, sheets.Count);

            switch (_source.textureSheetType)
            {
                case SheetType.Speed:
                    _frameId = (int)_finalVelocity.magnitude.Remap(_source.textureSheetFrameSpeedRange.from,
                        _source.textureSheetFrameSpeedRange.to, 0f, _sheetsList.Count);
                    break;
                case SheetType.Lifetime:
                    _frameId = (int)(_source.textureSheetFrameOverTime.Evaluate(
                                   TimeSinceBorn.Remap(0, Lifetime, 0f, 1f),
                                   _frameOverTimeLerp) * _source.textureSheetCycles) +
                               (int)_source.textureSheetStartFrame.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                                   _startFrameLerp);
                    break;
                case SheetType.FPS:
                    var dur = 1f / _source.textureSheetFPS;
                    _frameDelta += _source.timeScale == TimeScale.Normal ? Time.deltaTime : Time.unscaledDeltaTime;
                    while (_frameDelta >= dur)
                    {
                        _frameDelta -= dur;
                        _frameId++;
                    }

                    break;
            }

            _sheetId = (int)Mathf.Repeat(_frameId, _sheetsList.Count);

            var rol = Vector3.zero;

            if (_source.rotationOverLifetime.separated)
            {
                var x = 0f;
                var y = 0f;
                var z = 0f;

                if (_source.rotationOverLifetime.xCurve.mode == ParticleSystemCurveMode.Constant ||
                    _source.rotationOverLifetime.xCurve.mode == ParticleSystemCurveMode.TwoConstants)
                    x = TimeSinceBorn.Remap(0f, Lifetime, 0f,
                        _source.rotationOverLifetime.xCurve.Evaluate(_rotateLerp, _rotateLerp));
                else
                    x = _source.rotationOverLifetime.xCurve.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                        _rotateLerp);
                if (_source.rotationOverLifetime.yCurve.mode == ParticleSystemCurveMode.Constant ||
                    _source.rotationOverLifetime.yCurve.mode == ParticleSystemCurveMode.TwoConstants)
                    y = TimeSinceBorn.Remap(0f, Lifetime, 0f,
                        _source.rotationOverLifetime.yCurve.Evaluate(_rotateLerp, _rotateLerp));
                else
                    y = _source.rotationOverLifetime.yCurve.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                        _rotateLerp);
                if (_source.rotationOverLifetime.zCurve.mode == ParticleSystemCurveMode.Constant ||
                    _source.rotationOverLifetime.zCurve.mode == ParticleSystemCurveMode.TwoConstants)
                    z = TimeSinceBorn.Remap(0f, Lifetime, 0f,
                        _source.rotationOverLifetime.zCurve.Evaluate(_rotateLerp, _rotateLerp));
                else
                    z = _source.rotationOverLifetime.zCurve.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                        _rotateLerp);

                rol = new Vector3(x, y, z);

                if (!_source.alignToDirection) rol += Quaternion.Inverse(_source.transform.rotation).eulerAngles;
            }
            else
            {
                switch (_source.rotationOverLifetime.mainCurve.mode)
                {
                    case ParticleSystemCurveMode.Constant:
                    case ParticleSystemCurveMode.TwoConstants:
                        rol = new Vector3(0, 0, TimeSinceBorn.Remap(0f, Lifetime, 0f,
                            _source.rotationOverLifetime.mainCurve.Evaluate(TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                                _rotateLerp)));
                        break;
                    case ParticleSystemCurveMode.Curve:
                    case ParticleSystemCurveMode.TwoCurves:
                        rol = new Vector3(0, 0, _source.rotationOverLifetime.mainCurve.Evaluate(
                            TimeSinceBorn.Remap(0f, Lifetime, 0f, 1f),
                            _rotateLerp));
                        break;
                }

                if (!_source.alignToDirection)
                    rol += new Vector3(0, 0, Quaternion.Inverse(_source.transform.rotation).eulerAngles.z);
            }

            var rbs = Vector3.zero;

            if (_source.rotationBySpeed.separated)
                rbs = new Vector3(
                    TimeSinceBorn.Remap(0f, Lifetime, 0f,
                        _source.rotationBySpeed.xCurve.Evaluate(
                            _finalVelocity.magnitude.Remap(_source.rotationSpeedRange.from,
                                _source.rotationSpeedRange.to, 0f, 1f), _rotateLerp)),
                    TimeSinceBorn.Remap(0f, Lifetime, 0f,
                        _source.rotationBySpeed.yCurve.Evaluate(
                            _finalVelocity.magnitude.Remap(_source.rotationSpeedRange.from,
                                _source.rotationSpeedRange.to, 0f, 1f), _rotateLerp)),
                    TimeSinceBorn.Remap(0f, Lifetime, 0f,
                        _source.rotationBySpeed.zCurve.Evaluate(
                            _finalVelocity.magnitude.Remap(_source.rotationSpeedRange.from,
                                _source.rotationSpeedRange.to, 0f, 1f), _rotateLerp)));
            else
                rbs = new Vector3(0, 0,
                    TimeSinceBorn.Remap(0f, Lifetime, 0f,
                        _source.rotationBySpeed.mainCurve.Evaluate(
                            _finalVelocity.magnitude.Remap(_source.rotationSpeedRange.from,
                                _source.rotationSpeedRange.to, 0f, 1f), _rotateLerp)));

            if (_source.alignToDirection)
            {
                var q = Quaternion.FromToRotation(Vector3.up, _direction);
                rot = _startRotation + Quaternion.Euler(new Vector3(0, 0, q.eulerAngles.z)).eulerAngles;
            }
            else
            {
                rot = _startRotation;
            }


            vert.color = Color;
            vert.position = RotatePointAroundPivot(new Vector3(_position.x - _size.x / 2, _position.y - _size.y / 2),
                _position, rot + rol + rbs);
            vert.uv0 = _sheetsList[_sheetId].pos;
            vh.AddVert(vert);

            vert.position = RotatePointAroundPivot(new Vector3(_position.x + _size.x / 2, _position.y - _size.y / 2),
                _position, rot + rol + rbs);
            vert.uv0 = new Vector2(_sheetsList[_sheetId].size.x, _sheetsList[_sheetId].pos.y);
            vh.AddVert(vert);

            vert.position = RotatePointAroundPivot(new Vector3(_position.x + _size.x / 2, _position.y + _size.y / 2),
                _position, rot + rol + rbs);
            vert.uv0 = _sheetsList[_sheetId].size;
            vh.AddVert(vert);

            vert.position = RotatePointAroundPivot(new Vector3(_position.x - _size.x / 2, _position.y + _size.y / 2),
                _position, rot + rol + rbs);
            vert.uv0 = new Vector2(_sheetsList[_sheetId].pos.x, _sheetsList[_sheetId].size.y);
            vh.AddVert(vert);

            vh.AddTriangle(i + 0, i + 2, i + 1);
            vh.AddTriangle(i + 3, i + 2, i + 0);
        }

        private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            return Quaternion.Euler(angles) * (point - pivot) + pivot;
        }

        private Vector3 RotatePointAroundCenter(Vector3 point, Vector3 angles)
        {
            return Quaternion.Euler(angles) * point;
        }

        public struct TrailPoint
        {
            public Vector2 point;
            public float time;

            public TrailPoint(Vector2 p, float t)
            {
                point = p;
                time = t;
            }
        }
    }

    internal struct SpriteSheet
    {
        public Vector2 size;
        public Vector2 pos;

        public SpriteSheet(Vector2 s, Vector2 p)
        {
            size = s;
            pos = p;
        }
    }
}