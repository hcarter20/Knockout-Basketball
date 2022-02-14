using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialManager : MonoBehaviour
{
    public GameObject[] popUps;
    public int popUpIndex = 0;

    void Update() {
        for (int i = 0; i < popUps.Length; i++) {
            if(i == popUpIndex){
                popUps[popUpIndex].SetActive(true);
            } else {
                popUps[popUpIndex].SetActive(false);
            }
        }

        if(popUpIndex == 0){
            if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)){
                popUpIndex+=1;
            }
        } else if(popUpIndex == 1){
            if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)){
                popUpIndex+=1;
            }
        } else if(popUpIndex == 2){
            if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)){
                popUpIndex+=1;
            }
        } else if(popUpIndex == 3){
            if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)){
                popUpIndex+=1;
            }
        } else if(popUpIndex == 4){
            if(Input.GetKeyDown(KeyCode.Space)){
                popUpIndex+=1;
            }
        } else if(popUpIndex == 5){
            //do nothing?
        }
    }
}
