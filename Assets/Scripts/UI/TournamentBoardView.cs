﻿using System.Collections;
using System.Collections.Generic;
using Base.Data.Tournaments;
using UnityEngine;
using UnityEngine.UI;

public class TournamentBoardView : MonoBehaviour {
	
	[SerializeField] Text tournamentTitleText;
	

	public string MaxPlayersAmount {
		get { return tournamentTitleText ? string.Empty : tournamentTitleText.text ; }
		set {

			tournamentTitleText.text = value + " Players";
		}
	}

}
