using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Buffers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Tools {

	public class JSONDictionary : Dictionary<string, object> {

		public JSONDictionary() : base() { }

		public JSONDictionary( IDictionary<string, object> fields ) : base( fields ) { }
	}


	public class JSONBuilder {

		StringBuilder builder = new StringBuilder();


		public JSONBuilder( JSONDictionary pairs = null ) {
			if ( !pairs.IsNullOrEmpty() ) {
				WriteKeyValuePairs( pairs );
			}
		}

		public JSONBuilder WriteKeyValuePairs( Dictionary<string, object> pairs, params JsonConverter[] converters ) {
			foreach ( var pair in pairs ) {
				WriteKeyValuePair( pair.Key, pair.Value, converters );
			}
			return this;
		}

		public JSONBuilder WriteKeyValuePair( string key, object value, params JsonConverter[] converters ) {
			key = JsonConvert.SerializeObject( key );
			value = JsonConvert.SerializeObject( value, converters );
			((builder.Length > 0) ? builder.Append( ',' ) : builder).Append( key ).Append( ':' ).Append( value );
			return this;
		}

		public JSONBuilder WriteOptionalStructKeyValuePair<T>( string key, T? nullable, params JsonConverter[] converters ) where T : struct {
			if ( nullable.HasValue ) {
				return WriteKeyValuePair( key, nullable.Value, converters );
			}
			return this;
		}

		public JSONBuilder WriteOptionalClassKeyValuePair<T>( string key, T nullable, params JsonConverter[] converters ) where T : class {
			if ( !nullable.IsNull() ) {
				return WriteKeyValuePair( key, nullable, converters );
			}
			return this;
		}

		public JSONBuilder WriteKeyValuesPair<T>( string key, T[] values, Func<T, string> writeItem ) {
			if ( builder.Length > 0 ) {
				builder.Append( ',' );
			}
			key = JsonConvert.SerializeObject( key );
			builder.Append( key ).Append( ':' ).Append( '[' );
			for ( var i = 0; i < values.Length; i++ ) {
				((i > 0) ? builder.Append( ',' ) : builder).Append( writeItem( values[ i ] ) );
			}
			builder.Append( ']' );
			return this;
		}

		public string Build() {
			return new StringBuilder().Append( '{' ).Append( builder.ToString() ).Append( '}' ).ToString();
		}

		public override string ToString() {
			return Build();
		}
	}


	public abstract class NullableObject {

		public override string ToString() {
			return Serialize();
		}

		public virtual string Serialize() {
			return JsonConvert.SerializeObject( this );
		}
	}


	public class Assert {

		public static void Equal<T>( T a, T b, string alertMessage ) {
			if ( !a.Equals( b ) ) {
				throw new ArgumentException( alertMessage );
			}
		}

		public static void Check( bool condition, string message ) {
			Unity.Console.Assert( condition, message );
		}
	}


	public static class Tool {

		readonly static Dictionary<string, byte> hexLibrary = new Dictionary<string, byte>();
		readonly static Dictionary<string, byte> binaryLibrary = new Dictionary<string, byte>();


		static Dictionary<string, byte> HexLibrary {
			get {
				if ( hexLibrary.Count == 0 ) {
					for ( var i = 0; i < 256; i++ ) {
						hexLibrary.Add( Convert.ToString( ( byte )i, 16 ).PadLeft( 2, '0' ), ( byte )i );
					}
				}
				return hexLibrary;
			}
		}

		static Dictionary<string, byte> BinaryLibrary {
			get {
				if ( binaryLibrary.Count == 0 ) {
					for ( var i = 0; i < 256; i++ ) {
						binaryLibrary.Add( Convert.ToString( ( byte )i, 2 ).PadLeft( 8, '0' ), ( byte )i );
					}
				}
				return binaryLibrary;
			}
		}

		public static string ToHex( byte[] data, char? separator = null ) {
			if ( data.IsNullOrEmpty() ) {
				throw new ArgumentException( "Data is null or empty!" );
			}
			var builder = new StringBuilder();
			for ( var i = 0; i < data.Length; i++ ) {
				((separator.HasValue && (i > 0)) ? builder.Append( separator.Value ) : builder).Append( Convert.ToString( data[ i ], 16 ).PadLeft( 2, '0' ) );
			}
			return builder.ToString();
		}

		public static string ToDecimal( byte[] data, char? separator = null ) {
			if ( data.IsNullOrEmpty() ) {
				throw new ArgumentException( "Data is null or empty!" );
			}
			var builder = new StringBuilder();
			for ( var i = 0; i < data.Length; i++ ) {
				((separator.HasValue && (i > 0)) ? builder.Append( separator.Value ) : builder).Append( Convert.ToString( data[ i ], 10 ) );
			}
			return builder.ToString();
		}

		public static string ToBinary( byte[] data, char? separator = null ) {
			if ( data.IsNullOrEmpty() ) {
				throw new ArgumentException( "Data is null or empty!" );
			}
			var builder = new StringBuilder();
			for ( var i = 0; i < data.Length; i++ ) {
				((separator.HasValue && (i > 0)) ? builder.Append( separator.Value ) : builder).Append( Convert.ToString( data[ i ], 2 ).PadLeft( 8, '0' ) );
			}
			return builder.ToString();
		}

		public static byte[] FromHex( string hex ) {
			if ( hex.IsNullOrEmpty() ) {
				throw new ArgumentException( "Hex is null or empty!" );
			}
			if ( hex.Length % 2 != 0 ) {
				throw new ArgumentException( "The binary key cannot have an odd number of digits: " + hex );
			}
			var result = new List<byte>();
			for ( var i = 0; i < hex.Length; i += 2 ) {
				result.Add( HexLibrary[ hex.Substring( i, 2 ) ] );
			}
			return result.ToArray();
		}

		public static string FromHex2Chars( string hex ) {
			var data = Tool.FromHex( hex );
			var builder = new StringBuilder();
			for ( var i = 0; i < data.Length; i++ ) {
				if ( data[ i ] > 0 ) {
					builder.Append( ( char )data[ i ] );
				}
			}
			return builder.ToString();
		}

		public static byte[] FromBinary( string binary ) {
			if ( binary.IsNullOrEmpty() ) {
				throw new ArgumentException( "Binary is null or empty!" );
			}
			if ( binary.Length % 8 != 0 ) {
				throw new ArgumentException( "The binary key cannot have an odd number of digits: " + binary );
			}
			var result = new List<byte>();
			for ( var i = 0; i < binary.Length; i += 8 ) {
				result.Add( BinaryLibrary[ binary.Substring( i, 8 ) ] );
			}
			return result.ToArray();
		}

		public static string WrapUInt64( ulong value ) {
			return value.ToString();
		}

		public static string WrapInt64( long value ) {
			return value.ToString();
		}

		public static DateTime ZeroTime() {
			return new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
		}

		public static ByteBuffer ToBuffer( Action<ByteBuffer> serializer ) {
			var buffer = new ByteBuffer( ByteBuffer.LITTLE_ENDING );
			serializer( buffer );
			return buffer;
		}

		public static void PrintException( Exception ex ) {
			PrintException( null, ex );
		}

		public static void PrintException( string tag, Exception ex ) {
			var builder = new StringBuilder();
			if ( !tag.IsNullOrEmpty() ) {
				builder.Append( Unity.Console.SetGreenColor( tag ) );
				builder.Append( ' ' );
			}
			builder.Append( ex.GetType().Name );
			builder.Append( '\n' );
			builder.Append( Unity.Console.SetYellowColor( "Message" ) );
			builder.Append( ": " );
			builder.Append( ex.Message );
			if ( !ex.StackTrace.IsNullOrEmpty() ) {
				builder.Append( '\n' );
				builder.Append( Unity.Console.SetRedColor( "StackTrace" ) );
				builder.Append( ": " );
				builder.Append( ex.StackTrace );
			}
			if ( ex.InnerException != null ) {
				PrintException( tag + " -> inner", ex.InnerException );
			}
			Unity.Console.Error( builder.ToString() );
		}
	}


	public static class Extensions {

		public static bool IsNull<T>( this T n ) where T : class {
			return n == null;
		}

		public static bool IsNullOrEmpty( this JContainer jC ) {
			return jC.IsNull() || jC.Count == 0;
		}

		public static T ToNullableObject<T>( this JObject jO ) where T : class {
			return jO.IsNull() ? null : jO.ToObject<T>();
		}

		public static uint Count( this IEnumerable e ) {
			var count = uint.MinValue;
			foreach ( var item in e ) {
				count++;
			}
			return count;
		}

		public static bool IsNullOrEmpty( this ICollection c ) {
			return c.IsNull() || c.Count == 0;
		}

		public static object[] ToArray( this IList l ) {
			if ( l.IsNull() ) {
				return new object[ 0 ];
			}
			var r = new object[ l.Count ];
			for ( var i = 0; i < r.Length; i++ ) {
				r[ i ] = l[ i ];
			}
			return r;
		}

		public static List<T> OrEmpty<T>( this List<T> l ) {
			return l.Or( new List<T>() );	
		}

		public static List<T> Or<T>( this List<T> l, List<T> defaultValue ) {
			return l ?? defaultValue;
		}

		public static bool IsNullOrEmpty( this Base.Data.SpaceTypeId sti ) {
			return sti.IsNull() || Base.Data.SpaceTypeId.EMPTY.Equals( sti );
		}

		#region String
		public static bool IsNullOrEmpty( this string s ) {
			return string.IsNullOrEmpty( s );
		}

		public static string OrEmpty( this string s ) {
			return s.Or( string.Empty );
		}

		public static string Or( this string s, string defaultValue ) {
			return s ?? defaultValue;
		}

		public static string ToNullableString<T>( this T n ) where T : class {
			return n.IsNull() ? null : n.ToString();
		}

		public static string ToString<T>( this List<T> l, string separator = null ) {
			if ( l.IsNull() ) {
				throw new NullReferenceException();
			}
			var builder = new StringBuilder();
			for ( var i = 0; i < l.Count; i++ ) {
				builder.Append( l[ i ].ToString() );
				if ( !separator.IsNullOrEmpty() && i < l.Count - 1 ) {
					builder.Append( separator );
				}
			}
			return builder.ToString();
		}

		public static string ToString( this int value, int radix ) {
			if ( radix < 2 || radix > 36 ) {
				throw new ArgumentException( string.Format( "Radix {0} doesn't support!", radix ) );
			}
			var currentBase = "0123456789abcdefghijklmnopqrstuvwxyz".Substring( 0, radix ).ToCharArray();

			// 32 is the worst cast buffer size for base 2 and int.MaxValue
			var i = 32;
			var buffer = new char[ i ];

			do {
				buffer[ --i ] = currentBase[ value % radix ];
				value /= radix;
			} while ( value > 0 );

			var result = new char[ 32 - i ];
			Array.Copy( buffer, i, result, 0, 32 - i );

			return new string( result );
		}
		#endregion

		#region Array
		public static bool IsArray( this object o ) {
			if ( o.IsNull() ) {
				throw new NullReferenceException();
			}
			return o.GetType().IsArray;
		}

		public static bool IsNullOrEmpty<T>( this T[] a ) {
			return a.IsNull() || a.Length == 0;
		}

		public static T[] OrEmpty<T>( this T[] a ) {
			return a ?? new T[ 0 ];
		}

		public static T[] Add<T>( this T[] a, T b ) {
			if ( a.IsNull() ) {
				throw new NullReferenceException();
			}
			Array.Resize( ref a, a.Length + 1 );
			a[ a.Length - 1 ] = b;
			return a;
		}

		public static T[] Remove<T>( this T[] a, T b ) {
			if ( a.IsNull() ) {
				throw new NullReferenceException();
			}
			var temp = new T[ 0 ];
			var removed = false;
			for ( var i = 0; i < a.Length; i++ ) {
				if ( removed || !a[ i ].Equals( b ) ) {
					temp = temp.Add( a[ i ] );
				} else {
					// skip
					removed = true;
				}
			}
			if ( a.Length > 0 ) {
				Array.Resize( ref a, a.Length - 1 );
			}
			for ( var i = 0; i < a.Length; i++ ) {
				a[ i ] = temp[ i ];
			}
			return a;
		}

		public static T[] Concat<T>( this T[] a, params T[][] b ) {
			if ( a.IsNull() ) {
				throw new NullReferenceException();
			}
			if ( b.IsNull() ) {
				throw new NullReferenceException();
			}
			var offset = a.Length;
			var newSize = a.Length;
			for ( var i = 0; i < b.Length; i++ ) {
				newSize += b[ i ].Length;
			}
			Array.Resize( ref a, newSize );
			for ( var i = 0; i < b.Length; i++ ) {
				for ( var j = 0; j < b[ i ].Length; j++ ) {
					a[ offset + j ] = b[ i ][ j ];
				}
				offset += b[ i ].Length;
			}
			return a;
		}

		public static T[] Slice<T>( this T[] b, int fromIndex, int toIndex ) {
			if ( b.IsNull() ) {
				throw new NullReferenceException();
			}
			if ( fromIndex < 0 || fromIndex >= b.Length ) {
				throw new ArgumentOutOfRangeException( "fromIndex" );
			}
			if ( toIndex < 0 || toIndex > b.Length ) {
				throw new ArgumentOutOfRangeException( "toIndex" );
			}
			if ( fromIndex > toIndex ) {
				throw new ArgumentException( "from index > to index" );
			}
			var result = new T[ toIndex - fromIndex ];
			for ( var i = fromIndex; i < toIndex; i++ ) {
				result[ i - fromIndex ] = b[ i ];
			}
			return result;
		}

		public static T[] Slice<T>( this T[] b, int fromIndex = 0 ) {
			return b.Slice( fromIndex, b.Length );
		}

		public static bool DeepEqual<T>( this T[] a, T[] b ) where T : struct {
			if ( a.IsNull() ) {
				throw new NullReferenceException();
			}
			if ( b.IsNull() ) {
				throw new NullReferenceException();
			}
			if ( a.Length != b.Length ) {
				return false;
			}
			for ( var i = 0; i < a.Length; i++ ) {
				if ( !a[ i ].Equals( b[ i ] ) ) {
					return false;
				}
			}
			return true;
		}

		public static T[] Fill<T>( this T[] a, T b ) {
			if ( a.IsNull() ) {
				throw new NullReferenceException();
			}
			for ( var i = 0; i < a.Length; i++ ) {
				a[ i ] = b;
			}
			return a;
		}

		public static bool Contains<T>( this T[] a, T o ) {
			if ( a.IsNullOrEmpty() ) {
				return false;
			}
			foreach ( var item in a ) {
				if ( item.Equals( o ) ) {
					return true;
				}
			}
			return false;
		}

		public static T First<T>( this T[] a ) where T : class {
			if ( a.IsNullOrEmpty() ) {
				return null;
			}
			return a[ 0 ];
		}

		public static T Last<T>( this T[] a ) where T : class {
			if ( a.IsNullOrEmpty() ) {
				return null;
			}
			return a[ a.Length - 1 ];
		}
		#endregion

		#region DateTime
		public static bool IsZero( this DateTime dt ) {
			return dt.GetTimeFrom1Jan1970().TotalMilliseconds < 1.0;
		}

		static TimeSpan GetTimeFrom1Jan1970( this DateTime dt ) {
			if ( dt.Kind.Equals( DateTimeKind.Utc ) ) {
				return dt - Tool.ZeroTime();
			}
			if ( dt.Kind.Equals( DateTimeKind.Local ) ) {
				return dt.ToUniversalTime() - Tool.ZeroTime();
			}
			throw new ArgumentException( "Unexpected kind for datetime: " + dt.Kind );
		}

		public static uint GetTimeFrom1Jan1970AtSeconds( this DateTime dt ) {
			var milliseconds = dt.GetTimeFrom1Jan1970AtMilliseconds();
			var seconds = milliseconds / 1000.0;
			seconds = Math.Floor( seconds );
			return ( uint )seconds;
		}

		public static double GetTimeFrom1Jan1970AtMilliseconds( this DateTime dt ) {
			return dt.GetTimeFrom1Jan1970().TotalMilliseconds;
		}
		#endregion

		public static bool IsNumeric( this Type t ) {
			switch ( Type.GetTypeCode( t ) ) {
			case TypeCode.Byte:
			case TypeCode.SByte:
			case TypeCode.UInt16:
			case TypeCode.UInt32:
			case TypeCode.UInt64:
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
			case TypeCode.Decimal:
			case TypeCode.Double:
			case TypeCode.Single:
				return true;
			default:
				return false;
			}
		}

		public static bool IsFloating( this Type t ) {
			switch ( Type.GetTypeCode( t ) ) {
			case TypeCode.Double:
			case TypeCode.Single:
				return true;
			default:
				return false;
			}
		}

		#region Hash
		public static byte[] HashAndDispose( this SHA256 sha256, byte[] buffer ) {
			var hash = sha256.ComputeHash( buffer );
			(sha256 as IDisposable).Dispose();
			return hash;
		}

		public static byte[] HashAndDispose( this SHA512 sha512, byte[] buffer ) {
			var hash = sha512.ComputeHash( buffer );
			(sha512 as IDisposable).Dispose();
			return hash;
		}

		public static byte[] HashAndDispose( this RIPEMD160 ripemd160, byte[] buffer ) {
			var hash = ripemd160.ComputeHash( buffer );
			(ripemd160 as IDisposable).Dispose();
			return hash;
		}

		public static byte[] HashAndDispose( this HMACSHA256 hmacsha256, byte[] buffer ) {
			var hash = hmacsha256.ComputeHash( buffer );
			(hmacsha256 as IDisposable).Dispose();
			return hash;
		}
		#endregion
	}
}