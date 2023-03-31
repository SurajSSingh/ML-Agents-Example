using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressableButton : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] Material unpressedMat;
    [SerializeField] Material pressedMat;

    [Header("Colors: used when no material is found")]
    [SerializeField] Color unpressedColor = Color.red;
    [SerializeField] Color pressedColor = Color.green;
    private bool buttonPressed = false;
    private bool buttonTouched = false;

    void Awake()
    {
        ResetButton();
    }
    public void ResetButton()
    {
        buttonPressed = false;
        if (gameObject.GetComponent<Renderer>() is Renderer renderer)
        {
            if (unpressedMat) renderer.material = unpressedMat;
            else renderer.material.color = unpressedColor;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !buttonTouched)
        {
            buttonTouched = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && buttonTouched)
        {
            buttonTouched = false;
        }
    }

    public bool PressButton()
    {
        if (buttonTouched && !buttonPressed)
        {
            buttonPressed = true;
            if (gameObject.GetComponent<Renderer>() is Renderer renderer)
            {
                if (unpressedMat) renderer.material = pressedMat;
                else renderer.material.color = pressedColor;
            }
            return true;
        }
        return false;
    }
}
