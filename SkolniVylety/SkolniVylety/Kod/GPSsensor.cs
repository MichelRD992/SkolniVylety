using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SkolniVylety
{
    public static class GPSsensor
    {

		public static async Task<object> Pozice()
		{
			Position position = null;
			try
			{
				var locator = CrossGeolocator.Current;
				locator.DesiredAccuracy = 5;

				if (!locator.IsGeolocationAvailable)
					return "Zjišťování polohy není k dispozici";
				else if (!locator.IsGeolocationEnabled)
					return "Zjišťování polohy není povoleno, prosím zapněte jej";

				position = await locator.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);

			}
			catch (Exception ex)
			{
				return "Nastala chyba: " + ex;
			}

			return position;
		}
	}
}
