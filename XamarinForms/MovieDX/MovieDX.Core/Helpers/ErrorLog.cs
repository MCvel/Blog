using System;
using System.Diagnostics;

namespace MovieDX.Core.Helpers
{
	public static class ErrorLog
	{
		public static void LogError(string message, Exception ex)
		{
			Debug.WriteLine("{0}. {1}", message, ex.ToString());
		}

	}
}
