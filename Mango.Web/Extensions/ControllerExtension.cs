using Xango.Services.Interfaces;
using Xango.Services.Server.Utility;

namespace Xango.Web.Extensions
{
	public static class ControllerExtension
	{
		public static void SetClientToken(this Microsoft.AspNetCore.Mvc.Controller controller, ISetToken client, ITokenProvider tokenProvider)
		{
			var authHeader = controller.Request.Headers["Authorization"].ToString();
			if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
			{
				var token = authHeader.Substring("Bearer ".Length).Trim();
				client.SetToken(token);
			}
			else
			{
				client.SetToken(tokenProvider.GetToken());
			}

		}
	}
}
