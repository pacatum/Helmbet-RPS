using System.Collections.Generic;
using UnityEngine;
using Gesture = Base.Config.ChainTypes.RockPaperScissorsGesture;


public class ResultLogController : MonoBehaviour {

    [SerializeField] Transform thisPlayerStepsContainer;
    [SerializeField] Transform anotherPlayerStepsContainer;
    [SerializeField] GameButtonView stepPrefab;
    [SerializeField] RoundItemView roundIreItemViewPrefab;

    List<GameButtonView> steps = new List<GameButtonView>();
    List<RoundItemView> roundItemViews = new List<RoundItemView>();


    public void LogNewRound( int roundNumber, Gesture? anotherPlayerChoise, Gesture? thisPlayerChoise,
                             GameResult state ) {
        var logInstance = Instantiate( roundIreItemViewPrefab ).GetComponent<RoundItemView>();
        logInstance.UpdateItem( roundNumber, anotherPlayerChoise, thisPlayerChoise, state );
        logInstance.transform.SetParent( transform, false );

        var thisPlayerStep = Instantiate( stepPrefab );
        thisPlayerStep.SetStep( roundNumber, thisPlayerChoise );
        thisPlayerStep.transform.SetParent( thisPlayerStepsContainer, false );

        var anotherPlayerStep = Instantiate( stepPrefab );
        anotherPlayerStep.SetStep( roundNumber, anotherPlayerChoise );
        anotherPlayerStep.transform.SetParent( anotherPlayerStepsContainer, false );


        steps.Add( thisPlayerStep );
        steps.Add( anotherPlayerStep );
        roundItemViews.Add( logInstance );
    }

    public void ClearLog() {

        foreach ( var step in steps ) {
            Destroy( step.gameObject );
        }
        steps.Clear();

        foreach ( var roundItem in roundItemViews ) {
            Destroy( roundItem.gameObject );
        }

        roundItemViews.Clear();
    }

}