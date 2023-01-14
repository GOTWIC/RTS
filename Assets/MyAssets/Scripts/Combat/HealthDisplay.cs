using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] GameObject healthBarParent;
    [SerializeField] Image healthBarImage = null;
    [SerializeField] bool alwaysShown = false;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        if (alwaysShown)
            healthBarParent.SetActive(true);
    }

    private void Update()
    {
        healthBarParent.transform.LookAt(mainCamera.transform);
    }

    private void Awake()
    {
        health.ClientOnHealthUpdated += handleHealthUpdated;
    }

    private void OnDestroy()
    {
        health.ClientOnHealthUpdated -= handleHealthUpdated;
    }

    private void OnMouseEnter()
    {
        if (alwaysShown) { return; }
        healthBarParent.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (alwaysShown) { return; }
        healthBarParent.SetActive(false);
    }

    private void handleHealthUpdated(int currentHealth, int maxHealth)
    {
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }
}
