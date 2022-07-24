﻿using System;

namespace MyAbilityFirst.Constants
{
	public static class CacheSetting
	{

		public static class SitemapNodes
		{
			public const string Key = "SitemapNodes";
			public static readonly TimeSpan SlidingExpiration = TimeSpan.FromDays(1);
		}

	}
}