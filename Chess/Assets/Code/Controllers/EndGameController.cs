using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code{
    public class EndGameController:MonoBehaviour{
        public void EndGame(){
            SceneManager.LoadScene(0);
        }

        public void QuitGame(){
            Debug.Log("Quiting...");
            Application.Quit();
        }
        
    }
}