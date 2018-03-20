using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker.Controls.Error;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Utility;
using Hearthstone_Deck_Tracker.Utility.Analytics;
using Hearthstone_Deck_Tracker.Utility.Extensions;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Hearthstone_Deck_Tracker.Utility.Toasts;
using HearthSim.Core.HSReplay;

namespace Hearthstone_Deck_Tracker.HsReplay
{
	internal static class HSReplayNetHelper
	{
		static HSReplayNetHelper()
		{
			//ConfigWrapper.CollectionSyncingChanged += () => SyncCollection().Forget();
			Core.HSReplay.OAuth.CollectionUpdated += () =>
			{
				ToastManager.ShowCollectionUpdatedToast();
				Influx.OnCollectionSynced(true);
			};
			Core.HSReplay.Events.CollectionUploadError += args => Influx.OnCollectionSynced(false);
			Core.HSReplay.Events.BlizzardAccountClaimed += args => Influx.OnBlizzardAccountClaimed(true);
			Core.HSReplay.Events.BlizzardAccountClaimError += args => Influx.OnBlizzardAccountClaimed(false);
			Core.HSReplay.OAuth.AuthenticationError += Influx.OnOAuthLoginComplete;
			Core.HSReplay.OAuth.Authenticating += authenticating =>
			{
				if(authenticating)
					Influx.OnOAuthLoginInitiated();
			};
			Core.HSReplay.OAuth.LoggedOut += Influx.OnOAuthLogout;
			Core.HSReplay.OAuth.Authenticated += () => Influx.OnOAuthLoginComplete(AuthenticationErrorType.None);
		}

		public static void OpenDecksUrlWithCollection(string campaign)
		{
			var query = new List<string>();
			if(CollectionHelper.TryGetCollection(out var collection) && collection != null)
			{
				var region = Helper.GetRegion(collection.AccountHi);
				query.Add($"hearthstone_account={(int)region}-{collection.AccountLo}");
			}
			Helper.TryOpenUrl(Helper.BuildHsReplayNetUrl("decks", campaign, query, new[] { "maxDustCost=0" }));
		}
	}
}
