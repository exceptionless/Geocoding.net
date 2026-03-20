﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Geocoding.MapQuest
{
	/// <summary>
	/// Provides geocoding and reverse geocoding through the MapQuest API.
	/// </summary>
	/// <remarks>
	/// See http://open.mapquestapi.com/geocoding/ and http://developer.mapquest.com/.
	/// </remarks>
	public class MapQuestGeocoder : IGeocoder, IBatchGeocoder
	{
		readonly string key;

		volatile bool useOSM;
		/// <summary>
		/// When true, will use the Open Street Map API
		/// </summary>
		public virtual bool UseOSM
		{
			get { return useOSM; }
			set { useOSM = value; }
		}

		/// <summary>
		/// Gets or sets the proxy used for MapQuest requests.
		/// </summary>
		public IWebProxy Proxy { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MapQuestGeocoder"/> class.
		/// </summary>
		/// <param name="key">The MapQuest application key.</param>
		public MapQuestGeocoder(string key)
		{
			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentException("key can not be null or blank");

			this.key = key;
		}

		IEnumerable<Address> HandleSingleResponse(MapQuestResponse res)
		{
			if (res != null && !res.Results.IsNullOrEmpty())
			{
				return HandleSingleResponse(from r in res.Results
											where r != null && !r.Locations.IsNullOrEmpty()
											from l in r.Locations
											select l);
			}
			else
				return new Address[0];
		}

		IEnumerable<Address> HandleSingleResponse(IEnumerable<MapQuestLocation> locs)
		{
			if (locs == null)
				return new Address[0];
			else
			{
				return from l in locs
					   where l != null && l.Quality < Quality.COUNTRY
					   let q = (int)l.Quality
					   let c = string.IsNullOrWhiteSpace(l.Confidence) ? "ZZZZZZ" : l.Confidence
					   orderby q ascending, c ascending
					   select l;
			}
		}

		/// <inheritdoc />
		public async Task<IEnumerable<Address>> GeocodeAsync(string address, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrWhiteSpace(address))
				throw new ArgumentException("address can not be null or empty!");

			var f = new GeocodeRequest(key, address) { UseOSM = this.UseOSM };
			MapQuestResponse res = await Execute(f, cancellationToken).ConfigureAwait(false);
			return HandleSingleResponse(res);
		}

		/// <inheritdoc />
		public Task<IEnumerable<Address>> GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken = default(CancellationToken))
		{
			var sb = new StringBuilder();
			if (!string.IsNullOrWhiteSpace(street))
				sb.AppendFormat("{0}, ", street);
			if (!string.IsNullOrWhiteSpace(city))
				sb.AppendFormat("{0}, ", city);
			if (!string.IsNullOrWhiteSpace(state))
				sb.AppendFormat("{0} ", state);
			if (!string.IsNullOrWhiteSpace(postalCode))
				sb.AppendFormat("{0} ", postalCode);
			if (!string.IsNullOrWhiteSpace(country))
				sb.AppendFormat("{0} ", country);

			if (sb.Length > 1)
				sb.Length--;

			string s = sb.ToString().Trim();
			if (string.IsNullOrWhiteSpace(s))
				throw new ArgumentException("Concatenated input values can not be null or blank");

			if (s.Last() == ',')
				s = s.Remove(s.Length - 1);

			return GeocodeAsync(s, cancellationToken);
		}

		/// <inheritdoc />
		public async Task<IEnumerable<Address>> ReverseGeocodeAsync(Location location, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (location == null)
				throw new ArgumentNullException("location");

			var f = new ReverseGeocodeRequest(key, location) { UseOSM = this.UseOSM };
			MapQuestResponse res = await Execute(f, cancellationToken).ConfigureAwait(false);
			return HandleSingleResponse(res);
		}

		/// <inheritdoc />
		public Task<IEnumerable<Address>> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default(CancellationToken))
		{
			return ReverseGeocodeAsync(new Location(latitude, longitude), cancellationToken);
		}

		/// <summary>
		/// Executes a raw MapQuest request and returns the deserialized response.
		/// </summary>
		/// <param name="f">The request to execute.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The deserialized MapQuest response.</returns>
		public async Task<MapQuestResponse> Execute(BaseRequest f, CancellationToken cancellationToken = default(CancellationToken))
		{
			HttpWebRequest request = await Send(f, cancellationToken).ConfigureAwait(false);
			MapQuestResponse r = await Parse(request, cancellationToken).ConfigureAwait(false);
			if (r != null && !r.Results.IsNullOrEmpty())
			{
				foreach (MapQuestResult o in r.Results)
				{
					if (o == null)
						continue;

					foreach (MapQuestLocation l in o.Locations)
					{
						if (!string.IsNullOrWhiteSpace(l.FormattedAddress) || o.ProvidedLocation == null)
							continue;

						if (string.Compare(o.ProvidedLocation.FormattedAddress, "unknown", true) != 0)
							l.FormattedAddress = o.ProvidedLocation.FormattedAddress;
						else
							l.FormattedAddress = o.ProvidedLocation.ToString();
					}
				}
			}
			return r;
		}

		private async Task<HttpWebRequest> Send(BaseRequest f, CancellationToken cancellationToken)
		{
			if (f == null)
				throw new ArgumentNullException("f");

			HttpWebRequest request;
			bool hasBody = false;
			switch (f.RequestVerb)
			{
				case "GET":
				case "DELETE":
				case "HEAD":
					{
						var u = string.Format("{0}json={1}&", f.RequestUri, WebUtility.UrlEncode(f.RequestBody));
						request = WebRequest.Create(u) as HttpWebRequest;
					}
					break;
				case "POST":
				case "PUT":
				default:
					{
						request = WebRequest.Create(f.RequestUri) as HttpWebRequest;
						hasBody = !string.IsNullOrWhiteSpace(f.RequestBody);
					}
					break;
			}
			request.Method = f.RequestVerb;
			request.ContentType = "application/" + f.InputFormat + "; charset=utf-8";

			if (Proxy != null)
				request.Proxy = Proxy;

			if (hasBody)
			{
				byte[] buffer = Encoding.UTF8.GetBytes(f.RequestBody);
				//request.Headers.ContentLength = buffer.Length;
				using(cancellationToken.Register(request.Abort, false))
				using (Stream rs = await request.GetRequestStreamAsync().ConfigureAwait(false))
				{
					cancellationToken.ThrowIfCancellationRequested();
					await rs.WriteAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
					await rs.FlushAsync(cancellationToken).ConfigureAwait(false);
				}
			}
			return request;
		}

		private async Task<MapQuestResponse> Parse(HttpWebRequest request, CancellationToken cancellationToken)
		{
			if (request == null)
				throw new ArgumentNullException("request");

			string requestInfo = string.Format("[{0}] {1}", request.Method, request.RequestUri);
			try
			{
				string json;
				using (HttpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as HttpWebResponse)
				{
					cancellationToken.ThrowIfCancellationRequested();
					if ((int)response.StatusCode >= 300) //error
						throw new Exception((int)response.StatusCode + " " + response.StatusDescription);

					using (var sr = new StreamReader(response.GetResponseStream()))
						json = await sr.ReadToEndAsync().ConfigureAwait(false);
				}
				if (string.IsNullOrWhiteSpace(json))
					throw new Exception("Remote system response with blank: " + requestInfo);

				MapQuestResponse o = json.FromJSON<MapQuestResponse>();
				if (o == null)
					throw new Exception("Unable to deserialize remote response: " + requestInfo + " => " + json);

				return o;
			}
			catch (WebException wex) //convert to simple exception & close the response stream
			{
				using (HttpWebResponse response = wex.Response as HttpWebResponse)
				{
					var sb = new StringBuilder(requestInfo);
					sb.Append(" | ");
					sb.Append(response.StatusDescription);
					sb.Append(" | ");
					using (var sr = new StreamReader(response.GetResponseStream()))
					{
						sb.Append(await sr.ReadToEndAsync().ConfigureAwait(false));
					}
					throw new Exception((int)response.StatusCode + " " + sb.ToString());
				}
			}
		}

		/// <inheritdoc />
		public async Task<IEnumerable<ResultItem>> GeocodeAsync(IEnumerable<string> addresses, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (addresses == null)
				throw new ArgumentNullException("addresses");

			string[] adr = (from a in addresses
							where !string.IsNullOrWhiteSpace(a)
							group a by a into ag
							select ag.Key).ToArray();
			if (adr.IsNullOrEmpty())
				throw new ArgumentException("Atleast one none blank item is required in addresses");

			var f = new BatchGeocodeRequest(key, adr) { UseOSM = this.UseOSM };
			MapQuestResponse res = await Execute(f, cancellationToken).ConfigureAwait(false);
			return HandleBatchResponse(res);
		}

		ICollection<ResultItem> HandleBatchResponse(MapQuestResponse res)
		{
			if (res != null && !res.Results.IsNullOrEmpty())
			{
				return (from r in res.Results
						where r != null && !r.Locations.IsNullOrEmpty()
						let resp = HandleSingleResponse(r.Locations)
						where resp != null
						select new ResultItem(r.ProvidedLocation, resp)).ToArray();
			}
			else
				return new ResultItem[0];
		}

		/// <inheritdoc />
		public Task<IEnumerable<ResultItem>> ReverseGeocodeAsync(IEnumerable<Location> locations, CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotSupportedException("ReverseGeocodeAsync(...) is not available for MapQuestGeocoder.");
		}
	}
}
