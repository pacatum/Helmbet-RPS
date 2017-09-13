using UnityEngine;
using System.Text;


namespace Unity {

	public enum LogEnum {
		
		Message,
		Warning,
		Error
	}


	public class Console : ILogHandler {

		#region class
		ILogHandler defaultLogHandler = Debug.logger.logHandler;

		Console() { }


		public void LogFormat( LogType logType, Object context, string format, params object[] args ) {
			// todo
			defaultLogHandler.LogFormat( logType, context, format, args );
		}

		public void LogException( System.Exception exception, Object context ) {
			defaultLogHandler.LogException( exception, context );
		}
		#endregion


		public static void Set() {
			if ( Debug.logger.logHandler is Console ) {
				return;
			}
			Debug.logger.logHandler = new Console();
		}

		public static void UnSet() {
			if ( Debug.logger.logHandler is Console ) {
				Debug.logger.logHandler = (Debug.logger.logHandler as Console).defaultLogHandler;
			}
		}

		static string ColorToHex( Color32 color ) {
			return "#" + color.r.ToString( "X2" ) + color.g.ToString( "X2" ) + color.b.ToString( "X2" );
		}

		public static string SetRedColor( params object[] source ) {
			return SetColor( Color.red, ' ', source );
		}

		public static string SetGreenColor( params object[] source ) {
			return SetColor( Color.green, ' ', source );
		}

		public static string SetBlueColor( params object[] source ) {
			return SetColor( Color.blue, ' ', source );
		}

		public static string SetCyanColor( params object[] source ) {
			return SetColor( Color.cyan, ' ', source );
		}

		public static string SetMagentaColor( params object[] source ) {
			return SetColor( Color.magenta, ' ', source );
		}

		public static string SetYellowColor( params object[] source ) {
			return SetColor( Color.yellow, ' ', source );
		}

		public static string SetWhiteColor( params object[] source ) {
			return SetColor( Color.white, ' ', source );
		}

		public static string SetBlackColor( params object[] source ) {
			return SetColor( Color.black, ' ', source );
		}

		public static string SetGrayColor( params object[] source ) {
			return SetColor( Color.gray, ' ', source );
		}

		public static string SetColor( Color color, char separator, params object[] messages ) {
			var builder = new StringBuilder();
			for ( var i = 0; i < messages.Length; i++ ) {
				((i > 0) ? builder.Append( separator ) : builder).Append( (messages[ i ] ?? "null").ToString() );
			}
#if UNITY_EDITOR
			if ( builder.Length > 0 ) {
				builder.Insert( 0, string.Format( "<color={0}>", ColorToHex( color ) ) );
				builder.Append( "</color>" );
			}
#endif
			return builder.ToString();
		}

		public static void DebugLog( params object[] messages ) {
			DebugLog( LogEnum.Message, ' ', messages );
		}

		public static void DebugWarning( params object[] messages ) {
			DebugLog( LogEnum.Warning, ' ', messages );
		}

		public static void DebugError( params object[] messages ) {
			DebugLog( LogEnum.Error, ' ', messages );
		}

		static void DebugLog( LogEnum type, char separator, params object[] messages ) {
#if DEBUG
			var builder = new StringBuilder();
			builder.Append( "DEBUG:" );
			for ( var i = 0; i < messages.Length; i++ ) {
				builder.Append( separator ).Append( (messages[ i ] ?? "null").ToString() );
			}
			switch ( type ) {
			case LogEnum.Message:
				Debug.Log( builder.ToString() );
				break;
			case LogEnum.Warning:
				Debug.LogWarning( builder.ToString() );
				break;
			case LogEnum.Error:
				Debug.LogError( builder.ToString() );
				break;
			}
#endif
		}

		public static void Log( params object[] messages ) {
			Log( LogEnum.Message, ' ', messages );
		}

		public static void Warning( params object[] messages ) {
			Log( LogEnum.Warning, ' ', messages );
		}

		public static void Error( params object[] messages ) {
			Log( LogEnum.Error, ' ', messages );
		}

		public static void Assert( bool condition, string message ) {
			Debug.AssertFormat( condition, "{0}", message );
		}

		static void Log( LogEnum type, char separator, params object[] messages ) {
			var builder = new StringBuilder();
			for ( var i = 0; i < messages.Length; i++ ) {
				((i > 0) ? builder.Append( separator ) : builder).Append( (messages[ i ] ?? "null").ToString() );
			}
			switch ( type ) {
			case LogEnum.Message:
				Debug.Log( builder.ToString() );
				break;
			case LogEnum.Warning:
				Debug.LogWarning( builder.ToString() );
				break;
			case LogEnum.Error:
				Debug.LogError( builder.ToString() );
				break;
			}
		}
	}
}