using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO.Ports;
using System.Reflection;

public class GameController : MonoBehaviour{
    SerialPort arduinoPort = new SerialPort("/dev/tty.usbserial-1410", 9600); // ou 1420
    public Text textoPergunta;
    public Text textoPontosPlayer1;
    public Text textoPontosPlayer2;
    public Text textoTimer;
    public Text highScoreText;
    
    public SimpleObjectPool answerButtonObjectPool;
    public Transform answerButtonParent;
    public GameObject painelDePerguntas;
    public GameObject painelFimRodada;
    public GameObject UIPanel;
    public GameObject ButtonResposta1;
    public GameObject ButtonResposta2;
    public GameObject ButtonResposta3;
    public GameObject ButtonResposta4;

    private DataController dataController;
    private RoundData rodadaAtual;
    private QuestionData[] questionPool;

    private bool rodadaAtiva;
    private float tempoRestante;
    private int questionIndex;
    private int player1Score;
    private int player2Score;
    private int playerSelected;
    private int answerSeleted;

    private int respostaCorreta;

    List<int> usedValues = new List<int>();
    List<GameObject> answerButtonGameObjects = new List<GameObject>();
    // Start is called before the first frame update
    
    void Start(){
        try{
            arduinoPort.BaudRate = 9600;
            arduinoPort.Open();
            arduinoPort.ReadTimeout = 1;

        }catch(System.Exception){
            Debug.Log("NÃo foi possivel conectar");
        }
        // foreach (string port in SerialPort.GetPortNames())
        // {
        //     Debug.Log(port);
        // }

        dataController = FindObjectOfType<DataController>();
        rodadaAtual = dataController.GetCurrentRoundData();
        questionPool = rodadaAtual.perguntas;
        tempoRestante = rodadaAtual.limiteDeTempo;
        // Debug.Log(questionPool.Length);
        UpdateTimer();

        player1Score = 0;
        player2Score = 0;
        questionIndex = 0;
        rodadaAtiva = true;
        ShowQuestion();
    }

    // Update is called once per frame
    void Update(){        
        if(rodadaAtiva){
            tempoRestante -= Time.deltaTime;
            UpdateTimer();
            if(tempoRestante <= 0){
                EndRound();
            }
        }
        arduinoPort.DiscardOutBuffer();
        arduinoPort.DiscardInBuffer();
    }

    private void UpdateTimer(){
        textoTimer.text = "Timer: " + Mathf.Round(tempoRestante).ToString();
    }

    public async void ShowQuestion(){
        RemoveAnswerButtons();
        int random = UnityEngine.Random.Range(0,questionPool.Length);
        while(usedValues.Contains(random)){
            random = UnityEngine.Random.Range(0,questionPool.Length);
        }

        QuestionData questionData = questionPool[random];
        usedValues.Add(random);
        textoPergunta.text = questionData.textoDaPergunta;

        for (int i = 0; i < questionData.respostas.Length; i++){
            // Debug.Log(questionData.respostas[i]);
            if(questionData.respostas[i].estaCorreta){
                respostaCorreta = i;
            }
            switch (i){
                case 0:
                    ButtonResposta1.GetComponentInChildren<Text>().text = questionData.respostas[i].textoResposta;
                    break;
                case 1:
                    ButtonResposta2.GetComponentInChildren<Text>().text = questionData.respostas[i].textoResposta;
                    break;
                case 2:
                    ButtonResposta3.GetComponentInChildren<Text>().text = questionData.respostas[i].textoResposta;
                    break;
                case 3:
                    ButtonResposta4.GetComponentInChildren<Text>().text = questionData.respostas[i].textoResposta;
                    break;
                default:
                    Debug.Log("Erro");
                    break;
            }
            
            // GameObject answerButtongameObject = answerButtonObjectPool.GetObject();
            
            // answerButtongameObject.transform.SetParent(answerButtonParent);

            // answerButtonGameObjects.Add(answerButtongameObject);

            // AnswerButton answerButton = answerButtongameObject.GetComponent<AnswerButton>();
            // answerButton.Setup(questionData.respostas[i]);
        }
        await Invoke("whichPlayer", 0.5f);
        tempoRestante = rodadaAtual.limiteDeTempo;// sempre q trocar as perguntas, resetar o tempo
        if(playerSelected == 1){
            Debug.Log("Player1 Joga");
            // Invoke("selectRespostaPlayer1", 0.5f);
        }else if(playerSelected == 2){
            Debug.Log("Player2 Joga");
            // Invoke("selectRespostaPlayer2", 0.5f);
        }
        
    }

    private void RemoveAnswerButtons(){
        while(answerButtonGameObjects.Count > 0){
            answerButtonObjectPool.ReturnObject(answerButtonGameObjects[0]);
            answerButtonGameObjects.RemoveAt(0);
        }
    }

    public void AnswerSelected(){
        if(answerSeleted == respostaCorreta){
            if(playerSelected == 1){
                player1Score += rodadaAtual.pontosPorAcerto;
                textoPontosPlayer1.text = "Player1 Score: " + player1Score.ToString();
            }else{
                player2Score += rodadaAtual.pontosPorAcerto;
                textoPontosPlayer2.text = "Player2 Score: " + player1Score.ToString();
            }
            
        }

        if(questionPool.Length > questionIndex + 1){
            if(questionIndex == 2)// acabar a partida apos 3 perguntas
                Invoke("EndRound", 1.5f);
                // EndRound();
            questionIndex ++;
            Invoke("ShowQuestion", 1.5f);
            // ShowQuestion();
        }else{
            Invoke("EndRound", 1.5f);
            // EndRound();
        }
    }

    public void EndRound(){
        arduinoPort.Close();
        rodadaAtiva = false;

        dataController.EnviarNovoHighScore(player1Score);
        highScoreText.text = "High Score: "+dataController.GetHighScore().ToString();
        painelDePerguntas.SetActive(false);
        UIPanel.SetActive(false);
        painelFimRodada.SetActive(true);
    }

    public void ReturnToMenu(){
        SceneManager.LoadScene("Menu");
    }

    public void whichPlayer(){
        if(!arduinoPort.IsOpen)
            arduinoPort.Open();
        int player = 0;
        while(player == 0){
            try{
                int playerSerial = int.Parse(arduinoPort.ReadLine());
                if(playerSerial == 1){
                    Debug.Log("Player 1");
                    player = 1;
                }else if(playerSerial == 2){
                    Debug.Log("Player 2");
                    player = 2;
                }
            }catch(System.Exception){}
        }
        playerSelected = player;
        
    }

    public void selectRespostaPlayer1(){
        if(!arduinoPort.IsOpen)
            arduinoPort.Open();
        int serial = 0;
        while(serial == 0){
            try{
                int respostaSerial = int.Parse(arduinoPort.ReadLine());
                if(respostaSerial == 1){//3
                    serial = 0;
                    ButtonResposta1.GetComponent<Image>().color = Color.green;
                }else if(respostaSerial == 2){//4
                    serial = 1;
                    ButtonResposta2.GetComponent<Image>().color = Color.green;
                }else if(respostaSerial == 5){
                    serial = 2;
                }else if(respostaSerial == 6){
                    serial = 3;
                }
            }catch(System.Exception){}
        }
        answerSeleted = serial;
        AnswerSelected();

    }
    public void selectRespostaPlayer2(){
        if(!arduinoPort.IsOpen)
            arduinoPort.Open();
        int serial = 0;
        while(serial == 0){
            try{
                int respostaSerial = int.Parse(arduinoPort.ReadLine());
                if(respostaSerial == 7){
                    serial = 0;
                }else if(respostaSerial == 8){
                    serial = 1;
                }else if(respostaSerial == 9){
                    serial = 2;
                }else if(respostaSerial == 10){
                    serial = 3;
                }
            }catch(System.Exception){}
        }
        answerSeleted = serial;
        AnswerSelected();

    }
}
