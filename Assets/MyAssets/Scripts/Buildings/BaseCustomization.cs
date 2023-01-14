using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;
using System.Linq;

public class BaseCustomization : NetworkBehaviour
{
    [SerializeField] private TMP_Text baseName = null;
    [SerializeField] private GameObject haloRing = null;

    [SerializeField] private Color albedoColorPlayer;
    [SerializeField] private Color emissionColorPlayer;
    [SerializeField] private Color albedoColorEnemy;
    [SerializeField] private Color emissionColorEnemy;


    private List<Material> ringMaterials;
    private Material ringLight;
    private Material ringDots;


    private void Start()
    {
        ringMaterials = haloRing.GetComponent<Renderer>().materials.ToList();
        ringDots = ringMaterials[2];
        ringLight = ringMaterials[3];
    }


    void Update()
    {
        if(baseName != null) { updateBaseName(); }

        if(haloRing!= null) { updateHaloRingColors(); }
    }

    private void updateBaseName()
    {
        if (hasAuthority)
        {
            baseName.text = "Home";
            baseName.color = new Color(0, 150, 250);
        }

        else
        {
            baseName.text = "Enemy";
            baseName.color = new Color(255, 0, 0);
        }
    }

    private void updateHaloRingColors()
    {
        if (hasAuthority)
        {
            ringLight.color = albedoColorPlayer;
            ringLight.SetColor("_EmissionColor", emissionColorPlayer);
            ringDots.color = albedoColorPlayer;
            ringDots.SetColor("_EmissionColor", emissionColorPlayer);
        }

        else
        {
            ringLight.color = albedoColorEnemy;
            ringLight.SetColor("_EmissionColor", emissionColorEnemy);
            ringDots.color = albedoColorEnemy;
            ringDots.SetColor("_EmissionColor", emissionColorEnemy);
        }
    }
}
