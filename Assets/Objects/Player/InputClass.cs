using System;
using UnityEngine;

[Serializable]
public class InputClass
{
    public float joystickXPosition;
    public bool jumpButtonPressed;
    public bool jumpPerformed;


    public InputClass(float _joystickXPosition, bool _jumpButtonPressed, bool _jumpPerformed)
    {
        joystickXPosition = _joystickXPosition;
        jumpButtonPressed = _jumpButtonPressed;
        jumpPerformed = _jumpPerformed;
    } 
}
