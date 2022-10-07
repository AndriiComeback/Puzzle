using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Score
{
	[SerializeField] private int m_currentScore;
	[SerializeField] private int m_levelScoreBonus;
	[SerializeField] private int m_turnScoreBonus;
	public int CurrentScore { set { m_currentScore = value; } get { return m_currentScore; } }
	public void AddLevelBonus() {
		CurrentScore += m_levelScoreBonus;
	}
	public void AddTurnBonus() {
		CurrentScore += m_turnScoreBonus;
	}

}
