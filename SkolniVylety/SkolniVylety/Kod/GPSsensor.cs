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

		public static async Task<Position> Pozice()
		{
			Position position = null;
			try
			{
				var locator = CrossGeolocator.Current;
				locator.DesiredAccuracy = 5;

				if (!locator.IsGeolocationAvailable || !locator.IsGeolocationEnabled)
					return null;

				position = await locator.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);

			}
			catch
			{
				return null;
			}

			return position;
		}
	}
}
