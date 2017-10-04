using System;
using Base;
using Base.Data;
using Base.Data.Assets;
using Base.Data.Operations;
using Base.Data.Tournaments;
using Base.Transactions.Tournaments;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class LeaveTournamentConfirmation : BaseCanvasView {

    public event Action OnOperationSuccess;

    [SerializeField] Text gameTitleText;
    [SerializeField] Text creatorNameText;
    [SerializeField] Text numberOfPlayersText;
    [SerializeField] Text winsAmountText;
    [SerializeField] Text registrationDeadlineText;
    [SerializeField] Text buyinText;
    [SerializeField] Text feeText;
    [SerializeField] Button leaveButton;
    [SerializeField] Button cancelButton;
    [SerializeField] ScreenLoader loader;
    [SerializeField] MessagePopupView messagePopupView;

    LeaveFromTournamentData currentData;
    long currentFee;
    TournamentLeaveOperationData currentOperation;
    AssetData currentAccountBalanceObject;


    public override void Awake() {
        base.Awake();
        cancelButton.onClick.AddListener( Hide );
        leaveButton.onClick.AddListener( LeaveTournament );
    }

    public void SetUp( TournamentObject tournament ) {
        ApiManager.Instance.Database
            .GetAccountBalances( AuthorizationManager.Instance.UserData.FullAccount.Account.Id.Id, Array.ConvertAll( AuthorizationManager.Instance.UserData.FullAccount.Balances, balance => balance.AssetType.Id ) )
            .Then( accountBalances => {
                var data = new LeaveFromTournamentData();
                data.tournament = tournament;
                data.account = AuthorizationManager.Instance.UserData.FullAccount.Account.Id;
                currentData = data;
                AssetData asset = null;
                foreach ( var balance in accountBalances ) {
                    if ( balance.Asset.Equals( currentData.tournament.Options.BuyIn.Asset ) ) {
                        asset = balance;
                    }
                }

                TournamentTransactionService.GenerateLeaveFromTournamentOperation( currentData )
                    .Then( operation => {
                        var feeAsset = SpaceTypeId.CreateOne( SpaceType.Asset );

                        ApiManager.Instance.Database.GetRequiredFee( operation, feeAsset.Id )
                            .Then( feeResult => {
                                Repository.GetInPromise<AssetObject>( feeResult.Asset )
                                    .Then( assetData => {
                                        buyinText.text = tournament.Options.BuyIn.Amount / Math.Pow( 10, assetData.Precision ) + assetData.Symbol;
                                        feeText.text = feeResult.Amount / Math.Pow( 10, assetData.Precision ) + assetData.Symbol;
                                        currentFee = feeResult.Amount;
                                        currentOperation = operation;
                                        currentAccountBalanceObject = asset;

                                        ApiManager.Instance.Database.GetAccount( tournament.Creator.Id )
                                            .Then( creator => {

                                                gameTitleText.text = "ROCK, PAPER, SCISSORS";
                                                creatorNameText.text = Utils.GetFormatedString( creator.Name );
                                                numberOfPlayersText.text = data.tournament.Options.NumberOfPlayers.ToString();
                                                winsAmountText.text = data.tournament.Options.NumberOfWins.ToString();
                                                registrationDeadlineText.text = data.tournament.Options.RegistrationDeadline.ToString( "MMMM dd, yyyy hh:mmtt (z)" ).ToUpper();
                                                Show();
                                            } );
                                    } )
                                    .Catch( exception => OperationOnDone( "There was a mistake during leaving of a tournament!", false ) );
                            } )
                            .Catch( exception => OperationOnDone( "There was a mistake during leaving of a tournament!", false ) );
                    } )
                    .Catch( exception => OperationOnDone( "There was a mistake during leaving of a tournament!", false ) );
            } );
    }

    void LeaveTournament() {
        loader.IsLoading = true;
        if ( !currentAccountBalanceObject.IsNull() && currentAccountBalanceObject.Amount < currentData.tournament.Options.BuyIn.Amount + currentFee ) {
            OperationOnDone( "Failed\r\nInsufficient Funds available", false );
        } else {
            TournamentTransactionService.LeaveFromTournament( currentOperation )
                .Then( () => {
                    gameObject.SetActive( false );
                    if ( OnOperationSuccess != null ) {
                        OnOperationSuccess();
                    }
                    OperationOnDone( "You have successfully left a tournament!", true );
                } )
                .Catch( exception => OperationOnDone( "There was a mistake during leaving of a tournament!", false ) );
        }
    }

    void OperationOnDone( string messageText, bool isSuccess ) {
        loader.IsLoading = false;
        if ( isSuccess ) {
            messagePopupView.SetSuccessPopup( messageText );
        } else {
            messagePopupView.SerErrorPopup( messageText );
        }
    }

    public override void Hide() {
        gameObject.SetActive( false );
    }

}
