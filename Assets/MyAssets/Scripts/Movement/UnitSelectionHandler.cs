using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask = new LayerMask();
    [SerializeField] private RectTransform unitSelectionArea = null;

    private Vector2 startPos;
    
    private Camera mainCamera;
    private PlayerScript player;
    
    private List<Unit> selectedUnits = new List<Unit>();

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if(player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<PlayerScript>();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            startSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            clearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            updateSelectionArea();
        }
    }

    private void startSelectionArea()
    {
        //Deselect all units unless shift is pressed

        if(!Keyboard.current.leftShiftKey.isPressed)
            removeSelections();

        unitSelectionArea.gameObject.SetActive(true);

        startPos = Mouse.current.position.ReadValue();

        updateSelectionArea();

    }

    private void updateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float width = mousePosition.x - startPos.x;
        float height = mousePosition.y - startPos.y;


        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        unitSelectionArea.anchoredPosition = startPos + new Vector2(width / 2, height / 2);
    }

    private void clearSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(false);

        // Single Select
        if(unitSelectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

            if (!unit.hasAuthority) { return; }


            // Shaky Code
            if (!selectedUnits.Contains(unit))
            {
                selectedUnits.Add(unit);
                unit.select();
            }
            else
            {
                selectedUnits.Remove(unit);
                unit.deselect();
            }

            /*
            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.select();
            }
            */

            return;
        }

        // MultiSelect

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

        foreach(Unit unit in player.getUnits())
        {
            if(selectedUnits.Contains(unit)) { continue; }

            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

            if(screenPosition.x >= min.x && screenPosition.x <= max.x && screenPosition.y >= min.y && screenPosition.y <= max.y)
            {
                selectedUnits.Add(unit);
                unit.select();
            }
        }


    }

    private void removeSelections()
    {
        foreach (Unit selectedUnit in selectedUnits)
        {
            selectedUnit.deselect();
        }

        selectedUnits.Clear();
    }

    public List<Unit> getSelectedUnits()
    {
        return selectedUnits;
    }
}
