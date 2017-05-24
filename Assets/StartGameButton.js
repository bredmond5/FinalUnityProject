#pragma strict
import UnityEngine.SceneManagement;
var loadmenu : boolean = false;


function StartGame(){
    if(!loadmenu)SceneManager.LoadScene(1, LoadSceneMode.Single);
    else SceneManager.LoadScene(0, LoadSceneMode.Single);
}