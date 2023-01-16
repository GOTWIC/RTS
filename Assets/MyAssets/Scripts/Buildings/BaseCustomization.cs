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
    [SerializeField] private GameObject mapIconOBJ = null;

    [SerializeField] private Color albedoColorPlayer;
    [SerializeField] private Color emissionColorPlayer;
    [SerializeField] private Color mapIconColorPlayer;
    [SerializeField] private Color albedoColorEnemy;
    [SerializeField] private Color emissionColorEnemy;
    [SerializeField] private Color mapIconColorEnemy;


    private List<Material> ringMaterials;
    private Material ringLight;
    private Material ringDots;

    private List<Material> mapIconMaterials;
    private Material mapIcon;


    private void Start()
    {
        ringMaterials = haloRing.GetComponent<Renderer>().materials.ToList();
        ringDots = ringMaterials[2];
        ringLight = ringMaterials[3];

        mapIconMaterials = mapIconOBJ.GetComponent<Renderer>().materials.ToList();
        mapIcon = mapIconMaterials[0];
    }


    void Update()
    {
        // For now, these are set to update. Later, they will change when there is a change in ownership,
        // rather than every frame
        if(baseName != null) { updateBaseName(); }

        if(haloRing!= null) { updateHaloRingColors(); }

        if(mapIconOBJ != null) { updateMinimapIcon(); }
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

    private void updateMinimapIcon()
    {
        if (hasAuthority)
        {
            mapIcon.color = albedoColorPlayer;
        }

        else
        {
            mapIcon.color = albedoColorEnemy;
        }
    }
}
