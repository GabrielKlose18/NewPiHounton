using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour{
    public Text textoDaResposta;
    private AnswerData answerData;
    
    private GameController gameController;
    // Start is called before the first frame update
    void Start(){
        gameController = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(AnswerData data){
        GetComponent<Image>().color = Color.white;
        answerData = data;
        textoDaResposta.text = answerData.textoResposta;
    }

    public void HandleClick(){
        if(answerData.estaCorreta)
            GetComponent<Image>().color = Color.green;
        else
            GetComponent<Image>().color = Color.red;
        gameController.AnswerButtonClicked(answerData.estaCorreta);
    }
}
