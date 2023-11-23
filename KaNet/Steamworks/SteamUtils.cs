using KaNet.Session;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KaNet.SteamworksAPI
{
	public static class SteamUtilExtension
	{
		public static async Task<Texture2D> GetTextureFromSteamIDAsync(SteamId id)
		{
			var avatarImage = await SteamFriends.GetLargeAvatarAsync(id);
			Steamworks.Data.Image image = avatarImage.Value;

			int width = (int)image.Width;
			int height = (int)image.Height;

			Texture2D texture = new Texture2D(width, height);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Steamworks.Data.Color pixel = image.GetPixel(x, y);
					texture.SetPixel(x, height - y, pixel.ToUnityColor());
				}
			}

			texture.Apply();

			return texture;
		}
	}
}
