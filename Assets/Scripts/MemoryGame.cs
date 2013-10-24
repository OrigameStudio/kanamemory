﻿
using UnityEngine;
using System.Collections;


public class MemoryGame : MonoBehaviour{

	public static string TOTAL_GAMES	= "TotalGames";
	public static string GAMES_WON		= "GamesWon";
	public static string GAMES_LOST		= "GamesLost";

	public GameLanguageData		language;
	public GameStatus			status		= GameStatus.Ready;
	public GameType				type		= GameType.GameType1;
	public GameDifficulty		difficulty	= GameDifficulty.MEDIUM;
	public BoardSize			boardSize	= BoardSize.REGULAR;
	public bool					hint		= false;
	public MemoryGameStatsCards	cards;
	public int					games;
	public int					gamesWon;
	public int					gamesLost;
	public MemoryGameStatsTime	time;

	private static MemoryGame instance;
	public static MemoryGame GetInstance(){

		if(MemoryGame.instance == null){

			MemoryGame.instance = (MemoryGame)GameObject.FindObjectOfType( typeof(MemoryGame) );
		}

		return(MemoryGame.instance);
	}

	void Awake(){

		MemoryGame.GetInstance();

		if(MemoryGame.instance != null && MemoryGame.instance != this){

			GameObject.Destroy(this.gameObject);

		}else{

			this.language = ( GameAvailableLanguages.FindInstance() ).GetGameLanguage().data;

			this.games		= PlayerPrefs.GetInt(TOTAL_GAMES);
			this.gamesWon	= PlayerPrefs.GetInt(GAMES_WON);
			this.gamesLost	= PlayerPrefs.GetInt(GAMES_LOST);

			GameObject.DontDestroyOnLoad(this.gameObject);
		}
	}

	public void StartGame(int pairs, int seconds){

		this.time.Reset(seconds);
		this.cards.Reset(pairs);

		this.status = GameStatus.Playing;
	}

	public void UpdateTimeLeft(float now){

		if(this.status != GameStatus.Playing){

			return;
		}

		this.time.left = this.time.total - Mathf.FloorToInt(now - this.time.start);

		if(this.time.left <= 0 && this.PairsLeft() > 0){

			this.Fail();
		}
	}

	public void ExtendTimeLeft(float time){

		this.time.total += (int)time;
	}

	public int PairsLeft(){

		return(this.cards.total - this.cards.matches);
	}

	public int GetSecondsLeft(){

		return(this.time.left > 0 ? this.time.left : 0);
	}

	public bool SelectCards(Card card1, Card card2){

		bool success;

		if(this.status != GameStatus.Playing){

			return(true);
		}

		success = card1.Matches(card2);

		if(success){

			this.cards.matches++;

			if(this.PairsLeft() == 0){

				this.Success();
			}

		}else{

			this.cards.failures++;
		}

		return(success);
	}

	private void Fail(){

		this.status = GameStatus.Failure;

		this.games++;
		PlayerPrefs.SetInt(TOTAL_GAMES, this.games);

		this.gamesLost++;
		PlayerPrefs.SetInt(GAMES_LOST, this.gamesLost);
	}


	private void Success(){

		this.status = GameStatus.Success;

		this.games++;
		PlayerPrefs.SetInt(TOTAL_GAMES, this.games);

		this.gamesWon++;
		PlayerPrefs.SetInt(GAMES_WON, this.gamesWon);
	}
}
