using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuController : MonoBehaviour{
    private DataController data;
    
    // Start is called before the first frame update
    void Start(){
        data = FindObjectOfType<DataController>(); // se tiver mais de um DataController em cena, procurar outra forma de buscar
    }

    public void StartGame(int round){
        data.SetRoundData(round);
        SceneManager.LoadScene("Game");
    }
}
