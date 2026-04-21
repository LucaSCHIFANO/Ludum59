using System;
using UnityEngine;

/// <summary>
/// A class that stores all the inputs a player can enter at the same time
/// </summary>
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
