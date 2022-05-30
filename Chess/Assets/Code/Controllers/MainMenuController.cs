using Code;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour{
    public static bool isGameAgainstComputer;
    public static float selectedGameTimer = 120;
    public static TeamType humanPlayerTeamType = TeamType.White;
    public TMPro.TMP_Dropdown dropdown;
    public TMPro.TMP_Dropdown humanPlayerTeamTypeDropdown;

    public void StartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void setGameType(bool gameType){
        isGameAgainstComputer = gameType;
    }

    public void setHumanPlayerTeamType(){
        if (humanPlayerTeamTypeDropdown.value == 0){
            humanPlayerTeamType = TeamType.White;
        }
        else{
            humanPlayerTeamType = TeamType.Black;
        }
    }

    public void setPlayersTimers(){
        if (dropdown.value == 0)
            selectedGameTimer = 120;
        if (dropdown.value == 1)
            selectedGameTimer = 5 * 60;
        if (dropdown.value == 2)
            selectedGameTimer = 60;
        if (dropdown.value == 3)
            selectedGameTimer = 60 / 2f;
    }

    public void Log(){
        Debug.Log("xd");
        Application.Quit();
    }
}