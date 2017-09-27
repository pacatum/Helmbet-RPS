using System;
using Base;
using Base.Config;
using Base.Data;
using Base.Data.Operations.Fee;
using Base.Data.Properties;
using Base.Data.Tournaments;
using Base.Transactions.Tournaments;
using UnityEngine;
using UnityEngine.UI;


public class CreateNewTournamentPopupView : MonoBehaviour {

    [SerializeField] Text gameTitleText;
    [SerializeField] Text creatorNameText;
    [SerializeField] Text numberOfPlayersText;
    [SerializeField] Text winsAmountText;
    [SerializeField] Text registrationDeadlineText;
    [SerializeField] Text buyinText;
    [SerializeField] Text feeText;

    [SerializeField] Button createButton;
    [SerializeField] Button cancelButton;

    [SerializeField] ScreenLoader loader;

    [SerializeField] MessagePopupView messagePopupView;
    [SerializeField] ChooseHandController chooseHandController;

    CreateTournamentData currentData;
    bool isJoinToTournament;
    bool isChoosenHand = false;


    public string Fee {
        get { return feeText == null ? string.Empty : feeText.text; }
        set {
            if ( feeText != null ) {
                feeText.text = value;
            }
        }
    }

    void Awake() {
        cancelButton.onClick.AddListener( Close );
        createButton.onClick.AddListener( CreateTournament );
    }

    public void SetTournamentInformation( CreateTournamentData data, bool isJoin ) {
        isJoinToTournament = isJoin;
        currentData = data;
        gameTitleText.text = "ROCK, PAPER, SCISSORS";
        var username = AuthorizationManager.Instance.Authorization.UserNameData.UserName;
        creatorNameText.text = Utils.GetFormatedString(username); 
        numberOfPlayersText.text = data.numberOfPlayers.ToString();
        winsAmountText.text = data.numberOfWins.ToString();
        registrationDeadlineText.text = data.registrationDeadline.ToString( "MMMM dd, yyyy hh:mmtt (z)" ).ToUpper();
        TournamentManager.Instance.GetAssetObject( data.buyInAssetId )
            .Then( asset => {
                buyinText.text =
                    Utils.GetFormatedDecimaNumber( data.buyInAmount.ToString() ) + asset.Symbol;
                var feeAsset = SpaceTypeId.CreateOne( SpaceType.GlobalProperties );

                Repository
                    .GetInPromise<GlobalPropertiesObject>( feeAsset )
                    .Then( result => {

                        TournamentJoinOperationFeeParametersData joinFee = null;
                        TournamentCreateOperationFeeParametersData createFee = null;

                        foreach ( var fee in result.Parameters.CurrentFees.Parameters ) {
                            if ( fee != null && fee.Type == ChainTypes.FeeParameters.TournamentJoinOperation ) {
                                joinFee = fee as TournamentJoinOperationFeeParametersData;
                            }
                            if ( fee != null && fee.Type == ChainTypes.FeeParameters.TournamentCreateOperation ) {
                                createFee = fee as TournamentCreateOperationFeeParametersData;
                            }
                        }

                        if ( isJoin ) {
                            feeText.text = Convert.ToDouble( createFee.Fee + joinFee.Fee ) /
                                           Convert.ToDouble( result.Parameters.CurrentFees.Scale * 10 ) + asset.Symbol;
                        } else {
                            feeText.text = Convert.ToDouble( createFee.Fee ) /
                                           Convert.ToDouble( result.Parameters.CurrentFees.Scale * 10 ) + asset.Symbol;
                        }

                        gameObject.SetActive( true );

                    }) ;
            } );

    }


    void JoinTournament( SpaceTypeId tournament ) {
        if ( isJoinToTournament ) {
            Action<TournamentObject> JoinToTournament = tournamentObject => {
                var joinTournamentData = new JoinToTournamentData();

                joinTournamentData.tournament = tournamentObject;
                joinTournamentData.account = AuthorizationManager.Instance.Authorization.UserNameData.FullAccount
                    .Account.Id;

                TournamentTransactionService.GenerateJoinToTournamentOperation( joinTournamentData )
                    .Then( operation => {
                        TournamentTransactionService.JoinToTournament( operation )
                            .Then( () => {
                                JoinOperationOnDone( "Your tournament was successfully created & joined!", true );
                            } )
                            .Catch( exception => {
                                JoinOperationOnDone( "Your tournament was successfully created, but not joined!",
                                                    false );
                            } );
                    } )
                    .Catch( exception => JoinOperationOnDone( "There was a mistake during joining of a tournament!",
                                                             false ) );
            };
            Repository.GetInPromise( tournament, () =>TournamentManager.Instance.LoadTournament( tournament.Id ) )
                .Then( JoinToTournament );
        }
    }

    void JoinToTournamentAfterChoosingHand() {
        isChoosenHand = true;
        CreateTournament();
        //chooseHandController.OnApplyClick -= JoinToTournamentAfterChoosingHand;
    }

    void JoinOperationOnDone( string operationMessage, bool isSuccces ) {
        TournamentTransactionService.OnCreateTournamentResult -= JoinTournament;
        if ( isSuccces ) {
            CreateOperationOnDone( operationMessage );
        } else {
            CreateOperationOnFailed( operationMessage );
        }
    }

    void CreateTournament() {
        //if ( isJoinToTournament && !isChoosenHand ) {
        //	//chooseHandController.Show();
        //	//chooseHandController.OnApplyClick += JoinToTournamentAfterChoosingHand;
        //} else {
        loader.IsLoading = true;
        isChoosenHand = false;
        TournamentTransactionService.GenerateCreateTournamentOperation( currentData )
            .Then( operation => {
                TournamentTransactionService.CreateTournament( operation )
                    .Then( () => {
                        TournamentTransactionService.OnCreateTournamentResult -= JoinTournament;
                        if ( isJoinToTournament ) {
                            TournamentTransactionService.OnCreateTournamentResult += JoinTournament;
                        } else {
                            CreateOperationOnDone( "Your tournament was successfully created!" );
                        }
                    } )
                    .Catch( exception => {
                        CreateOperationOnFailed( "There was a mistake during creation of a tournament!" );
                    } );
            } )
            .Catch( exception =>
                       CreateOperationOnFailed( "There was a mistake during creation of a tournament!" ) );

    }


    void CreateOperationOnDone( string message ) {
        messagePopupView.SetSuccessPopup( message );
        gameObject.SetActive( false );
        loader.IsLoading = false;
    }

    void CreateOperationOnFailed( string message ) {
        messagePopupView.SerErrorPopup( message );
        gameObject.SetActive( false );
        loader.IsLoading = false;
    }

    void Close() {
        gameObject.SetActive( false );
    }

}