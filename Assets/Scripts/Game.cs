using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Game : MonoBehaviour
{
    private enum Mode
    {
        Build,
        Play
    }

    private Mode mode = Mode.Build;

    [Header("Build Mode")]
    [Tooltip("Current Gold. Set in Inspector to define starting gold.")]
    public int gold = 50;

    [Tooltip("Layer mask for highliter raycasting. Should include hte layer of the stage.")]
    public LayerMask stageLayerMask;

    [Tooltip("Reference to the transform of the Highlighter GameObject.")]
    public Transform highlighter;

    [Tooltip("Reference to the Tower Selling Panel")]
    public RectTransform towerSellingPanel;

    [Tooltip("Reference to the text component of the Refund Text in the Tower Selling Panel.")]
    public TextMeshProUGUI sellRefundText;

    [Tooltip("Reference to the text component of the current gold text in the bottom-left corner of the UI.")]
    public TextMeshProUGUI currentGoldText;

    [Tooltip("The color to apply to the selected build button.")]
    public Color selectedBuildButtonColor = new Color(.2f, .8f, .2f);

    //Mouse position at the last frame:
    private Vector3 lastMousePosition;

    //Curent gold the last time we checked:
    private int goldLastFrame;

    //True if te cursor is over the stage right now, flase if not:
    private bool cursorIsOverStage = false;

    //Reference to the Tower Prefab selected by the build button.
    private Tower towerPrefabToBuild = null;

    //Currently selected tower instance, if any:
    private Tower selectedTower = null;

    //Dictionary Storing Tower instances by their position
    private Dictionary<Vector3, Tower> towers = new Dictionary<Vector3, Tower>();
    
    

    void BuildTower()
    {
        //Instanitate the tower at the given location and place it in the dictionary:
        towers[position] = Instantiate(towerPrefabToBuild, position, Quaternion.identity);
        //Decrese player gold:
        gold -= towerPrefabToBuild.goldCost;
        //Update the path through the maze:
        UpdateEnemyPath();
    }

    void OnStageClicked()
    {
        //if a build button is selected:
        if (towerPrefabToBuild != null)
        {
            //If there is no tower in that slot and we have enough gold to build the selected tower:
            if (!towers.ContainsKey(highlighter.position) && gold >= towerPrefabToBuild.goldCost)
            {
                BuildTower(towerPrefabToBuild, highlighter.position);
            }
        }

        //if no build button is selected:
        else
        {
            //check if a tower is at the current highlighter position:
            if (towers.ContainsKey(highlighter.position))
            {
                //Set the selected tower to this one:
                selectedTower = towers[highlighter.position];

                //Update the refund text:
                sellRefundText.text = "for" + Mathf.CeilToInt(selectedTower.goldCost * selectedTower.refundFactor) + "gold";

                // Make sure the sell tower UI panel is active so it shows:
                towerSellingPanel.gameObject.SetActive(true);
            }
        }
    }

    public void OnBuildButtonClicked(Tower associatedTower)
    {
        //set the prefab to build:
        towerPrefabToBuild = associatedTower;

        //clear the selected tower (if any)
        DeselectTower();
    }
    public void SetSelectedBuildButton()
    {
        //if we havae a build button already, make sure its color is reset:
        if (selectedBuildButtonImage != null)
        {
            selectedBuildButtonImage.color = Color.white;
            //Keep a reference to the button that was clicked:
            selectedBuildButtonImage = clickedButtonImage;
            //Set the color of the clicked button:
            clickedButtonImage.color = selectedBuildButtonColor;
        }
    }


    void PositionSellPanel()
    {
        //If there is a selected Tower:
        if(selectedTower != null)
        {
            //Convert tower world position, moved by 8 units, to screen space:
            var screenPosition = Camera.main.WorldToScreenPoint(selectedTower.transform.position + Vector3.forward * 8);
            //Apply the position ot the tower selling panel:
            towerSellingPanel.position = screenPosition;
        }
    }


    void UpdateCurrentGold()
    {
        //If the gold has changed since last frame, update the text to match:
        if(gold != goldLastFrame)
        {
            currentGoldText.text = gold + " gold";

            ///Keep track of the gold value in each frame:
            goldLastFrame = gold;
        }
    }


    public void DeselectTower()
    {
        //Null selected tower and hide the sell tower panel:
        selectedTower = null;
        towerSellingPanel.gameObject.SetActive(false);
    }

    void DeselectBuildButton()
    {
        //Nul the tower prefab to build, if there is one:
        towerPrefabToBuild = null;

        //Reset the color of the selected button, if there is one:
        if(selectedBuildButtonImage != null)
        {
            selectedBuildButtonImage.color = Color.white;
            selectedBuildButtonImage = null;
        }
    }

    void PositionHighlighter()
    {
        //If the mouse position is different than last frame
        if (Input.mousePosition != lastMousePosition)
        {
            //Get a ray at the mouse position, shooting out of the camera:
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit; //Information on what was hit will be stored here.

            //Cast the ray and check if it hit anything, using our layer mask:
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, stageLayerMask.value))
            {
                //If it did hit something, use hit.point to get the location it hit:
                Vector3 point = hit.point;

                //Round the X and Z values to multiples of 10:
                point.x = Mathf.Round(hit.point.x * .1f) * 10;
                point.z = Mathf.Round(hit.point.z * .1f) * 10;

                //Clamp Z between -80 and 80 to prevent sticking over the edge of the stage:
                point.z = Mathf.Clamp(point.z, -80, 80);

                //Ensure that Y is always .2, half the height of the highlighter:
                point.y = .2f;

                //Make sure the highlighter is active (visible) and set its position:
                highlighter.position = point;
                highlighter.gameObject.SetActive(true);
                cursorIsOverStage = true;
            }
            else //If the ray didn't hit anything,
            {
                //... mark cursorIsOverStage as false:
                cursorIsOverStage = false;

                ///Deactivate the highlighhter GameObject so it no longer shows:
                highlighter.gameObject.SetActive(false);
            }
        }
        //Make sure we keep track of the mouse position this frame:

        lastMousePosition = Input.mousePosition;
    }

    
    public void OnSellTowerButtonClicked()
    {
        //If there is a selected tower, sell it:
        if(selectedTower != null)
        {
            SellTower(selectedTower);
        }
    }


    void SellTower(Tower tower)
    {
        //Since it's not going to exist in a bit, deselect the tower:
        DeselectTower();
        //Refund the player:
        gold += Mathf.CeilToInt(tower.goldCost * tower.refundFactor);

        //Remove the tower from the dictionary using its position:
        towers.Remove(tower.transform.position);

        //Destroy the tower GameObject:
        Destroy(tower.gameObject);

        //Refresh pathfinding
        UpdateEnemyPath();
    }


    void BuildModeLogic()
    {
        PositionHighlighter();
        PositionSellPanel();
        UpdateCurrentGold();

        //If the left mouse btton is clicked while the cursor is over the stage:
        if (cursorIsOverStage && Input.GetMouseButtonDown(0))
        {
            OnStageClicked();
        }

        //If escape is pressed:
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectTower();
            DeselectBuildButton();
        }
    }

    void UpdateEnemyPath()
    {

    }
    void Update()
    {
        //Run build mode logic if we're in build mode:
        if (mode == Mode.Build)
        {
            BuildModeLogic();

        }
    }
}
