using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using Cirrious.Conference.Core.Interfaces;
using Cirrious.Conference.Core.Models.Raw;
using Cirrious.CrossCore.Core;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Plugins.File;
using Cirrious.MvvmCross.Plugins.Messenger;

using Newtonsoft.Json;
using RestSharp.Portable;
using System.Net.Http;
using System.Threading;

namespace Cirrious.Conference.Core.Models
{
	public class ConferenceService 
        : IConferenceService
	{
		private readonly FavoritesSaver _favoritesSaver = new FavoritesSaver ();

		// is loading setup
		private bool _isLoading;

		public bool IsLoading {
			get { return _isLoading; }
			private set { 
				_isLoading = value;
				FireLoadingChanged ();
			}
		}

		private void FireLoadingChanged ()
		{
			FireMessage (new LoadingChangedMessage (this));
		}
 
		// the basic lists
		public IDictionary<string, SessionWithFavoriteFlag> Sessions { get; private set; }

		public IDictionary<string, Sponsor> Exhibitors { get; private set; }

		public IDictionary<string, Sponsor> Sponsors { get; private set; }

		// a hashtable of favorites
		private IDictionary<string, SessionWithFavoriteFlag> _favoriteSessions;

		public IDictionary<string,SessionWithFavoriteFlag> GetCopyOfFavoriteSessions ()
		{
			lock (this) {
				if (_favoriteSessions == null)
					return new Dictionary<string, SessionWithFavoriteFlag> ();
				
				var toReturn = new Dictionary<string, SessionWithFavoriteFlag> (_favoriteSessions);
				return toReturn;
			}
		}

		private void FireFavoriteSessionsChanged ()
		{
			FireMessage (new FavoritesChangedMessage (this));
		}

		private void FireMessage (MvxMessage message)
		{
			var messenger = Mvx.Resolve<IMvxMessenger> ();
			messenger.Publish (message);
		}

		public void BeginAsyncLoad ()
		{
			IsLoading = true;
			MvxAsyncDispatcher.BeginAsync (Load);
		}

		public void DoSyncLoad ()
		{
			IsLoading = true;
			Load ();
		}

		private void Load ()
		{
			LoadSessions2 ();
			LoadFavorites ();
			LoadSponsors ();

			IsLoading = false;
		}

		private void LoadSponsors ()
		{
			var file = Mvx.Resolve<IMvxResourceLoader> ().GetTextResource ("ConfResources/Sponsors.txt");
			var jsonConvert = Mvx.Resolve<IMvxJsonConverter> ();
			var items = jsonConvert.DeserializeObject<List<Sponsor>> (file);
			Sponsors = items.Where (x => x.Level != "Exhibitor").ToDictionary (x => x.Name);
			Exhibitors = items.Where (x => x.Level == "Exhibitor").ToDictionary (x => x.Name);
		}

		private void LoadFavorites ()
		{
			lock (this) {
				_favoriteSessions = new Dictionary<string, SessionWithFavoriteFlag> ();
			}
			FireFavoriteSessionsChanged ();

			var files = Mvx.Resolve<IMvxFileStore> ();
			string json;
			if (!files.TryReadTextFile (Constants.FavoritesFileName, out json))
				return;

			var jsonConvert = Mvx.Resolve<IMvxJsonConverter> ();
			var parsedKeys = jsonConvert.DeserializeObject<List<string>> (json);
			if (parsedKeys != null) {
				foreach (var key in parsedKeys) {
					SessionWithFavoriteFlag session;
					if (Sessions.TryGetValue (key, out session))
						session.IsFavorite = true;
				}
			}
		}

		private void LoadSessions ()
		{
			var file = Mvx.Resolve<IMvxResourceLoader> ().GetTextResource ("ConfResources/Sessions.txt");
			var jsonConvert = Mvx.Resolve<IMvxJsonConverter> ();
			var items = jsonConvert.DeserializeObject<List<Session>> (file);
			//foreach (var item in items)
			//{
			//    item.Key = item.Key;
			//}
			Sessions = items.Select (x => new SessionWithFavoriteFlag () {
				Session = x,
				IsFavorite = false
			})
                .ToDictionary (x => x.Session.Key, x => x);

			foreach (var sessionWithFavoriteFlag in Sessions.Values) {
				sessionWithFavoriteFlag.PropertyChanged += SessionWithFavoriteFlagOnPropertyChanged;            
			}
		}

		private async void LoadSessions2 ()
		{
			string contents;

			var files = Mvx.Resolve<IMvxFileStore> ();
			files.TryReadTextFile ("RESTDATA.json", out contents);

			var loaded = false;

			using (var client = new RestClient (new Uri (@"https://ciafadmin.herokuapp.com/api/"))) {
				var request = new RestRequest ("schedule", HttpMethod.Get);
				var result = await client.Execute<SessionData[]> (request);
		
				// do something with the response.StatusCode and response.Stream
				if (result.StatusCode == System.Net.HttpStatusCode.OK) {
					//files.WriteFile ("RESTDATA.json", response..);

					this.ParseSponsors (result.Data);

					this.ParseExhibitors (result.Data);

					this.ParseSessions (result.Data);

					loaded = true;
				}
			}

			while (!loaded) {
				
			}

			//PocketConferenceModel conferenceModel;
			//if (!TryLoadSessionsFromStorage(out conferenceModel))
			//{
			//    conferenceModel = LoadSessionsFromResources();
			//}
			//LoadSessionsFromPocketConferenceModel(conferenceModel);
		}

		private void SessionWithFavoriteFlagOnPropertyChanged (object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			if (propertyChangedEventArgs.PropertyName != "IsFavorite")
				return;

			var session = (SessionWithFavoriteFlag)sender;
			lock (this) {
				if (_favoriteSessions == null)
					return;

				if (session.IsFavorite) {
					_favoriteSessions [session.Session.Key] = session;
				} else {
					if (_favoriteSessions.ContainsKey (session.Session.Key))
						_favoriteSessions.Remove (session.Session.Key);
				}

				_favoritesSaver.RequestAsyncSave (_favoriteSessions.Keys.ToList ());
			}

			FireFavoriteSessionsChanged ();
		}

		public string DayFromInt (int numberDay)
		{
			string day;
			switch (numberDay) {
			case 1:
				day = "Friday";
				break;
			case 2:
				day = "Saturday";
				break;
			case 3:
			default:
				day = "Sunday";
				break;
			}
			return day;
		}

		public int DayToInt (string dayName)
		{
			int day;
			switch (dayName) {
			case "Friday":
				day = 1;
				break;
			case "Saturday":
				day = 2;
				break;
			case "Sunday":
			default:
				day = 3;
				break;
			}
			return day;
		}

		private DateTime MakeDateTime(string day, string time)
		{
			DateTime d = new DateTime (DateTime.Now.Year, 1, 1)
				.AddDays (210 + DayToInt (day));

			TimeSpan ts = TimeSpan.Parse (time); //06:12:00

			return d.Add(ts);
		}

		private void ParseSessions (SessionData[] data)
		{
			Sessions = new Dictionary<string, SessionWithFavoriteFlag> ();
			var i = 0;
			foreach (var sessionData in data) {
				var session = new Session {

					Key = sessionData._id,
					Day = DayToInt (sessionData.day),

					When = MakeDateTime(sessionData.day,sessionData.start),
					Until = MakeDateTime(sessionData.day, sessionData.end),
	
					Description = sessionData.description,
					SpeakerKey = sessionData.name,
					Type = sessionData.type,
					Level = "0",
					Where = sessionData.location

				};

				Sessions.Add (session.Key, new SessionWithFavoriteFlag { Session = session });
				i++;
			}
		}

		private void ParseExhibitors (SessionData[] data)
		{
			Exhibitors = new Dictionary<string, Sponsor> ();
			var i = 0;
			foreach (var location in data.Select(sessionData => sessionData.location).Where(s => !string.IsNullOrEmpty(s)).Distinct()) {
				if (!Exhibitors.ContainsKey (location)) {
					Exhibitors.Add (location,
						new Sponsor {
							Description = location,
							//DisplayOrder = i,
							Level = "Premium",
							Name = location,
							Image = "",
							Url = ""
						});
				}
				i++;
			}
		}

		private void ParseSponsors (SessionData[] data)
		{
			Sponsors = new Dictionary<string, Sponsor> ();
			var i = 0;
			foreach (var sessionData in data.Where(sessionData => !string.IsNullOrEmpty(sessionData.name))) {
				if (!Sponsors.ContainsKey (sessionData.name)) {
					Sponsors.Add (sessionData.name,
						new Sponsor {
							Description = sessionData.description,
							//DisplayOrder = i,
							Level = "Premium",
							Name = sessionData.name,
							Image = "",
							Url = ""
						});
				}
				i++;
			}
		}
	}
}
