using UnityEngine;
using System.Collections;

public class gameStateManager : MonoBehaviour {

	// properties
	int lifes = 5;
	int coins = 0;
	int score = 0;
	int stars = 0;
	int bannerScore = 0;
	int currentLevel = 0;
	int nextLevel = 3;
	
	// level management
	public void setCurrentLevel(int i) {
		currentLevel = i;
	}
	public int getCurrentLevel() {
		return currentLevel;
	}
	public void setNextLevel(int i) {
		nextLevel = i;
	}
	public int getNextLevel() {
		return nextLevel;
	}

	// setter methods
	public void changeLifes(int i) {
		lifes += i;
	}
	public void addCoins(int i) {
		coins += i;
	}
	public void addScore(int i) {
		score += i;
	}
	public void addStars(int i) {
		stars += i;
	}
	public void addBannerScore(int i) {
		bannerScore += i;
	}

	// getter methods
	public int getLifes() {
		return lifes;
	}
	public int getCoins() {
		return coins;
	}
	public int getScore() {
		return score;
	}
	public int getStars() {
		return stars;
	}
	public int getBannerScore() {
		return bannerScore;
	}

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
