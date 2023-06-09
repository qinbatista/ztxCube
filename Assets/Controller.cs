using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
enum SwipeDirection
{
    horizontal,
    vertical
}
public class Controller : MonoBehaviour
{
    bool _isPressedScreen;
    Vector2 _previousPosition;
    Vector2 _touchDelta;
    [SerializeField] Camera _camera;
    [SerializeField] Transform _targetTransform;
    bool isRotateStart = false;
    public void Move(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();
        if(_isPressedScreen)
        {
            _touchDelta = _previousPosition - context.ReadValue<Vector2>();
        }
        else
        {
            _touchDelta = Vector2.zero;
        }
        // print("_touchDelta=" + _touchDelta);
        if (_isPressedScreen)
        {
            Ray ray = _camera.ScreenPointToRay(context.ReadValue<Vector2>());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                LeftSwipe(_touchDelta);
                RightSwipe(_touchDelta);
                UpSwipe(_touchDelta);
                DownSwipe(_touchDelta);
                _touchDelta = Vector2.zero;
            }
            else
            {
                if (isRotateStart) return;
                print("RotateAround=" + _touchDelta);
                _camera.transform.RotateAround(_targetTransform.position, Vector3.up, -_touchDelta.x * 30f * Time.deltaTime);
                _camera.transform.RotateAround(_targetTransform.position, Vector3.right, _touchDelta.y * 30f * Time.deltaTime);
            }
        }

        _previousPosition = context.ReadValue<Vector2>();
    }
    public void Press(InputAction.CallbackContext context)
    {
        _isPressedScreen = context.ReadValue<float>() == 0f ? false : true;
        if (context.canceled)
        {
            _touchDelta = Vector2.zero;
        }
    }
    bool LeftSwipe(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y) && delta.x > 0 && !isRotateStart)
        {
            isRotateStart = true;
            StartCoroutine(Rotate(90, SwipeDirection.horizontal));
            return true;
        }
        else
            return false;
    }
    bool RightSwipe(Vector2 swipe)
    {
        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y) && swipe.x < 0 && !isRotateStart)
        {
            isRotateStart = true;
            StartCoroutine(Rotate(-90, SwipeDirection.horizontal));
            return true;
        }
        else
            return false;
    }
    bool UpSwipe(Vector2 swipe)
    {
        if (Mathf.Abs(swipe.x) < Mathf.Abs(swipe.y) && swipe.y > 0 && !isRotateStart)
        {
            isRotateStart = true;
            StartCoroutine(Rotate(-90, SwipeDirection.vertical));
            return true;
        }
        else
            return false;
    }
    bool DownSwipe(Vector2 swipe)
    {
        if (Mathf.Abs(swipe.x) < Mathf.Abs(swipe.y) && swipe.y < 0 && !isRotateStart)
        {
            isRotateStart = true;
            StartCoroutine(Rotate(90, SwipeDirection.vertical));
            return true;
        }
        else
            return false;
    }
    IEnumerator Rotate(float angle, SwipeDirection direction)
    {
        Vector3 targetRotation;
        if (direction == SwipeDirection.horizontal)
            targetRotation = new Vector3(_targetTransform.localEulerAngles.x, _targetTransform.localEulerAngles.y + angle, _targetTransform.localEulerAngles.z);
        else
        {
            print("<0:" + _targetTransform.localEulerAngles);
            if (((int)_targetTransform.localEulerAngles.y / 90) % 2 == 0)
            {
                if ((int)_targetTransform.localEulerAngles.y / 90 == 2)
                    targetRotation = new Vector3(_targetTransform.localEulerAngles.x - angle, _targetTransform.localEulerAngles.y, _targetTransform.localEulerAngles.z);
                else
                    targetRotation = new Vector3(_targetTransform.localEulerAngles.x + angle, _targetTransform.localEulerAngles.y, _targetTransform.localEulerAngles.z);
            }
            else
            {
                if ((int)(_targetTransform.localEulerAngles.y - 90) / 90 == 2)
                    targetRotation = new Vector3(_targetTransform.localEulerAngles.x, _targetTransform.localEulerAngles.y, _targetTransform.localEulerAngles.z - angle);
                else
                    targetRotation = new Vector3(_targetTransform.localEulerAngles.x, _targetTransform.localEulerAngles.y, _targetTransform.localEulerAngles.z + angle);
            }
        }

        Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
        while (true)
        {
            if (_targetTransform.rotation == Quaternion.Euler(targetRotation))
            {
                isRotateStart = false;
                break;
            }
            _targetTransform.rotation = Quaternion.RotateTowards(_targetTransform.rotation, targetQuaternion, 180f * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
    }
}
