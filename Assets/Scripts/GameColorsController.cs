using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameColorsController : MonoBehaviour {
    private PlayerAndTrampolines ChangeColor;


    private SpriteRenderer thisBckgr;
    
    public Color bckgrPink2, bckgrRed2, bckgrBlue2, bckgrGreen2, bckgrOrange2,
                 bckgrPink, bckgrRed, bckgrBlue, bckgrGreen, bckgrOrange;
    int colorInRow;

    private void Awake()
    {
        thisBckgr = GetComponentInChildren<SpriteRenderer>();
        ChangeColor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAndTrampolines>();
        //print(colorInRow);
    }

    public void ChangeColors()
    {
        switch (colorInRow++)
        {
            case 0:
                ChangeColor.ChangeColors(colorInRow - 1, bckgrPink, thisBckgr.color);
                break;
            case 1:
                ChangeColor.ChangeColors(colorInRow - 1, bckgrRed, thisBckgr.color);
                break;
            case 2:
                ChangeColor.ChangeColors(colorInRow - 1, bckgrBlue, thisBckgr.color);
                break;
            case 3:
                ChangeColor.ChangeColors(colorInRow - 1, bckgrGreen, thisBckgr.color);
                break;
            case 4:
                ChangeColor.ChangeColors(colorInRow - 1, bckgrOrange, thisBckgr.color);
                break;
            case 5:
                ChangeColor.ChangeColors(colorInRow - 1, bckgrPink2, thisBckgr.color);
                break;
            case 6:
                ChangeColor.ChangeColors(colorInRow - 1, bckgrRed2, thisBckgr.color);
                break;
            case 7:
                ChangeColor.ChangeColors(colorInRow - 1, bckgrBlue2, thisBckgr.color);
                break;
            case 8:
                ChangeColor.ChangeColors(colorInRow - 1, bckgrGreen2, thisBckgr.color);
                break;
            case 9:
                ChangeColor.ChangeColors(colorInRow - 1, bckgrOrange2, thisBckgr.color);
                break;

            default:
                Debug.LogError("Default case called in color switch!");    
            break;
        }
        //print("case: "+colorInRow);
        if (colorInRow > 9) colorInRow = 0;
    }
    
    // Use this for initialization
    void Start () {
        colorInRow = 0;
	}
}
