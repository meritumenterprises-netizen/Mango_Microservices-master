using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Xango.Services.Interfaces;

namespace Xango.Services.Server.Utility
{
	public static class ControllerBaseExtensions
	{
		public static void SetClientToken(this ControllerBase controller, ISetToken client, ITokenProvider tokenProvider)
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
