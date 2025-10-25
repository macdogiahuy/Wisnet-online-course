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

## Test Case Inventory

### Assignment Module

| Test ID | Test Name                                                        | Type       | Priority | Description                                 |
| ------- | ---------------------------------------------------------------- | ---------- | -------- | ------------------------------------------- |
| TC001   | CreateAssignment_WithValidData_ShouldCreateSuccessfully          | Functional | High     | Tao assignment voi du lieu hop le           |
| TC004   | AssignmentAttempt_EnrolledStudent_ShouldDisplayAssignment        | Functional | High     | Hien thi assignment cho hoc vien da dang ky |
| TC007   | CreateAssignment_WithInvalidFile_ShouldReturnError               | Negative   | High     | Tao assignment voi file khong hop le        |
| TC009   | AssignmentAttempt_NotEnrolled_ShouldRedirectToCourseDetail       | Negative   | High     | Truy cap assignment khi chua dang ky        |
| TC012   | CreateAssignment_WithMinimumDuration_ShouldHandleCorrectly       | Boundary   | Medium   | Test voi thoi gian toi thieu                |
| TC013   | CreateAssignment_WithBoundaryGradeToPass_ShouldValidateCorrectly | Boundary   | Medium   | Test voi diem dat o cac gia tri bien        |
| TC016   | SubmitAssignment_WithValidAnswers_ShouldRedirectToOverview       | Functional | Medium   | Nop bai assignment thanh cong               |
| TC017   | AssignmentAttempt_UnauthenticatedUser_ShouldRedirectToLogin      | Negative   | Medium   | User chua dang nhap truy cap assignment     |

### Course Module

| Test ID | Test Name                                                          | Type       | Priority | Description                       |
| ------- | ------------------------------------------------------------------ | ---------- | -------- | --------------------------------- |
| TC002   | UpdateCourse_ByOwner_ShouldUpdateSuccessfully                      | Functional | High     | Cap nhat khoa hoc boi chu so huu  |
| TC003   | CreateCourse_WithValidData_ShouldCreateSuccessfully                | Functional | High     | Tao khoa hoc voi du lieu hop le   |
| TC005   | SearchCourses_ByKeyword_ShouldReturnMatchingResults                | Functional | Medium   | Tim kiem khoa hoc theo tu khoa    |
| TC006   | CourseDetail_WithValidId_ShouldDisplayFullInformation              | Functional | Medium   | Hien thi chi tiet khoa hoc        |
| TC010   | UpdateCourse_WithEmptyTitle_ShouldReturnError                      | Negative   | High     | Cap nhat khoa hoc voi title trong |
| TC011   | CourseDetail_WithNonExistentId_ShouldRedirectTo404                 | Negative   | High     | Truy cap khoa hoc khong ton tai   |
| TC015   | UpdateCourse_ByDifferentInstructor_ShouldPreventUnauthorizedAccess | Security   | High     | Ngan chan truy cap trai phep      |
| TC018   | CreateCourse_WithoutSections_ShouldShowValidationError             | Negative   | Medium   | Tao khoa hoc khong co sections    |
| TC019   | CreateCourse_AsLearner_ShouldReturnForbidden                       | Security   | High     | Learner tao khoa hoc (forbidden)  |

### Payment/Withdrawal Module

| Test ID | Test Name                                                         | Type       | Priority | Description                            |
| ------- | ----------------------------------------------------------------- | ---------- | -------- | -------------------------------------- |
| TC003   | RequestWithdrawal_WithSufficientBalance_ShouldProcessSuccessfully | Functional | High     | Yeu cau rut tien voi so du du          |
| TC008   | RequestWithdrawal_ExceedsBalance_ShouldReturnError                | Negative   | High     | Yeu cau rut tien vuot qua so du        |
| TC014   | InstructorWithdraw_AsLearner_ShouldReturnForbidden                | Security   | High     | Learner truy cap trang rut tien        |
| TC020   | DisplayWithdrawalHistory_ShouldShowPreviousRequests               | Functional | Medium   | Hien thi lich su rut tien              |
| TC021   | RequestWithdrawal_WithInvalidAccountNumber_ShouldReturnError      | Negative   | Medium   | Rut tien voi so tai khoan khong hop le |
| TC022   | InstructorWithdraw_Unauthenticated_ShouldRedirectToLogin          | Security   | High     | User chua dang nhap truy cap rut tien  |
| TC023   | RequestWithdrawal_WithMinimumAmount_ShouldProcessCorrectly        | Boundary   | Medium   | Test voi so tien toi thieu             |

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
[Test Session Finished] 10/25/2025 13:03:47
Total tests: 1
Passed tests: 1
Success rate: 100.00%
...
Build succeeded with 8 warning(s) in 2.8s
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
