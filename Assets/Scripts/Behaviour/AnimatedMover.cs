using System;
using System.Collections;
using UnityEngine;

public class AnimatedMover : MonoBehaviour
{
    private Transform _transform;
    private Vector3 _destination;
    private Vector3 _delta;
    private float _steps;
    private Coroutine _coroutine_instance;
    internal Vector3 localPosition { get => _destination; }

    public Action OnComplete;

    void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    public void Move(Vector3 pos, bool with_tween, float seconds = 1.0f)
    {
        Stop();
        _destination = pos;

        if (with_tween)
        {
            _steps = Mathf.Floor(seconds / Time.deltaTime);
            _delta = (_destination - _transform.localPosition) / _steps;
            _coroutine_instance = StartCoroutine(MovingCoroutine());
        }
        else
            _transform.localPosition = _destination;
    }

    IEnumerator MovingCoroutine()
    {
        while (_steps-- > 0)
        {
            _transform.localPosition += _delta;
            yield return new WaitForEndOfFrame();
        }

        _transform.localPosition = _destination;
        OnComplete?.Invoke();
        yield return null;
    }

    internal void Dispose()
    {
        Stop();
    }

    internal void Stop()
    {
        if (_coroutine_instance == null)
            return;

        StopCoroutine(_coroutine_instance);
        _coroutine_instance = null;
    }
}
