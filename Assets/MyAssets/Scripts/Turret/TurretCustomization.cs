using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using System.Linq;

public class TurretCustomization : NetworkBehaviour
{
    [SerializeField] private Color albedoColorPlayer;
    [SerializeField] private Color emissionColorPlayer;
    [SerializeField] private Color albedoColorEnemy;
    [SerializeField] private Color emissionColorEnemy;

    [SerializeField] private GameObject gunBase = null;

    [SerializeField] private List<Material> turretAccents;
    [SerializeField] private List<GameObject> listOfChildren;

    void Start()
    {
        List<Material> turretMaterials;

        GetChildRecursive(gunBase);

        listOfChildren.Add(gunBase);

        foreach (GameObject g in listOfChildren)
        {
            if(!g.TryGetComponent<Renderer>(out Renderer rend)) { continue; }

            turretMaterials = rend.materials.ToList();

            if(turretMaterials.Count == 2)
            {
                turretAccents.Add(turretMaterials[1]);
            }

            /*

            foreach(Material mat in turretMaterials)
            {
                if (mat.name == "Main color 02")
                {
                    turretAccents.Add(mat);
                }
            }
            */
        }
    }

    private void GetChildRecursive(GameObject obj)
    {
        if (null == obj)
            return;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
                continue;
            //child.gameobject contains the current child you can do whatever you want like add it to an array
            listOfChildren.Add(child.gameObject);
            GetChildRecursive(child.gameObject);
        }
    }

    void Update()
    {
        updateTurretColors();
    }

    private void updateTurretColors()
    {
        if (hasAuthority)
        {
            foreach (Material turretAccent in turretAccents)
            {
                turretAccent.color = albedoColorPlayer;
                turretAccent.SetColor("_EmissionColor", emissionColorPlayer);
            }
        }

        else
        {
            foreach (Material turretAccent in turretAccents)
            {
                turretAccent.color = albedoColorEnemy;
                turretAccent.SetColor("_EmissionColor", emissionColorEnemy);
            }
        }
    }
}
