using Moq;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using ProjectThoiTrang.Service;
using System.Collections.Generic;

public class VnPayServiceTest
{
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<HttpContext> _contextMock;
    private readonly VnPayService _vnPayService;

    public VnPayServiceTest()
    {
        _configMock = new Mock<IConfiguration>();
        _contextMock = new Mock<HttpContext>();
        _vnPayService = new VnPayService(_configMock.Object);
    }

    [Fact]
    public void CreatePaymentUrl_ShouldReturnCorrectUrl()
    {
        // Thiết lập các giá trị cấu hình
        _configMock.Setup(config => config["VnPay:Version"]).Returns("2.0");
        _configMock.Setup(config => config["VnPay:Command"]).Returns("pay");
        _configMock.Setup(config => config["VnPay:TmnCode"]).Returns("TMNCODE");
        _configMock.Setup(config => config["VnPay:CurrCode"]).Returns("VND");
        _configMock.Setup(config => config["VnPay:Locate"]).Returns("vn");
        _configMock.Setup(config => config["VnPay:PaymentBackReturnUrl"]).Returns("https://return.url");
        _configMock.Setup(config => config["VnPay:BaseUrl"]).Returns("https://pay.vnpay.vn");
        _configMock.Setup(config => config["VnPay:HashSecret"]).Returns("SECRET");

        // Tạo một VnPaymentRequestModel với dữ liệu kiểm thử
        var model = new VnPaymentRequestModel
        {
            Amount = 100000,
            CreatedDate = DateTime.Now,
            OrderId = "12345"
        };

        // Mock phương thức GetIpAddress
        _contextMock.Setup(context => Utils.GetIpAddress(It.IsAny<HttpContext>())).Returns("127.0.0.1");

        // Gọi phương thức CreatePaymentUrl
        var result = _vnPayService.CreatePaymentUrl(_contextMock.Object, model);

        // Kiểm tra URL trả về có đúng như mong đợi không
        Assert.Contains("vnp_Version=2.0", result);
        Assert.Contains("vnp_Command=pay", result);
        Assert.Contains("vnp_TmnCode=TMNCODE", result);
        Assert.Contains("vnp_Amount=10000000", result); // 100000 * 100
        Assert.Contains("vnp_CurrCode=VND", result);
        Assert.Contains("vnp_IpAddr=127.0.0.1", result);
        Assert.Contains("vnp_Locale=vn", result);
        Assert.Contains("vnp_OrderInfo=Thanh toan don hang:12345", result);
        Assert.Contains("vnp_ReturnUrl=https://return.url", result);
    }

    [Fact]
    public void PaymentExecute_ShouldReturnCorrectResponse()
    {
        // Thiết lập các giá trị cấu hình
        _configMock.Setup(config => config["VnPay:HashSecret"]).Returns("SECRET");

        // Tạo một IQueryCollection với dữ liệu kiểm thử
        var queryCollection = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            { "vnp_TxnRef", "12345" },
            { "vnp_TransactionNo", "67890" },
            { "vnp_SecureHash", "VALID_HASH" },
            { "vnp_ResponseCode", "00" },
            { "vnp_OrderInfo", "Test Order" },
            { "vnp_Amount", "1000000" } // 10000 * 100
        });

        // Mock phương thức ValidateSignature
        var vnpayMock = new Mock<VnPayLibrary>();
        vnpayMock.Setup(v => v.ValidateSignature("VALID_HASH", "SECRET")).Returns(true);

        // Gọi phương thức PaymentExecute
        var result = _vnPayService.PaymentExecute(queryCollection);

        // Kiểm tra kết quả trả về có đúng như mong đợi không
        Assert.True(result.Success);
        Assert.Equal("VnPay", result.PaymentMethod);
        Assert.Equal("Test Order", result.OrderDescription);
        Assert.Equal("12345", result.OrderId);
        Assert.Equal("67890", result.TransactionId);
        Assert.Equal("VALID_HASH", result.Token);
        Assert.Equal(10000, result.Amount); // 1000000 / 100
        Assert.Equal("00", result.VnPayResponseCode);
    }

    [Fact]
    public void PaymentExecute_ShouldReturnSuccessResponse()
    {
        // Arrange
        var collections = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            { "vnp_TxnRef", "12345" },
            { "vnp_TransactionNo", "67890" },
            { "vnp_SecureHash", "securehash" },
            { "vnp_ResponseCode", "00" },
            { "vnp_OrderInfo", "Order Info" },
            { "vnp_Amount", "100000" }
        });

        _configMock.SetupGet(x => x["VnPay:HashSecret"]).Returns("secret");

        // Act
        var result = _vnPayService.PaymentExecute(collections);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("VnPay", result.PaymentMethod);
        Assert.Equal("Order Info", result.OrderDescription);
        Assert.Equal("12345", result.OrderId);
        Assert.Equal("67890", result.TransactionId);
        Assert.Equal("securehash", result.Token);
        Assert.Equal(1000, result.Amount);
        Assert.Equal("00", result.VnPayResponseCode);
    }

    [Fact]
    public void PaymentExecute_ShouldReturnFailureResponse_WhenSignatureInvalid()
    {
        // Arrange
        var collections = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
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
        Assert.NotNull(result);
        Assert.False(result.Success);
    }
}