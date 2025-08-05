using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    public float maxStamina = 5f;
    public float currentStamina;

    public float staminaDrainRate = 1f;
    public float staminaRegenRate = 0.5f;

    public Slider leftSlider;
    public Slider rightSlider;

    void Start()
    {
        currentStamina = maxStamina;
        leftSlider.maxValue = maxStamina / 2f;
        rightSlider.maxValue = maxStamina / 2f;

        leftSlider.value = maxStamina / 2f;
        rightSlider.value = maxStamina / 2f;
    }

    void Update()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift); // O la tecla que uses

        if (isRunning && currentStamina > 0)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
        else
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }

        float halfStamina = currentStamina / 2f;
        leftSlider.value = halfStamina;
        rightSlider.value = halfStamina;
    }
}
