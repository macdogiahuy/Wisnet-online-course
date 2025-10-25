using CourseHub.API.Controllers.PaymentControllers;
using CourseHub.API.Helpers.AppStart;
using CourseHub.API.Services.AppInfo;
using CourseHub.API.Services.External.Payment;
using CourseHub.Core.Helpers.Messaging;
using CourseHub.Core.Helpers.Messaging.Messages;
using CourseHub.Core.Models.Course.CourseModels;
using CourseHub.Core.RequestDtos.Payment.BillDtos;
using CourseHub.Core.Services.Domain.CourseServices.Contracts;
using CourseHub.Core.Services.Domain.PaymentServices.Contracts;
using CourseHub.Core.Services.Domain.PaymentServices.TempModels;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using System.Collections.Generic;
using Xunit;

namespace CourseHub.Tests.API.Payments;

public class BillsControllerTests
{
    static BillsControllerTests()
    {
        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["External:Payment:VNPay:TmnCode"] = "TESTTMNCODE",
            ["External:Payment:VNPay:HashSecret"] = "TESTSECRET",
            ["External:Payment:VNPay:Url"] = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"
        });
        Configurer.Init(configuration);
    }

    private static BillsController CreateControllerWithUser(Guid clientId)
    {
        var controller = new BillsController();
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, clientId.ToString()) }, "TestAuth"))
        };
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        return controller;
    }

    [Fact(DisplayName = "GetRedirectLink trả về URL thanh toán khi dữ liệu hợp lệ")]
    public async Task GetRedirectLink_Should_ReturnPaymentUrl_When_CourseValid()
    {
        var courseService = new Mock<ICourseService>();
        var controller = CreateControllerWithUser(Guid.NewGuid());
        var courseId = Guid.NewGuid();
        var dto = new CreateBillDto
        {
            Action = PaymentDomainMessages.ACTION_PAY_COURSE,
            Note = courseId.ToString(),
            Gateway = PaymentDomainMessages.GATEWAY_VNPAY
        };
        var courseMin = new CourseMinModel
        {
            Id = courseId,
            Title = "Sample",
            Price = 200,
            Discount = 0.1,
            DiscountExpiry = DateTime.UtcNow.AddDays(5)
        };
        courseService
            .Setup(s => s.GetMinAsync(courseId))
            .ReturnsAsync(new ServiceResult<CourseMinModel>(200, courseMin));

        var appInfo = Options.Create(new AppInfoOptions
        {
            AppName = "WisNet",
            MainBackendApp = "https://backend.local",
            MainFrontendApp = "https://frontend.local"
        });

        var actionResult = await controller.GetRedirectLink(dto, courseService.Object, appInfo);
        var okResult = actionResult as OkObjectResult;

        okResult.Should().NotBeNull();
        var url = okResult!.Value as string;
        url.Should().NotBeNull();
        url!.Should().StartWith("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html");
        url.Should().Contain("vnp_Amount=18000");
        courseService.Verify(s => s.GetMinAsync(courseId), Times.Once);
    }

    [Fact(DisplayName = "GetRedirectLink trả về BadRequest khi note không hợp lệ")]
    public async Task GetRedirectLink_Should_ReturnBadRequest_When_NoteInvalid()
    {
        var courseService = new Mock<ICourseService>();
        var controller = CreateControllerWithUser(Guid.NewGuid());
        var dto = new CreateBillDto
        {
            Action = PaymentDomainMessages.ACTION_PAY_COURSE,
            Note = "not-a-guid",
            Gateway = PaymentDomainMessages.GATEWAY_VNPAY
        };
        var appInfo = Options.Create(new AppInfoOptions
        {
            MainBackendApp = "https://backend.local",
            MainFrontendApp = "https://frontend.local",
            AppName = "WisNet"
        });

        var result = await controller.GetRedirectLink(dto, courseService.Object, appInfo);

        result.Should().BeOfType<BadRequestObjectResult>();
        courseService.Verify(s => s.GetMinAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact(DisplayName = "GetRedirectLink trả về BadRequest khi action không hỗ trợ")]
    public async Task GetRedirectLink_Should_ReturnBadRequest_When_ActionUnsupported()
    {
        var courseService = new Mock<ICourseService>();
        var controller = CreateControllerWithUser(Guid.NewGuid());
        var dto = new CreateBillDto
        {
            Action = "UNKNOWN_ACTION",
            Note = Guid.NewGuid().ToString(),
            Gateway = PaymentDomainMessages.GATEWAY_VNPAY
        };
        var appInfo = Options.Create(new AppInfoOptions
        {
            MainBackendApp = "https://backend.local",
            MainFrontendApp = "https://frontend.local",
            AppName = "WisNet"
        });

        var result = await controller.GetRedirectLink(dto, courseService.Object, appInfo);

        result.Should().BeOfType<BadRequestObjectResult>();
        courseService.Verify(s => s.GetMinAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact(DisplayName = "GetRedirectLink trả về BadRequest khi không lấy được thông tin khóa học")]
    public async Task GetRedirectLink_Should_ReturnBadRequest_When_CourseNotFound()
    {
        var courseService = new Mock<ICourseService>();
        var controller = CreateControllerWithUser(Guid.NewGuid());
        var courseId = Guid.NewGuid();
        var dto = new CreateBillDto
        {
            Action = PaymentDomainMessages.ACTION_PAY_COURSE,
            Note = courseId.ToString(),
            Gateway = PaymentDomainMessages.GATEWAY_VNPAY
        };
        courseService
            .Setup(s => s.GetMinAsync(courseId))
            .ReturnsAsync(new ServiceResult<CourseMinModel>(404));
        var appInfo = Options.Create(new AppInfoOptions
        {
            MainBackendApp = "https://backend.local",
            MainFrontendApp = "https://frontend.local",
            AppName = "WisNet"
        });

        var result = await controller.GetRedirectLink(dto, courseService.Object, appInfo);

        result.Should().BeOfType<BadRequestObjectResult>();
        courseService.Verify(s => s.GetMinAsync(courseId), Times.Once);
    }

    [Fact(DisplayName = "RedirectedFromVNPay gọi service và redirect về trang chi tiết khi thành công")]
    public async Task RedirectedFromVNPay_Should_ProcessPayment_When_ResponseValid()
    {
        var clientId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var controller = CreateControllerWithUser(clientId);

        var billService = new Mock<IBillService>();
        billService
            .Setup(s => s.Create(It.IsAny<Guid>(), It.IsAny<CreateBillDto>(), It.IsAny<PaymentResponse>(), clientId))
            .ReturnsAsync(new ServiceResult<Guid>(201));

        var enrollmentService = new Mock<IEnrollmentService>();
        enrollmentService
            .Setup(s => s.Enroll(courseId, clientId, It.IsAny<Guid>()))
            .ReturnsAsync(new ServiceResult(200));
        enrollmentService.Setup(s => s.ForceCommitAsync()).Returns(Task.CompletedTask);

        var appInfo = Options.Create(new AppInfoOptions
        {
            MainFrontendApp = "https://frontend.local",
            MainBackendApp = "https://backend.local",
            AppName = "WisNet"
        });

        var response = new VNPayHelper.VNPayResponse
        {
            vnp_OrderInfo = $"{clientId} payment for course {courseId}",
            vnp_BankTranNo = "BANK123",
            vnp_Amount = 18000,
            vnp_TransactionNo = "TXN",
            vnp_TransactionStatus = "00",
            vnp_ResponseCode = "00",
            vnp_TmnCode = "TESTTMNCODE",
            vnp_TxnRef = "TRX",
            vnp_CardType = "VISA",
            vnp_PayDate = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
        };

        var actionResult = await controller.RedirectedFromVNPay(response, billService.Object, enrollmentService.Object, appInfo);
        var redirectResult = actionResult as RedirectResult;

        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be($"https://frontend.local/Course/Detail?id={courseId}");
        billService.Verify(s => s.Create(It.IsAny<Guid>(), It.IsAny<CreateBillDto>(), It.IsAny<PaymentResponse>(), clientId), Times.Once);
        enrollmentService.Verify(s => s.Enroll(courseId, clientId, It.IsAny<Guid>()), Times.Once);
        enrollmentService.Verify(s => s.ForceCommitAsync(), Times.Once);
    }

    [Fact(DisplayName = "RedirectedFromVNPay trả về trang lỗi khi thiếu BankTranNo")]
    public async Task RedirectedFromVNPay_Should_RedirectToFailed_When_MissingBankTranNo()
    {
        var clientId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var controller = CreateControllerWithUser(clientId);

        var billService = new Mock<IBillService>();
        var enrollmentService = new Mock<IEnrollmentService>();
        var appInfo = Options.Create(new AppInfoOptions
        {
            MainFrontendApp = "https://frontend.local",
            MainBackendApp = "https://backend.local",
            AppName = "WisNet"
        });

        var response = new VNPayHelper.VNPayResponse
        {
            vnp_OrderInfo = $"{clientId} payment for course {courseId}",
            vnp_BankTranNo = null,
            vnp_Amount = 18000,
            vnp_TransactionNo = "TXN",
            vnp_TransactionStatus = "00",
            vnp_ResponseCode = "00",
            vnp_TmnCode = "TESTTMNCODE",
            vnp_TxnRef = "TRX",
            vnp_CardType = "VISA",
            vnp_PayDate = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
        };

        var actionResult = await controller.RedirectedFromVNPay(response, billService.Object, enrollmentService.Object, appInfo);
        var redirectResult = actionResult as RedirectResult;

        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be($"https://frontend.local/Payment?courseId={courseId}&failed=true");
        billService.Verify(s => s.Create(It.IsAny<Guid>(), It.IsAny<CreateBillDto>(), It.IsAny<PaymentResponse>(), It.IsAny<Guid>()), Times.Never);
        enrollmentService.Verify(s => s.Enroll(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact(DisplayName = "RedirectedFromVNPay trả về 404 khi thiếu response")]
    public async Task RedirectedFromVNPay_Should_RedirectTo404_When_ResponseMissing()
    {
        var controller = CreateControllerWithUser(Guid.NewGuid());
        var billService = new Mock<IBillService>();
        var enrollmentService = new Mock<IEnrollmentService>();
        var appInfo = Options.Create(new AppInfoOptions
        {
            MainFrontendApp = "https://frontend.local",
            MainBackendApp = "https://backend.local",
            AppName = "WisNet"
        });

        var actionResult = await controller.RedirectedFromVNPay(null, billService.Object, enrollmentService.Object, appInfo);
        var redirectResult = actionResult as RedirectResult;

        redirectResult.Should().NotBeNull();
        redirectResult!.Url.Should().Be("https://frontend.local/404");
    }
}