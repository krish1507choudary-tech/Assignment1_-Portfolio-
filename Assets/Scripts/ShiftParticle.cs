using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class ShiftParticle : MonoBehaviour
{
    public VisualEffect particle;

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.leftShiftKey.isPressed)
        {
            particle.Play();
        }
        else
        {
            particle.Stop();
        }
    }
}