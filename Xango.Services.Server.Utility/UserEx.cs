using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Xango.Services.Server.Utility
{
	public static class UserEx
	{
		public static string GetUserEmail(this ClaimsPrincipal user)
		{
			return user.Claims.Where((claim) => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").First().Value;
		}
		public static string GetUserId(this ClaimsPrincipal user)
		{
			return user.Claims.Where(u => u.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").First().Value;
		}
	}
}
