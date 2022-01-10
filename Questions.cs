using UnityEngine;


[CreateAssetMenu(fileName = "Questions", menuName = "Question Pack")]
public class Questions : ScriptableObject
{
    
    [System.Serializable]
    public struct Question 
    {
        public string name;
        public string FirstQuestion;
        public bool FirtsAnswer;
        public int FirstPoint;
        public string SecondQuestion;
        public bool SecondAnswer;
        public int SecondPoint;
        public string ThirdQuestion;
        public bool ThirdAnswer;
        public int ThirdPoint;
    }
    [System.Serializable]
    public struct LastQuestion
    {
        public string name;
        public string Question;
        public bool Answer;
        public int Point;
    }
    public Question[] question;
    public LastQuestion lastQuestion;
}
