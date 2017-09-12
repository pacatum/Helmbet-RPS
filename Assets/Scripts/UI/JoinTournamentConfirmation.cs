using System;
using Base;
using Base.Config;
using Base.Data;
using Base.Data.Assets;
using Base.Data.Operations;
using Base.Data.Operations.Fee;
using Base.Data.Properties;
using Base.Data.Tournaments;
using Base.Transactions.Tournaments;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class JoinTournamentConfirmation : BaseCanvasView {

    public event Action OnOperationSuccess;

    [SerializeField] Text gameTitleText;
    [SerializeField] Text creatorNameText;
    [SerializeField] Text numberOfPlayersText;
    [SerializeField] Text winsAmountText;
    [SerializeField] Text registrationDeadlineText;
    [SerializeField] Text buyinText;
    [SerializeField] Text feeText;

    [SerializeField] Button joinButton;
    [SerializeField] Button cancelButton;

    [SerializeField] ScreenLoader loader;

    [SerializeField] MessagePopupView messagePopupView;

    private JoinToTournamentData currentData;
    private long currentFee;
    private TournamentLeaveOperationData currentOperation;
    private AssetData currentAccountBalanceObject;

    public override void Awake() {
        base.Awake();
        cancelButton.onClick.AddListener( Hide );
        joinButton.onClick.AddListener( JoinTournament );
    }

    public void SetUp( TournamentObject tournament ) {

        var data = new JoinToTournamentData();
        data.tournament = tournament;
        data.account = AuthorizationManager.Instance.UserData.FullAccount.Account.Id;

        currentData = data;

        gameTitleText.text = "ROCK, PAPER, SCISSORS";
        var username = AuthorizationManager.Instance.Authorization.UserNameData.UserName;
        creatorNameText.text = username.Length > 10 ? username.Substring( 0, 10 ) + "..." : username;
        numberOfPlayersText.text = data.tournament.Options.NumberOfPlayers.ToString();
        winsAmountText.text = data.tournament.Options.NumberOfWins.ToString();
        registrationDeadlineText.text = data.tournament.Options.RegistrationDeadline
            .ToString( "MMMM dd, yyyy hh:mmtt (z)" )
            .ToUpper();

        ApiManager.Instance.Database
            .GetAccountBalances( AuthorizationManager.Instance.UserData.FullAccount.Account.Id.Id,
                                Array.ConvertAll( AuthorizationManager.Instance.UserData.FullAccount.Balances,
                                                 balance => balance.AssetType.Id ) )
            .Then( accountBalances
                      => {
                      AssetData asset = null;
                      foreach ( var balance in accountBalances ) {
                          if ( balance.Asset.Equals( tournament.Options.BuyIn.Asset ) ) {
                              asset = balance;
                          }
                      }

                      var feeAsset = SpaceTypeId.CreateOne( SpaceType.GlobalProperties );

                      Repository
                          .GetInPromise<GlobalPropertiesObject>( feeAsset )
                          .Then( result => {

                              TournamentJoinOperationFeeParametersData myFee = null;

                              foreach ( var fee in result.Parameters.CurrentFees.Parameters ) {
                                  if ( fee != null && fee.Type == ChainTypes.FeeParameters.TournamentJoinOperation ) {

                                      myFee = fee as TournamentJoinOperationFeeParametersData;
                                  }
                              }

                              currentAccountBalanceObject = asset;
                              currentFee = (long) myFee.Fee;
                              Repository.GetInPromise<AssetObject>( asset.Asset )
                                  .Then( assetObject => {
                                      buyinText.text =
                                          tournament.Options.BuyIn.Amount /
                                          Math.Pow( 10, assetObject.Precision ) + assetObject.Symbol;
                                      feeText.text =
                                          myFee.Fee / Math.Pow( 10, assetObject.Precision ) +
                                          assetObject.Symbol;
                                      gameObject.SetActive( true );

                                  } );

                          } );
                  } );
    }


    void JoinTournament() {
        loader.IsLoading = true;

        if ( !IsPlayerInWhitelist( currentData.tournament.Options.Whitelist ) ) {
            OperationOnDone( "You cant join to the tournament, because you arent in whitelist!", false );
        } else if ( !currentAccountBalanceObject.IsNull() && currentAccountBalanceObject.Amount <
                    currentData.tournament.Options.BuyIn.Amount + currentFee ) {
            OperationOnDone( "Failed\r\nInsufficient Funds available", false );
        } else {

            TournamentTransactionService.GenerateJoinToTournamentOperation( currentData )
                .Then( operaion => {
                    TournamentTransactionService.JoinToTournament( operaion )
                        .Then( () => {
                            gameObject.SetActive( false );
                            if ( OnOperationSuccess != null ) {
                                OnOperationSuccess();
                            }
                            OperationOnDone( "You successfully joined to tournament №" + currentData.tournament.Id.Id,
                                            true );
                        } )
                        .Catch( exception => OperationOnDone( "There was a mistake during joining of a tournament!",
                                                             false ) );
                } )
                .Catch( exception => OperationOnDone( "There was a mistake during joining of a tournament!",
                                                     false ) );

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

    bool IsPlayerInWhitelist( SpaceTypeId[] whitelist ) {
        if ( whitelist.Length == 0 ) {
            return true;
        }
        return whitelist.Contains( AuthorizationManager.Instance.UserData.FullAccount.Account.Id );
    }

}
