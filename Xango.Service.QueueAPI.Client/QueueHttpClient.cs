using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xango.Models.Dto;
using Microsoft.Extensions.Configuration;
using Xango.Services.Server.Utility;
using System.Net.Http.Json;
using Xango.Services.Utility;
using Xango.Services.Client.Utility;

namespace Xango.Service.QueueAPI.Client
{
	public class QueueHttpClient : IQueueHttpClient
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;
		private readonly string _baseUri;
		private readonly ITokenProvider _tokenProvider;
		private string _token = string.Empty;

		public QueueHttpClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ITokenProvider tokenProvider)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_baseUri = Environment.GetEnvironmentVariable("QueueAPI");
			_tokenProvider = tokenProvider;
		}

		public void SetToken(string token)
		{
			this._token = token;
		}	
		public async Task<ResponseDto> PostOrderApproved(OrderHeaderDto orderHeader)
		{
			var client = _httpClientFactory.NewClientNoSslErrors("Queue");
			client.BaseAddress = new Uri(_baseUri);
			var token = _tokenProvider.GetToken();
			if (token == null)
			{
				token = this._token;
			}
			orderHeader.OrderTotalWithCurrency = orderHeader.OrderTotal.ToString("C2");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = await client.PostAsync("/api/queue/OrderApproved", StringContentUTF8.AsJsonString(orderHeader));
			response.EnsureSuccessStatusCode();
			var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
			if (resp != null && resp.IsSuccess)
			{
				return ResponseProducer.OkResponse(resp.Result);
			}
			return ResponseProducer.ErrorResponse("Could not post approved order to the queue");

		}

		public async Task<ResponseDto> PostOrderPending(OrderHeaderDto orderHeader)
		{
			var client = _httpClientFactory.NewClientNoSslErrors("Queue");
			client.BaseAddress = new Uri(_baseUri);
			var token = _tokenProvider.GetToken();
			if (token == null)
			{
				token = this._token;
			}

			orderHeader.OrderTotalWithCurrency = orderHeader.OrderTotal.ToString("C2");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = await client.PostAsync("/api/queue/OrderPending", StringContentUTF8.AsJsonString(orderHeader));
			response.EnsureSuccessStatusCode();
			var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
			if (resp != null && resp.IsSuccess)
			{
				return ResponseProducer.OkResponse(resp.Result);
			}
			return ResponseProducer.ErrorResponse("Could not post pending order to the queue");
		}

		public async Task<ResponseDto> PostOrderReadyForPickup(OrderHeaderDto orderHeader)
		{
			var client = _httpClientFactory.NewClientNoSslErrors("Queue");
			client.BaseAddress = new Uri(_baseUri);
			var token = _tokenProvider.GetToken();
			if (token == null)
			{
				token = this._token;
			}

			orderHeader.OrderTotalWithCurrency = orderHeader.OrderTotal.ToString("C2");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = await client.PostAsync("/api/queue/OrderReadyForPickup", StringContentUTF8.AsJsonString(orderHeader));
			response.EnsureSuccessStatusCode();
			var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
			if (resp != null && resp.IsSuccess)
			{
				return ResponseProducer.OkResponse(resp.Result);
			}
			return ResponseProducer.ErrorResponse("Could not post ready for pickup order to the queue");
		}

		public async Task<ResponseDto> PostOrderCancelled(OrderHeaderDto orderHeader)
		{
			var client = _httpClientFactory.NewClientNoSslErrors("Queue");
			client.BaseAddress = new Uri(_baseUri);
			var token = _tokenProvider.GetToken();
			if (token == null)
			{
				token = this._token;
			}

			orderHeader.OrderTotalWithCurrency = orderHeader.OrderTotal.ToString("C2");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = await client.PostAsync("/api/queue/OrderCancelled", StringContentUTF8.AsJsonString(orderHeader));
			response.EnsureSuccessStatusCode();
			var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
			if (resp != null && resp.IsSuccess)
			{
				return ResponseProducer.OkResponse(resp.Result);
			}
			return ResponseProducer.ErrorResponse("Could not post cancelled order to the queue");
		}

		public async Task<ResponseDto> PostOrderCompleted(OrderHeaderDto orderHeader)
		{
			var client = _httpClientFactory.NewClientNoSslErrors("Queue");
			client.BaseAddress = new Uri(_baseUri);
			var token = _tokenProvider.GetToken();
			if (token == null)
			{
				token = this._token;
			}

			orderHeader.OrderTotalWithCurrency = orderHeader.OrderTotal.ToString("C2");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = await client.PostAsync("/api/queue/OrderCompleted", StringContentUTF8.AsJsonString(orderHeader));
			response.EnsureSuccessStatusCode();
			var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
			if (resp != null && resp.IsSuccess)
			{
				return ResponseProducer.OkResponse(resp.Result);
			}
			return ResponseProducer.ErrorResponse("Could not posted completed order to the queue");
		}

		public async Task<ResponseDto> PostOrderShipped(OrderHeaderDto orderHeader)
		{
			var client = _httpClientFactory.NewClientNoSslErrors("Queue");
			client.BaseAddress = new Uri(_baseUri);
			var token = _tokenProvider.GetToken();
			if (token == null)
			{
				token = this._token;
			}

			orderHeader.OrderTotalWithCurrency = orderHeader.OrderTotal.ToString("C2");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = await client.PostAsync("/api/queue/OrderShipped", StringContentUTF8.AsJsonString(orderHeader));
			response.EnsureSuccessStatusCode();
			var resp = response.Content.ReadFromJsonAsync<ResponseDto?>().GetAwaiter().GetResult();
			if (resp != null && resp.IsSuccess)
			{
				return ResponseProducer.OkResponse(resp.Result);
			}
			return ResponseProducer.ErrorResponse("Could not posted shipped order to the queue");
		}

	}
}
