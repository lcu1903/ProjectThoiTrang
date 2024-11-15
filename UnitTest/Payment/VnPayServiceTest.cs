using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using ProjectThoiTrang.Service;
using ProjectThoiTrang.RequestModel;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Security.Cryptography;
using System.Text;

public class VnPayServiceTest
{
    private readonly Mock<IConfiguration> _configMock;
    private readonly VnPayService _vnPayService;

    public VnPayServiceTest()
    {
        _configMock = new Mock<IConfiguration>();
        _vnPayService = new VnPayService(_configMock.Object);
    }

    private string GenerateValidSignature(Dictionary<string, string> data, string secret)
    {
        var sortedData = data.OrderBy(kv => kv.Key).ToList();
        var dataString = string.Join("&", sortedData.Select(kv => $"{kv.Key}={kv.Value}"));
        var hash = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
        var hashBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(dataString));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    [Fact]
    public void CreatePaymentUrl_ShouldReturnCorrectUrl()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var model = new VnPaymentRequestModel
        {
            Amount = 1000,
            CreatedDate = DateTime.Now,
            OrderId = "12345"
        };

        _configMock.SetupGet(x => x["VnPay:Version"]).Returns("2.0");
        _configMock.SetupGet(x => x["VnPay:Command"]).Returns("pay");
        _configMock.SetupGet(x => x["VnPay:TmnCode"]).Returns("TMNCODE");
        _configMock.SetupGet(x => x["VnPay:CurrCode"]).Returns("VND");
        _configMock.SetupGet(x => x["VnPay:Locate"]).Returns("vn");
        _configMock.SetupGet(x => x["VnPay:PaymentBackReturnUrl"]).Returns("http://return.url");
        _configMock.SetupGet(x => x["VnPay:BaseUrl"]).Returns("http://base.url");
        _configMock.SetupGet(x => x["VnPay:HashSecret"]).Returns("secret");

        // Act
        var result = _vnPayService.CreatePaymentUrl(context, model);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("vnp_Amount=100000", result);
    }

    [Fact]
    public void PaymentExecute_ShouldReturnSuccessResponse_WhenSignatureValid()
    {
        // Arrange
        var data = new Dictionary<string, string>
    {
        { "vnp_TxnRef", "12345" },
        { "vnp_TransactionNo", "67890" },
        { "vnp_ResponseCode", "00" },
        { "vnp_OrderInfo", "Order Info" },
        { "vnp_Amount", "100000" }
    };

        var secret = "secret";
        var validSignature = GenerateValidSignature(data, secret);

        var collections = new QueryCollection(new Dictionary<string, StringValues>
    {
        { "vnp_TxnRef", "12345" },
        { "vnp_TransactionNo", "67890" },
        { "vnp_SecureHash", validSignature },
        { "vnp_ResponseCode", "00" },
        { "vnp_OrderInfo", "Order Info" },
        { "vnp_Amount", "100000" }
    });

        _configMock.SetupGet(x => x["VnPay:HashSecret"]).Returns(secret);

        // Act
        var result = _vnPayService.PaymentExecute(collections);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("VnPay", result.PaymentMethod);
        Assert.Equal("Order Info", result.OrderDescription);
        Assert.Equal("12345", result.OrderId);
        Assert.Equal("67890", result.TransactionId);
        Assert.Equal(validSignature, result.Token);
        Assert.Equal(1000, result.Amount);
        Assert.Equal("00", result.VnPayResponseCode);
    }

    

    [Fact]
    public void PaymentExecute_ShouldReturnFailureResponse_WhenSignatureInvalid()
    {
        // Arrange
        var collections = new QueryCollection(new Dictionary<string, StringValues>
        {
            { "vnp_TxnRef", "12345" },
            { "vnp_TransactionNo", "67890" },
            { "vnp_SecureHash", "invalidhash" },
            { "vnp_ResponseCode", "00" },
            { "vnp_OrderInfo", "Order Info" },
            { "vnp_Amount", "100000" }
        });

        _configMock.SetupGet(x => x["VnPay:HashSecret"]).Returns("secret");

        // Act
        var result = _vnPayService.PaymentExecute(collections);

        // Assert
        Assert.False(result.Success);
    }
}