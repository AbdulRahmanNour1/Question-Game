using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answer : MonoBehaviour
{
    public bool isCorrect = false;
    public QuizManager quizManager;

    public void AnswerIf()
    {
        
        // if (QuizManager.Instance == null)
        // {
        //     Debug.LogError("QuizManager instance is missing!");
        //     return;
        // }

        if (isCorrect)
        {
            Debug.Log("Correct");
            QuizManager.Instance.PlayFeedbackAudio(true);
            quizManager.Correct();
        }
        else
        {
            Debug.Log("Wrong");
            QuizManager.Instance.PlayFeedbackAudio(false);
        }
    }
}
