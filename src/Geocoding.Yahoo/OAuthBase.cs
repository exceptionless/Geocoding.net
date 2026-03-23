using System.Net;
using System.Security.Cryptography;
using System.Text;

//http://oauth.googlecode.com/svn/code/csharp/OAuthBase.cs
namespace Geocoding.Yahoo;

/// <summary>
/// Provides helper methods for generating OAuth 1.0 signatures.
/// </summary>
[Obsolete("Yahoo PlaceFinder/BOSS geocoding has been discontinued. This type is retained for source compatibility only and will be removed in a future major version.")]
public class OAuthBase
{

    /// <summary>
    /// Provides a predefined set of algorithms that are supported officially by the protocol
    /// </summary>
    public enum SignatureTypes
    {
        /// <summary>The HMACSHA1 value.</summary>
        HMACSHA1,
        /// <summary>The PLAINTEXT value.</summary>
        PLAINTEXT,
        /// <summary>The RSASHA1 value.</summary>
        RSASHA1
    }

    /// <summary>
    /// Provides an internal structure to sort the query parameter
    /// </summary>
    protected class QueryParameter
    {
        private readonly string _name = null!;
        private readonly string _value = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameter"/> class.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        public QueryParameter(string name, string value)
        {
            _name = name;
            _value = value;
        }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        public string Value
        {
            get { return _value; }
        }
    }

    /// <summary>
    /// Comparer class used to perform the sorting of the query parameters
    /// </summary>
    protected class QueryParameterComparer : IComparer<QueryParameter>
    {

        #region IComparer<QueryParameter> Members

        /// <summary>
        /// Compares two query parameters for sorting.
        /// </summary>
        /// <param name="x">The first parameter.</param>
        /// <param name="y">The second parameter.</param>
        /// <returns>A signed integer indicating relative sort order.</returns>
        public int Compare(QueryParameter x, QueryParameter y)
        {
            if (x.Name == y.Name)
            {
                return String.Compare(x.Value, y.Value);
            }
            else
            {
                return String.Compare(x.Name, y.Name);
            }
        }

        #endregion
    }

    /// <summary>
    /// The OAuth protocol version.
    /// </summary>
    protected const string OAuthVersion = "1.0";
    /// <summary>
    /// The OAuth parameter prefix.
    /// </summary>
    protected const string OAuthParameterPrefix = "oauth_";

    //
    // List of know and used oauth parameters' names
    //
    /// <summary>
    /// The OAuth consumer key parameter name.
    /// </summary>
    protected const string OAuthConsumerKeyKey = "oauth_consumer_key";
    /// <summary>
    /// The OAuth callback parameter name.
    /// </summary>
    protected const string OAuthCallbackKey = "oauth_callback";
    /// <summary>
    /// The OAuth version parameter name.
    /// </summary>
    protected const string OAuthVersionKey = "oauth_version";
    /// <summary>
    /// The OAuth signature method parameter name.
    /// </summary>
    protected const string OAuthSignatureMethodKey = "oauth_signature_method";
    /// <summary>
    /// The OAuth signature parameter name.
    /// </summary>
    protected const string OAuthSignatureKey = "oauth_signature";
    /// <summary>
    /// The OAuth timestamp parameter name.
    /// </summary>
    protected const string OAuthTimestampKey = "oauth_timestamp";
    /// <summary>
    /// The OAuth nonce parameter name.
    /// </summary>
    protected const string OAuthNonceKey = "oauth_nonce";
    /// <summary>
    /// The OAuth token parameter name.
    /// </summary>
    protected const string OAuthTokenKey = "oauth_token";
    /// <summary>
    /// The OAuth token secret parameter name.
    /// </summary>
    protected const string OAuthTokenSecretKey = "oauth_token_secret";

    /// <summary>
    /// The HMAC-SHA1 signature type name.
    /// </summary>
    protected const string HMACSHA1SignatureType = "HMAC-SHA1";
    /// <summary>
    /// The plain text signature type name.
    /// </summary>
    protected const string PlainTextSignatureType = "PLAINTEXT";
    /// <summary>
    /// The RSA-SHA1 signature type name.
    /// </summary>
    protected const string RSASHA1SignatureType = "RSA-SHA1";

    /// <summary>
    /// The random source used to generate nonces.
    /// </summary>
    protected Random random = new Random();

    /// <summary>
    /// The set of unreserved URL characters.
    /// </summary>
    protected string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

    /// <summary>
    /// Helper function to compute a hash value
    /// </summary>
    /// <param name="hashAlgorithm">The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function</param>
    /// <param name="data">The data to hash</param>
    /// <returns>a Base64 string of the hash value</returns>
    private string ComputeHash(HashAlgorithm hashAlgorithm, string data)
    {
        if (hashAlgorithm is null)
        {
            throw new ArgumentNullException(nameof(hashAlgorithm));
        }

        if (String.IsNullOrEmpty(data))
        {
            throw new ArgumentNullException(nameof(data));
        }

        byte[] dataBuffer = Encoding.ASCII.GetBytes(data);
        byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")
    /// </summary>
    /// <param name="parameters">The query string part of the Url</param>
    /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
    private List<QueryParameter> GetQueryParameters(string parameters)
    {
        if (parameters.StartsWith("?"))
        {
            parameters = parameters.Remove(0, 1);
        }

        List<QueryParameter> result = new List<QueryParameter>();

        if (!String.IsNullOrEmpty(parameters))
        {
            string[] p = parameters.Split('&');
            foreach (string s in p)
            {
                if (!String.IsNullOrEmpty(s) && !s.StartsWith(OAuthParameterPrefix))
                {
                    if (s.IndexOf('=') > -1)
                    {
                        string[] temp = s.Split('=');
                        result.Add(new QueryParameter(temp[0], temp[1]));
                    }
                    else
                    {
                        result.Add(new QueryParameter(s, String.Empty));
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
    /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
    /// </summary>
    /// <param name="value">The value to Url encode</param>
    /// <returns>Returns a Url encoded string</returns>
    protected string UrlEncode(string value)
    {
        StringBuilder result = new StringBuilder();

        foreach (char symbol in value)
        {
            if (unreservedChars.IndexOf(symbol) != -1)
            {
                result.Append(symbol);
            }
            else
            {
                result.Append('%' + $"{(int)symbol:X2}");
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Normalizes the request parameters according to the spec
    /// </summary>
    /// <param name="parameters">The list of parameters already sorted</param>
    /// <returns>a string representing the normalized parameters</returns>
    protected string NormalizeRequestParameters(IList<QueryParameter> parameters)
    {
        StringBuilder sb = new StringBuilder();
        QueryParameter? p = null;
        for (int i = 0; i < parameters.Count; i++)
        {
            p = parameters[i];
            sb.AppendFormat("{0}={1}", p.Name, p.Value);

            if (i < parameters.Count - 1)
            {
                sb.Append("&");
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Generate the signature base that is used to produce the signature
    /// </summary>
    /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
    /// <param name="consumerKey">The consumer key</param>
    /// <param name="token">The token, if available. If not available pass null or an empty string</param>
    /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
    /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
    /// <param name="timeStamp">The OAuth timestamp.</param>
    /// <param name="nonce">The OAuth nonce.</param>
    /// <param name="signatureType">The signature type name.</param>
    /// <param name="normalizedUrl">The normalized URL produced for the request.</param>
    /// <param name="normalizedRequestParameters">The normalized request parameters produced for the request.</param>
    /// <returns>The signature base</returns>
    public string GenerateSignatureBase(Uri url, string consumerKey, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, string signatureType, out string normalizedUrl, out string normalizedRequestParameters)
    {
        if (token is null)
        {
            token = String.Empty;
        }

        if (tokenSecret is null)
        {
            tokenSecret = String.Empty;
        }

        if (String.IsNullOrEmpty(consumerKey))
        {
            throw new ArgumentNullException(nameof(consumerKey));
        }

        if (String.IsNullOrEmpty(httpMethod))
        {
            throw new ArgumentNullException(nameof(httpMethod));
        }

        if (String.IsNullOrEmpty(signatureType))
        {
            throw new ArgumentNullException(nameof(signatureType));
        }

        normalizedUrl = null!;
        normalizedRequestParameters = null!;

        List<QueryParameter> parameters = GetQueryParameters(url.Query);
        parameters.Add(new QueryParameter(OAuthVersionKey, OAuthVersion));
        parameters.Add(new QueryParameter(OAuthNonceKey, nonce));
        parameters.Add(new QueryParameter(OAuthTimestampKey, timeStamp));
        parameters.Add(new QueryParameter(OAuthSignatureMethodKey, signatureType));
        parameters.Add(new QueryParameter(OAuthConsumerKeyKey, consumerKey));

        if (!String.IsNullOrEmpty(token))
        {
            parameters.Add(new QueryParameter(OAuthTokenKey, token));
        }

        parameters.Sort(new QueryParameterComparer());

        normalizedUrl = $"{url.Scheme}://{url.Host}";
        if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
        {
            normalizedUrl += ":" + url.Port;
        }
        normalizedUrl += url.AbsolutePath;
        normalizedRequestParameters = NormalizeRequestParameters(parameters);

        StringBuilder signatureBase = new StringBuilder();
        signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
        signatureBase.AppendFormat("{0}&", UrlEncode(normalizedUrl));
        signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

        return signatureBase.ToString();
    }

    /// <summary>
    /// Generate the signature value based on the given signature base and hash algorithm
    /// </summary>
    /// <param name="signatureBase">The signature based as produced by the GenerateSignatureBase method or by any other means</param>
    /// <param name="hash">The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or a key it should be set prior to calling this method</param>
    /// <returns>A base64 string of the hash value</returns>
    public string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
    {
        return ComputeHash(hash, signatureBase);
    }

    /// <summary>
    /// Generates a signature using the HMAC-SHA1 algorithm
    /// </summary>
    /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
    /// <param name="consumerKey">The consumer key</param>
    /// <param name="consumerSecret">The consumer seceret</param>
    /// <param name="token">The token, if available. If not available pass null or an empty string</param>
    /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
    /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
    /// <param name="timeStamp">The OAuth timestamp.</param>
    /// <param name="nonce">The OAuth nonce.</param>
    /// <param name="normalizedUrl">The normalized URL produced for the request.</param>
    /// <param name="normalizedRequestParameters">The normalized request parameters produced for the request.</param>
    /// <returns>A base64 string of the hash value</returns>
    public string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, out string normalizedUrl, out string normalizedRequestParameters)
    {
        return GenerateSignature(url, consumerKey, consumerSecret, token, tokenSecret, httpMethod, timeStamp, nonce, SignatureTypes.HMACSHA1, out normalizedUrl, out normalizedRequestParameters);
    }

    /// <summary>
    /// Generates a signature using the specified signatureType
    /// </summary>
    /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
    /// <param name="consumerKey">The consumer key</param>
    /// <param name="consumerSecret">The consumer seceret</param>
    /// <param name="token">The token, if available. If not available pass null or an empty string</param>
    /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
    /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
    /// <param name="timeStamp">The OAuth timestamp.</param>
    /// <param name="nonce">The OAuth nonce.</param>
    /// <param name="signatureType">The type of signature to use</param>
    /// <param name="normalizedUrl">The normalized URL produced for the request.</param>
    /// <param name="normalizedRequestParameters">The normalized request parameters produced for the request.</param>
    /// <returns>A base64 string of the hash value</returns>
    public string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, SignatureTypes signatureType, out string normalizedUrl, out string normalizedRequestParameters)
    {
        normalizedUrl = null!;
        normalizedRequestParameters = null!;

        switch (signatureType)
        {
            case SignatureTypes.PLAINTEXT:
                return WebUtility.UrlEncode($"{consumerSecret}&{tokenSecret}")!;
            case SignatureTypes.HMACSHA1:
                string signatureBase = GenerateSignatureBase(url, consumerKey, token, tokenSecret, httpMethod, timeStamp, nonce, HMACSHA1SignatureType, out normalizedUrl, out normalizedRequestParameters);

                HMACSHA1 hmacsha1 = new HMACSHA1();
                hmacsha1.Key = Encoding.ASCII.GetBytes(
                    $"{UrlEncode(consumerSecret)}&{(String.IsNullOrEmpty(tokenSecret) ? "" : UrlEncode(tokenSecret))}");

                return GenerateSignatureUsingHash(signatureBase, hmacsha1);
            case SignatureTypes.RSASHA1:
                throw new NotImplementedException();
            default:
                throw new ArgumentException("Unknown signature type", "signatureType");
        }
    }

    /// <summary>
    /// Generate the timestamp for the signature
    /// </summary>
    /// <returns>The current Unix timestamp.</returns>
    public virtual string GenerateTimeStamp()
    {
        // Default implementation of UNIX time of the current UTC time
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }

    /// <summary>
    /// Generate a nonce
    /// </summary>
    /// <returns>A random nonce value.</returns>
    public virtual string GenerateNonce()
    {
        // Just a simple implementation of a random number between 123400 and 9999999
        return random.Next(123400, 9999999).ToString();
    }

}
