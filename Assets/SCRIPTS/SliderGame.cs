using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SliderGame : MonoBehaviour
{
    public Slider slider;
    public float speed = 1.0f;
    public RectTransform panel;

    public float panelWidth = 200.0f;
    public float minX = 0.0f;
    public float maxX = 1.0f;

    private bool increasing = true;
    private bool stopSlider = false;
    public bool spawnOnce = true;

    private int consecutiveMisses = 0;
    private bool checkHandlePosition = false;
    
    public GameObject Restart;
    public GameObject Loader;

    public GameObject LOST;
    public GameObject WON;

    public List<Image> colorChangeImages;

    public Animator Enemy;
    public Animator Löwe;

    [Range(0, 1)]
    public float redValue = 1.0f;
    [Range(0, 1)]
    public float greenValue = 1.0f;
    [Range(0, 1)]
    public float blueValue = 1.0f;

    void Start()
    {
        RandomizePanelPosition();
    }

    void Update()
    {
        if (!stopSlider)
        {
            UpdateSliderValue();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            checkHandlePosition = true;
        }

        if (checkHandlePosition)
        {
            CheckSliderHandlePosition();
        }
    }

    void UpdateSliderValue()
    {
        if (increasing)
        {
            slider.value += Time.deltaTime * speed;

            if (slider.value >= 1.0f)
            {
                slider.value = 1.0f;
                increasing = false;
                if (!spawnOnce)
                {
                    RandomizePanelPosition();
                }
            }
        }
        else
        {
            slider.value -= Time.deltaTime * speed;

            if (slider.value <= 0.0f)
            {
                slider.value = 0.0f;
                increasing = true;
                if (!spawnOnce)
                {
                    RandomizePanelPosition();
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RandomizePanelPosition();
        }
    }

    void RandomizePanelPosition()
    {
        float randomX = Random.Range(minX, maxX);
        float fixedY = 0.5f;

        Vector2 newPosition = new Vector2(randomX, fixedY);
        panel.anchorMin = newPosition;
        panel.anchorMax = newPosition;
        panel.sizeDelta = new Vector2(panelWidth, panel.sizeDelta.y);
    }

    void CheckSliderHandlePosition()
    {
        Vector3 sliderHandlePosition = slider.handleRect.position;

        Vector3 localSliderHandlePosition = panel.InverseTransformPoint(sliderHandlePosition);

        if (localSliderHandlePosition.x >= 0 && localSliderHandlePosition.x <= panelWidth)
        {
            Debug.Log("Slider-Handle befindet sich im Panel!");
            Loader.SetActive(true);
            Invoke("Won", 1f);
            Enemy.SetTrigger("Death");
            Löwe.SetTrigger("Attack");
        }
        else
        {
            Debug.Log("Slider-Handle befindet sich nicht im Panel!");
            consecutiveMisses++;

            if (consecutiveMisses >= 1 && consecutiveMisses <= colorChangeImages.Count)
            {
                Debug.Log("Change Image Color!");
                ChangeImageColor(colorChangeImages[consecutiveMisses - 1], new Color(redValue, greenValue, blueValue));
                Enemy.SetTrigger("Attack");
                Löwe.SetTrigger("Hit");
            }

            if (consecutiveMisses >= 3)
            {
                Debug.Log("Restart ausgeführt!");
                Restart.SetActive(true);
                Invoke("Lost", 1f);
                Löwe.SetTrigger("Death");
                consecutiveMisses = 0; // Setzt Zählvariable zurück
            }
        }
        checkHandlePosition = false;
    }

    void Lost()
    {
        LOST.SetActive(true);
    }

    void Won()
    {
        WON.SetActive(true);
    }

    void ChangeImageColor(Image image, Color newColor)
    {
        if (image != null)
        {
            image.color = newColor;
        }
    }
}