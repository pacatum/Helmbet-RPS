using System.Collections;
using System.Collections.Generic;
using Base.Data.Tournaments;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using Base.Data.Assets;
using UnityEngine.EventSystems;
using Base.Config;
using Tools;

public enum ItemState { Normal, Hover}

public class BaseTournamentItemView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    

    [SerializeField] protected Text idText;
    [SerializeField] protected Text gameText;
    [SerializeField] protected Text playerRegisteredText;
    [SerializeField] protected Text maxPlayersText;
    [SerializeField] protected Text registrationDeadlineText;
    [SerializeField] protected Text startTimeText;
    [SerializeField] protected Text buyInText;
    [SerializeField] protected Text jackpotText;
    [SerializeField] protected UnityEngine.GameObject divider;

    [SerializeField] protected Color normalColor;
    [SerializeField] protected Color hoverColor;

    protected Image itemBackground;
    protected TournamentObject currentTournament;
    protected TournamentDetailsObject tournamentDetailsObject;
    protected AssetObject currentAsset;
    

    protected void SwitchState( ItemState state ) {
        switch ( state ) {
            case ItemState.Normal:
                itemBackground.color = normalColor;
                break;
            case ItemState.Hover:
                itemBackground.color = hoverColor;
                break;
        }
    }

    public virtual TournamentObject CurrentTournament {
        get { return currentTournament; }
        set { currentTournament = value; }
    }

    protected virtual void Start() {
        itemBackground = GetComponent<Image>();
        SwitchState( ItemState.Normal );
    }


    public string ID {
        get { return ( idText == null ) ? string.Empty : idText.text; }
        protected set {
            if ( idText != null ) {
                idText.text = value;
            }
        }
    }

    public string Game {
        get { return ( gameText == null ) ? string.Empty : gameText.text; }
        protected set {
            if ( gameText != null ) {
                gameText.text = value;
            }
        }
    }

    public string PlayerRegistered {
        get { return ( playerRegisteredText == null ) ? string.Empty : playerRegisteredText.text; }
        protected set {
            if ( playerRegisteredText != null ) {
                playerRegisteredText.text = value;
            }
        }
    }

    public string MaxPlayers {
        get { return ( maxPlayersText == null ) ? string.Empty : maxPlayersText.text; }
        protected set {
            if ( maxPlayersText != null ) {
                maxPlayersText.text = value;
            }
        }
    }

    public string RegistrationDeadline {
        get { return ( registrationDeadlineText == null ) ? string.Empty : registrationDeadlineText.text; }
        protected set {
            if ( registrationDeadlineText != null ) {
                registrationDeadlineText.text = value;
            }
        }
    }

    public string StartTime {
        get { return ( startTimeText == null ) ? string.Empty : startTimeText.text; }
        protected set {
            if ( startTimeText != null ) {
                startTimeText.text = value;
            }
        }
    }

    public string BuyIn {
        get { return ( buyInText == null ) ? string.Empty : buyInText.text; }
        protected set {
            if ( buyInText != null ) {
                buyInText.text = value;
            }
        }
    }

    public string Jackpot {
        get { return ( jackpotText == null ) ? string.Empty : jackpotText.text; }
        protected set {
            if ( jackpotText != null ) {
                jackpotText.text = value;
            }
        }
    }

    public virtual IEnumerator UpdateItem( TournamentObject info, TournamentDetailsObject details ) {
        if ( gameObject.activeSelf && gameObject.activeInHierarchy ) {
            CurrentTournament = info;

            if ( details.IsNull() ) {
                var detailsObject = new List<TournamentDetailsObject>();
                yield return TournamentManager.Instance.GetTournamentDetailsObject( info.Id.Id, detailsObject );
                tournamentDetailsObject = detailsObject[0];
            } else {
                tournamentDetailsObject = details;
            }

            Game = "RPS";
            ID = "#RPS" + info.Id;
            PlayerRegistered = info.RegisteredPlayers.ToString();
            MaxPlayers = info.Options.NumberOfPlayers.ToString();
            RegistrationDeadline = info.Options.RegistrationDeadline.ToLocalTime().ToString( "ddMMM, yyyy. HH:mm tt" );


            if ( currentAsset.IsNull() || !currentAsset.Id.Equals( currentTournament.Options.BuyIn.Asset ) ) {
                AssetObject asset = null;
                TournamentManager.Instance.GetAssetObject( currentTournament.Options.BuyIn.Asset.Id )
                    .Then( assetResult =>
                              asset = assetResult );


                while ( asset.IsNull() ) {
                    yield return null;
                }

                currentAsset = asset;
            }

            var buyIn =
                Decimal.Parse( ( info.Options.BuyIn.Amount / Math.Pow( 10, currentAsset.Precision ) ).ToString(),
                              NumberStyles.Float );

            BuyIn = buyIn + currentAsset.Symbol;
            Jackpot = buyIn * info.Options.NumberOfPlayers + currentAsset.Symbol;

            UpdateStartTime();
        }
    }

    protected virtual void UpdateStartTime() {
        if ( currentTournament.Options.StartTime.HasValue ) {
            DateTime time = currentTournament.Options.StartTime.Value;
            StartTime = time.ToLocalTime().ToString( "ddMMM, yyyy. HH:mm tt" );
        } else {
            StartTime = "2 minutes after full";
        }
    }

    protected virtual string SetDateTime( string dateTime ) {
        var time = DateTime.MinValue;
        if ( DateTime.TryParse( dateTime, out time ) ) {
            return time.ToLocalTime().ToString( "ddMMM, yyyy. HH:mm tt" );
        }

        return time.ToString( "ddMMM, yyyy. HH:mm tt" );
    }


    public virtual void UpdateTournament( TournamentObject tournament ) {
    }

    public void UpdateDetails( TournamentObject tournament ) {
        StartCoroutine( UpdateItem( tournament, tournamentDetailsObject ) );
    }

    public void OnPointerExit( PointerEventData eventData ) {
        SwitchState( ItemState.Normal );
    }

    public void OnPointerEnter( PointerEventData eventData ) {
        SwitchState( ItemState.Hover );
    }

    public void OnPointerClick( PointerEventData eventData ) {

        if ( currentTournament.IsNull() ) {
            return;
        }
        switch ( currentTournament.State ) {
            case ChainTypes.TournamentState.InProgress:
                ToGame();
                break;

            case ChainTypes.TournamentState.Concluded:
                UIController.Instance.UpdateTournamentDetails( currentTournament);
                UIManager.Instance.CurrentState = UIManager.ScreenState.TournamentDetails;
                break;

            case ChainTypes.TournamentState.AwaitingStart:
                var me = AuthorizationManager.Instance.UserData.FullAccount.Account.Id;

                if ( tournamentDetailsObject.RegisteredPlayers.Contains( me ) && (currentTournament.StartTime.Value - DateTime.UtcNow).TotalMinutes <= 2) {
                    UIController.Instance.UpdateStartGamePreview(currentTournament);
                }
                break;
        }
    }

    protected void ToGame() {
        var me = AuthorizationManager.Instance.UserData.FullAccount.Account.Id;

        
        if ( tournamentDetailsObject.RegisteredPlayers.Contains( me ) ) {
            ApiManager.Instance.Database
                .GetMatches( Array.ConvertAll( tournamentDetailsObject.Matches, match => match.Id ) )
                .Then( matches
                          => {
                          var matchesInProgress =
                              Array.FindAll( matches,
                                            match => match.State == ChainTypes.MatchState.InProgress);
                          var playerInMatches =
                              Array.FindAll( matchesInProgress, player => player.Players.Contains( me ) );

                          if ( playerInMatches.Length == 0 ) {
                              UIController.Instance.UpdateTournamentDetails( currentTournament );
                              UIManager.Instance.CurrentState = UIManager.ScreenState.TournamentDetails;
                          } else {
                              UIController.Instance.UpdateTournamentInProgress( currentTournament );
                          }

                      } );

        } else {
            UIController.Instance.UpdateTournamentDetails( currentTournament);
            UIManager.Instance.CurrentState = UIManager.ScreenState.TournamentDetails;
        }
    }

    public bool Divider {
        get { return divider.activeSelf; }
        set {
            if ( divider != null ) {
                divider.SetActive( value );
            }
        }
    }

    //public virtual bool IsMatch( string searchText ) {
    //    return ID.Contains( searchText ) || StartTime.Contains( searchText ) ||
    //           RegistrationDeadline.Contains( searchText )
    //           || BuyIn.Contains( searchText ) || Jackpot.Contains( searchText ) ||
    //           PlayerRegistered.Contains( searchText ) || MaxPlayers.Contains( searchText )
    //           || Game.Contains( searchText );
    //}

}
