using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class UnitCommander : MonoBehaviour
{
    [SerializeField] UnitSelectionHandler selectionHandler = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();
    [SerializeField] public GameObject animationPrefab;

    private GameObject effect;

    private Camera mainCamera;
    private int counter = 0;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }


        playAnimation(hit.point);

        if(hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {

            if(target.hasAuthority)
            {
                // Possible grouping mechanics here
                TryMove(hit.point);
                return;
            }

            else { 
                TryTarget(target);
                return;
            }
        }

        TryMove(hit.point);
    }

    private void playAnimation(Vector3 point)
    {
        if(counter == 0) {
            effect = GameObject.Instantiate(animationPrefab, point, Quaternion.identity);
            effect.transform.localScale = new Vector3(5, 5, 5);
            var light_main = effect.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            var star_main = effect.transform.GetChild(1).GetComponent<ParticleSystem>().main;
            light_main.simulationSpeed = 2;
            star_main.simulationSpeed = 2;
            Invoke("DestroyEffect", 0.2f);
            counter++;
        }
    }

    private void TryMove(Vector3 point)
    {
        foreach(Unit unit in selectionHandler.getSelectedUnits())
        {
            unit.getUnitMovement().CmdMove(point);
        }
    }

    private void TryTarget(Targetable target)
    {
        foreach (Unit unit in selectionHandler.getSelectedUnits())
        {
            unit.getTargeter().CmdSetTarget(target.gameObject, "user");
        }
    }

    public void DestroyEffect()
    {
        Destroy(effect);
        counter = 0;
    }
}
