# CourseHub

Setup and run guide for the CourseHub solution.

## Overview

CourseHub is a .NET 8 multi-project solution that powers an online course platform.

- **CourseHub.API** – ASP.NET Core Web API that hosts the back-end services.
- **CourseHub.UI** – Razor Pages front-end that consumes the API.
- **CourseHub.Core** – Shared domain models, interfaces, and business services.
- **CourseHub.Infrastructure** – Data access layer, repositories, and EF Core integrations.
- **CourseHub.Tests** – xUnit test suite with Moq and FluentAssertions.

## Prerequisites

- .NET SDK 8.0.100 or newer (`dotnet --version`).
- SQL Server instance (LocalDB, SQL Express, or an external server).
- PowerShell 5.1+ or Windows Terminal for running CLI commands.

## Configuration

1. Copy `CourseHub.API/appsettings.json` to `appsettings.Development.json` if you want environment-specific overrides.
2. Update the key settings:
   - `ConnectionStrings` – point to your SQL Server.
   - `TargetConnectionStrings:Context` – choose the connection string key (e.g., `Local`).
   - `JwtOptions:Secret` – must be at least 32 characters (256 bits) to satisfy HS256 signing requirements.
   - `External` – configure Google OAuth, VNPay, Gmail, and other integrations if needed.
3. In `CourseHub.UI/appsettings.json`, adjust `ServicePaths:ApiServerPath` if the API is not running on `https://localhost:7277`.

> Tokens are stored in cookies. Ensure you run over HTTPS and review `CookieOptions` before deploying to production.

## Running the solution

All commands assume the working directory is the repository root (`CourseHub`).

### 1. Restore dependencies

```powershell
dotnet restore
```

### 2. Start the API

```powershell
dotnet run --project CourseHub.API/CourseHub.API.csproj
```

The API listens on `https://localhost:7277` (defined in `launchSettings.json`).

### 3. Start the UI

Open a second shell and run:

```powershell
dotnet run --project CourseHub.UI/CourseHub.UI.csproj
```

The UI listens on `https://localhost:7117`. Confirm CORS origins in the API match the UI host.

### 4. Static assets

All necessary front-end libraries (Bootstrap, jQuery, SignalR) are included in `wwwroot`; no extra npm build step is required.

## Running tests

```powershell
dotnet test CourseHub.sln --configuration Release
```

GitHub Actions (`.github/workflows/dotnet-ci.yml`) performs restore, build, and test with the .NET 8 SDK.

---

## Quy trình kiểm thử (AI4SE)

### Phân tích Feature

- Xác định module có ảnh hưởng lớn (Course, User, Payment) dựa trên lưu lượng sử dụng và rủi ro kinh doanh.
- Thu thập yêu cầu chức năng, luồng nghiệp vụ và các ràng buộc hiện có (role, trạng thái, điều kiện dữ liệu).

### Các Function Cần Test

- Lập danh sách phương thức/domain service/controller quan trọng cùng trạng thái đầu ra mong đợi.
- Ưu tiên theo tiêu chí business value, độ phức tạp logic và mức độ dễ bị lỗi.

### Thiết kế Test Cases

- Chuyển từng function thành kịch bản kiểm thử cụ thể (happy-path, negative, boundary, security).
- Ghi nhận dữ liệu giả lập, điều kiện trước, kỳ vọng sau và mã lỗi dự kiến.

### Sinh Test Code

- Hiện thực test bằng xUnit + Moq + FluentAssertions, tái sử dụng builder/helper khi có thể.
- Đảm bảo test đặt tên theo convention Given/When/Should để dễ truy vết.

### Chạy & Debug Tests

- Thực thi `dotnet test` local và CI; phân tích kết quả, log và stacktrace khi có lỗi.
- Sử dụng breakpoint hoặc `ITestOutputHelper` khi cần làm rõ hành vi.

### Tối ưu & Mocking

- Tinh chỉnh mock để chỉ phủ nhận dependency cần thiết, tránh over-mocking.
- Loại bỏ test trùng lặp, gom fixture chung, bổ sung dữ liệu giả lập sát thực tế.

## Báo cáo Coverage (Tiếng Việt)

- Chạy kiểm thử kèm báo cáo coverage:
  ```powershell
  dotnet test CourseHub.sln --configuration Release --collect:"XPlat Code Coverage"
  ```
- Tệp Cobertura được tạo tại `CourseHub.Tests/TestResults/<test-run-id>/coverage.cobertura.xml`.
- Có thể dùng `dotnet tool install -g dotnet-reportgenerator-globaltool` và
  ```powershell
  reportgenerator -reports:CourseHub.Tests/TestResults/**/coverage.cobertura.xml -targetdir:coverage-report
  ```
  để tạo báo cáo HTML để dễ quan sát hơn.

## Test Case Inventory (29 tests)

### Course Service (15 tests)

| Test ID | Test Name                                                    | Type       | Priority | Mô tả ngắn                                    |
| ------- | ------------------------------------------------------------ | ---------- | -------- | --------------------------------------------- |
| C01     | CreateAsync_Should_ReturnCreated_When_InputIsValid           | Functional | High     | Tạo khóa học hợp lệ trả về 201 và commit      |
| C02     | CreateAsync_Should_ReturnServerError_When_InstructorMissing  | Negative   | High     | Thiếu giảng viên → không insert, trả về 500   |
| C03     | CreateAsync_Should_HandleRepositoryException                 | Negative   | High     | Repository ném lỗi → ServiceResult 500        |
| C04     | UpdateAsync_Should_ReturnServerError_When_InstructorMissing  | Negative   | High     | Instructor không tồn tại khi cập nhật         |
| C05     | UpdateAsync_Should_ReturnBadRequest_When_CourseMissing       | Negative   | High     | Khóa học không tồn tại khi cập nhật           |
| C06     | UpdateAsync_Should_UpdateCourse_When_DataValid               | Functional | High     | Cập nhật thành công với dữ liệu mới và commit |
| C07     | DeleteAsync_Should_ReturnServerError_When_InstructorMissing  | Negative   | High     | Instructor không tìm thấy khi xóa             |
| C08     | DeleteAsync_Should_ReturnNotFound_When_CourseMissing         | Negative   | High     | Không tìm thấy khóa học khi xóa               |
| C09     | DeleteAsync_Should_DeleteCourse_When_EntityExists            | Functional | High     | Xóa khóa học thành công và commit             |
| C10     | GetPagedAsync_Should_OrderByPrice_When_ByPriceEnabled        | Functional | Medium   | Sắp xếp theo giá khi bật ByPrice              |
| C11     | GetPagedAsync_Should_OrderByDiscountDescending               | Functional | Medium   | Sắp xếp giảm dần theo discount                |
| C12     | GetPagedAsync_Should_OrderByLastModificationTime_When_NoFlag | Functional | Medium   | Mặc định sắp xếp theo LastModificationTime    |
| C13     | GetPagedAsync_Should_FilterByKeyword_When_TitleProvided      | Functional | Medium   | Lọc theo từ khóa Title                        |
| C14     | GetAsync_Should_ReturnCourse_When_EntityExists               | Functional | Medium   | Lấy chi tiết khóa học thành công              |
| C15     | GetAsync_Should_ReturnNotFound_When_CourseMissing            | Negative   | Medium   | Trả về 404 khi khóa học không tồn tại         |

### User Service (7 tests)

| Test ID | Test Name                                                    | Type       | Priority | Mô tả ngắn                                     |
| ------- | ------------------------------------------------------------ | ---------- | -------- | ---------------------------------------------- |
| U01     | SignInAsync_Should_ReturnAuthModel_When_CredentialsValid     | Functional | High     | Đăng nhập bằng username/password hợp lệ        |
| U02     | SignInAsync_Should_ReturnAuthModel_When_UsingEmail           | Functional | High     | Đăng nhập bằng email hợp lệ                    |
| U03     | SignInAsync_Should_ReturnUnauthorized_When_PasswordInvalid   | Negative   | High     | Sai mật khẩu tăng AccessFailedCount            |
| U04     | SignInAsync_Should_ReturnForbidden_When_UserNotApproved      | Security   | High     | Tài khoản chưa duyệt bị chặn truy cập          |
| U05     | SignInAsync_Should_ReturnForbidden_When_AccessFailedExceeded | Security   | High     | Vượt ngưỡng AccessFailedCount → khóa đăng nhập |
| U06     | SignInAsync_Should_ReturnBadRequest_When_CredentialsMissing  | Negative   | Medium   | Thiếu thông tin đăng nhập                      |
| U07     | SignInAsync_Should_ReturnUnauthorized_When_UserMissing       | Negative   | Medium   | Không tìm thấy user → trả về 401               |

### Bills Controller (7 tests)

| Test ID | Test Name                                                          | Type       | Priority | Mô tả ngắn                                              |
| ------- | ------------------------------------------------------------------ | ---------- | -------- | ------------------------------------------------------- |
| P01     | GetRedirectLink_Should_ReturnPaymentUrl_When_CourseValid           | Functional | High     | Tạo URL thanh toán VNPay hợp lệ                         |
| P02     | GetRedirectLink_Should_ReturnBadRequest_When_NoteInvalid           | Negative   | High     | Note không phải GUID trả về 400                         |
| P03     | GetRedirectLink_Should_ReturnBadRequest_When_ActionUnsupported     | Negative   | High     | Từ chối action không hỗ trợ                             |
| P04     | GetRedirectLink_Should_ReturnBadRequest_When_CourseNotFound        | Negative   | High     | Không lấy được CourseMinModel → 400                     |
| P05     | RedirectedFromVNPay_Should_ProcessPayment_When_ResponseValid       | Functional | High     | Ghi nhận bill, enroll và redirect tới chi tiết khóa học |
| P06     | RedirectedFromVNPay_Should_RedirectToFailed_When_MissingBankTranNo | Negative   | Medium   | Response thiếu BankTranNo → redirect thất bại           |
| P07     | RedirectedFromVNPay_Should_RedirectTo404_When_ResponseMissing      | Negative   | Medium   | Không có response → chuyển hướng 404                    |

## Common issues

- **IDX10720: key size** – increase `JwtOptions:Secret` to at least 32 characters.
- **jQuery `$().fn` undefined** – ensure only one jQuery/Bootstrap bundle is loaded; the layout already uses the supported set.
- **ImageSharp advisory** – monitor `SixLabors.ImageSharp` for patched versions addressing GHSA-rxmq-m78w-7wmc.
- **Nullable warnings** – many models lack nullability annotations; plan to mark members as nullable or use the `required` keyword.

## Solution structure

```
CourseHub.sln
├── CourseHub.API/
├── CourseHub.Core/
├── CourseHub.Infrastructure/
├── CourseHub.UI/
└── CourseHub.Tests/
```

## Contributing

1. Branch from `upgrade-to-NET8`.
2. Implement changes and add/update tests where necessary.
3. Run `dotnet test` before submitting.
4. Open a pull request with a clear summary of your changes.

## Licensing notes

The solution depends on third-party libraries (ASP.NET Core, EF Core, ImageSharp, etc.). Review their licenses before redistributing or deploying.

---

## Output Examples

- Here are an example outputs from the application:

```bash
[Test Session Finished] 10/25/2025 13:50:19
Total tests: 29
Passed tests: 29
Success rate: 100.00%
...
Build succeeded with 7 warning(s) in 2.3s
```

## Prompts Used

```bash
Bạn là một trợ lý AI chuyên sâu về kiểm thử phần mềm và lựa chọn chức năng.
Tôi có một repository Git với đường dẫn: “[đường_dẫn_gitrepo]”. Trong repo này có nhiều chức năng đã được triển khai nhưng **chưa có thư mục test**, tức là hiện chưa có test case nào hoặc rất ít test case.

**Yêu cầu của tôi là:**

1. Quét toàn bộ repository, xác định tất cả các **chức năng (features / modules / phương thức chính)** có trong dự án — ví dụ các API endpoints, các service, các thành phần UI, logic kinh doanh, v.v.
2. Từ đó, **chọn ra 3 chức năng tiềm năng nhất** — tiêu chí “tiềm năng” ở đây là:
   - Chức năng có vai trò quan trọng trong business/ứng dụng (high-value)
   - Chức năng có logic tương đối rõ, có thể kiểm thử được nhiều trường hợp (cover nhiều scenario)
   - Chức năng mà nếu có test case tốt sẽ đem lại lợi ích lớn cho chất lượng code (giảm lỗi, dễ maintain)
3. Sau khi liệt kê 3 ứng cử viên và lý do chọn mỗi ứng cử viên, hãy **chọn 1 trong 3** mà bạn đánh giá là “ổn nhất” để đi sâu tiếp.
4. Đối với chức năng được chọn:
   - Mô tả rõ chức năng: mục đích, inputs, outputs, logic kinh doanh, ranh giới (boundary)
   - Vì chưa có test thư mục, nên **thiết kế test case** từ đầu:
     - Tóm tắt các scenario chính cần test (bao gồm cả normal cases, boundary/edge cases, error cases)
     - Viết mã mẫu test case (theo framework test mà dự án sử dụng hoặc bạn đề xuất) — chú thích bằng tiếng Việt
     - Nếu cần, đề xuất cấu trúc thư mục “test” mới, naming convention, cách tổ chức để dễ maintain
   - Đánh giá các rủi ro nếu không test chức năng này, và lợi ích khi có test case tốt.

**Thông tin thêm**
- Repo sử dụng ngôn ngữ: C# ASP dotnet
- Nếu bạn chưa rõ framework test sẽ dùng, hãy đề xuất một framework phổ biến phù hợp.
- Mức độ ưu tiên: Tôi muốn bạn **giải thích rõ ràng từng bước**, và nếu có thể đề cập đến số liệu giả định (ví dụ: số lượng methods, độ phức tạp ước tính) thì tốt.
- Kết quả mong muốn: Tôi muốn có một bản “báo cáo” (report) với cấu trúc rõ ràng gồm:
   1. Mục lục
   2. Danh sách 3 ứng cử viên (với lý do)
   3. Chọn 1 ứng cử viên & phân tích chi tiết
   4. Thiết kế test case + mã mẫu
   5. Kết luận và đề xuất tiếp theo.

Hãy bắt đầu bằng việc **quét repository và liệt kê 3 ứng cử viên hàng đầu** — mỗi ứng cử viên là một chức năng kèm theo lý do lựa chọn. Sau đó chúng ta tiếp tục sang bước chọn 1 và thực hiện chi tiết.

Thiết lập project test theo cấu trúc gợi ý, chuẩn hóa tool (xUnit/Moq/FluentAssertions).
Bao phủ thêm UpdateAsync, DeleteAsync, GetPagedAsync với dữ liệu mô phỏng.
Sau khi hoàn thành module Course, mở rộng tương tự cho BillsController và IUserService.SignInAsync vì impact cao.
Thiết lập CI chạy test (GitHub Actions) để đảm bảo chất lượng liên tục.
```
