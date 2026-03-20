﻿namespace Geocoding.Microsoft;

 /// <summary>
 /// Represents the authentication status returned by the Bing Maps service.
 /// </summary>
 public enum AuthenticationResultCode
 {
	 /// <summary>The None value.</summary>
	 None,
	 /// <summary>The NoCredentials value.</summary>
	 NoCredentials,
	 /// <summary>The ValidCredentials value.</summary>
	 ValidCredentials,
	 /// <summary>The InvalidCredentials value.</summary>
	 InvalidCredentials,
	 /// <summary>The CredentialsExpired value.</summary>
	 CredentialsExpired,
	 /// <summary>The NotAuthorized value.</summary>
	 NotAuthorized,
 }