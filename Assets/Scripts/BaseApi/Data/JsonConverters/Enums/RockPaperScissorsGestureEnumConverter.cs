using System;
using Base.Config;


namespace Base.Data.Json {

	public sealed class RockPaperScissorsGestureEnumConverter : JsonCustomConverter<ChainTypes.RockPaperScissorsGesture, string> {

		const string ROCK = "rock"; // \m/,
		const string PAPER = "paper";
		const string SCISSORS = "scissors";
		const string SPOCK = "spock";
		const string LIZARD = "lizard";


		protected override ChainTypes.RockPaperScissorsGesture Deserialize( string value, Type objectType ) {
			return ConvertFrom( value );
		}

		protected override string Serialize( ChainTypes.RockPaperScissorsGesture value ) {
			return ConvertTo( value );
		}

		public static string ConvertTo( ChainTypes.RockPaperScissorsGesture state ) {
			switch ( state ) {
			case ChainTypes.RockPaperScissorsGesture.Rock:
				return ROCK;
			case ChainTypes.RockPaperScissorsGesture.Paper:
				return PAPER;
			case ChainTypes.RockPaperScissorsGesture.Scissors:
				return SCISSORS;
			case ChainTypes.RockPaperScissorsGesture.Spock:
				return SPOCK;
			case ChainTypes.RockPaperScissorsGesture.Lizard:
				return LIZARD;
			}
			throw new ArgumentException( "Unexpected value: " + state );
		}

		public static ChainTypes.RockPaperScissorsGesture ConvertFrom( string state ) {
			switch ( state ) {
			case ROCK:
				return ChainTypes.RockPaperScissorsGesture.Rock;
			case PAPER:
				return ChainTypes.RockPaperScissorsGesture.Paper;
			case SCISSORS:
				return ChainTypes.RockPaperScissorsGesture.Scissors;
			case SPOCK:
				return ChainTypes.RockPaperScissorsGesture.Spock;
			case LIZARD:
				return ChainTypes.RockPaperScissorsGesture.Lizard;
			}
			throw new ArgumentException( "Unexpected value: " + state );
		}
	}
}