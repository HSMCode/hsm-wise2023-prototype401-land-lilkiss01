using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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

    public AudioSource Aud_Lion;
    public AudioSource Aud_Enemy;

    [Range(0, 1)]
    public float redValue = 1.0f;
    [Range(0, 1)]
    public float greenValue = 1.0f;
    [Range(0, 1)]
    public float blueValue = 1.0f;

    public Camera mainCamera;
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;

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
            Aud_Lion.Play();
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
                Aud_Enemy.Play();
            }

            if (consecutiveMisses >= 3)
            {
                Debug.Log("Restart ausgeführt!");
                Restart.SetActive(true);
                Invoke("Lost", 1f);
                Löwe.SetTrigger("Death");
                consecutiveMisses = 0;
            }

            StartCoroutine(ShakeCamera());
        }
        checkHandlePosition = false;
    }

    IEnumerator ShakeCamera()
{
    float elapsed = 0.0f;
    Vector3 originalCamPos = mainCamera.transform.position;

    while (elapsed < shakeDuration)
    {
        float x = Random.Range(-1f, 1f) * shakeMagnitude;
        float y = Random.Range(-1f, 1f) * shakeMagnitude;

        mainCamera.transform.position = new Vector3(originalCamPos.x + x, originalCamPos.y + y, originalCamPos.z);

        elapsed += Time.deltaTime;

        yield return null;
    }

    mainCamera.transform.position = originalCamPos;
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