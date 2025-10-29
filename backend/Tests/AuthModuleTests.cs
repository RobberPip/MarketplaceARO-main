using Auth.Dtos;
using Market.Profile;
using System.Net;
using System.Net.Http.Json;
using Tests.Utils;

namespace Tests {

    public class AuthModuleTests : IntegrationTestBase {
        public AuthModuleTests(SoftwaresMarketFactory factory) : base(factory) { }

        [Fact]
        public async Task Login_Success() {
            //Arrange
            var loginDto = new LoginDto {
                Email = "test_user123@test.com",
                Password = "n0rmAl###"
            };

            //Act
            var response = await unauthHttpClient.PostAsJsonAsync("/auth/login", loginDto);
            response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> cookies);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(cookies);
            Assert.Contains(cookies, c => c.StartsWith("auth="));
        }

        [Fact]
        public async Task Login_UncorrectPassword() {
            //Arrange
            var loginDto = new LoginDto {
                Email = "test_user123@test.com",
                Password = "Test%$eedU$er1"
            };

            //Act
            var response = await unauthHttpClient.PostAsJsonAsync("/auth/login", loginDto);
            //var body = await responce.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Logout_Success() {
            //Arrange

            //Act
            var response = await authHttpClient.PostAsync("/auth/logout", null);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CurrenUserInfo_Success() {
            //Arrange
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            //Act
            var response = await authHttpClient.GetAsync("/auth/current-user-info");
            var body = await response.Content.ReadFromJsonAsync<CurrentUserInfoDto>();

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(body?.Roles.Any());
        }

        [Fact]
        public async Task Register_Success() {
            //Arrange
            var registerDto = new RegistrationDto {
                Email = "test@test.com",
                Password = "tE$t12111",
                ConfirmPassword = "tE$t12111"
            };
            var loginDto = new LoginDto {
                Email = "test@test.com",
                Password = "tE$t12111"
            };

            //Act
            var registerResponse = await unauthHttpClient.PostAsJsonAsync("/auth/register", registerDto);
            var loginResponse = await unauthHttpClient.PostAsJsonAsync("/auth/login", loginDto);

            var currentUserResponse = await unauthHttpClient.GetAsync("/auth/current-user-info");
            var currentUser = await currentUserResponse.Content.ReadFromJsonAsync<CurrentUserInfoDto>();

            //Assert
            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, currentUserResponse.StatusCode);
            Assert.True(currentUser?.Roles.Contains("Member"));
        }

        [Fact]
        public async Task Register_AlreadyExist() {
            //Arrange
            var dto = new RegistrationDto {
                Email = "test_user123@test.com",
                Password = "n0rmAl###",
                ConfirmPassword = "n0rmAl###"
            };

            //Act
            var response = await unauthHttpClient.PostAsJsonAsync("/auth/register", dto);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ConfrimEmail_Unauthorized() {
            //Arrange

            //Act
            var response = await unauthHttpClient.GetAsync("/auth/confirm-email");

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ConfrimChangeEmail_BadRequest() {
            //Arrange

            //Act
            var response = await unauthHttpClient.GetAsync("/auth/confirm-change-email");

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ChangeEmail_Success() {
            //Arrange
            var dto = new EmailDto {
                Email = "qwerty@qwerty.ru"
            };

            //Act
            var responce = await authHttpClient.PostAsJsonAsync("/auth/change-email", dto);

            //Assert
            Assert.Equal(HttpStatusCode.OK, responce.StatusCode);
        }

        [Fact]
        public async Task ChangeEmail_ValidationEmail() {
            //Arrange
            var dto = new EmailDto {
                Email = "qwerty"
            };

            //Act
            var responce = await authHttpClient.PostAsJsonAsync("/auth/change-email", dto);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, responce.StatusCode);
        }

        [Fact]
        public async Task ChangeEmail_EmailAlreadyExist() {
            //Arrange
            var registerDto = new RegistrationDto {
                Email = "test@test.com",
                Password = "tE$t12111",
                ConfirmPassword = "tE$t12111"
            };
            var changeEmailDto = new EmailDto {
                Email = "test@test.com"
            };

            //Act
            var registerResponse = await authHttpClient.PostAsJsonAsync("/auth/register", registerDto);
            var changeEmailResponce = await authHttpClient.PostAsJsonAsync("/auth/change-email", changeEmailDto);

            //Assert
            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, changeEmailResponce.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_Success() {
            //Arrange
            var changePasswordDto = new ChangePasswordDto {
                CurrentPassword = "n0rmAl###",
                NewPassword = "12Wqwe%%3"
            };
            var loginDto = new LoginDto {
                Email = "test_user123@test.com",
                Password = "12Wqwe%%3"
            };

            //Act
            var changePasswordResponce = await authHttpClient.PostAsJsonAsync("/auth/change-password", changePasswordDto);
            var loginResponse = await authHttpClient.PostAsJsonAsync("/auth/login", loginDto);
            loginResponse.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> cookies);

            //Assert
            Assert.Equal(HttpStatusCode.OK, changePasswordResponce.StatusCode);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_CurrentPasswordInvalid() {
            //Arrange
            var changePasswordDto = new ChangePasswordDto {
                CurrentPassword = "n0rmAl#vdasfv##",
                NewPassword = "12Wqwe%%3"
            };

            //Act
            var changePasswordResponce = await authHttpClient.PostAsJsonAsync("/auth/change-password", changePasswordDto);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, changePasswordResponce.StatusCode);
        }

        [Fact]
        public async Task ResetPassword_EmailNotExist() {
            //Arrange
            var emailDto = new EmailDto {
                Email = "qwerty@qwerty.ru"
            };

            //Act
            var changePasswordResponce = await unauthHttpClient.PostAsJsonAsync("/auth/reset-password", emailDto);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, changePasswordResponce.StatusCode);
        }

        [Fact]
        public async Task ResetPassword_Success() {
            //Arrange
            var emailDto = new EmailDto {
                Email = "test_user123@test.com"
            };

            //Act
            var changePasswordResponce = await unauthHttpClient.PostAsJsonAsync("/auth/reset-password", emailDto);

            //Assert
            Assert.Equal(HttpStatusCode.OK, changePasswordResponce.StatusCode);
        }
    }
}