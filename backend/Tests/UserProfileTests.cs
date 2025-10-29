using Market.Profile;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Json;
using Tests.Utils;

namespace Tests {
    public class UserProfileTests : IntegrationTestBase {
        public UserProfileTests(SoftwaresMarketFactory factory) : base(factory) { }


        [Fact]
        public async Task Profile_Get_Success() {
            //Arrange
            var identityData = TestData.GetIdentityTestData();
            var email = identityData.User.Email;
            var userId = identityData.User.Id;

            //Act
            var response = await authHttpClient.GetAsync("/api/v1/profile");
            var dto = await response.Content.ReadFromJsonAsync<GetUserProfileOut>();

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(email, dto?.UserEmail);
            Assert.Equal(userId, dto?.UserId);
        }

        [Fact]
        public async Task Profile_Put_Success() {
            //Arrange
            var updateDto = new UpdateUserProfileIn {
                Name = "display test",
                //CompanyIndustryId = 2,
                //CompanyRoleId = 1
            };

            //Act
            var response = await authHttpClient.PutAsJsonAsync("/api/v1/profile", updateDto);
            var dto = await response.Content.ReadFromJsonAsync<UpdateUserProfileOut>();

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(updateDto.Name, dto?.Name);
            Assert.Equal(updateDto.CompanyIndustryId, dto?.CompanyIndustry.Id);
            Assert.Equal(updateDto.CompanyRoleId, dto?.CompanyRole.Id);
        }

        [Fact]
        public async Task Profile_Put_RoleIdNotExist() {
            //Arrange
            var updateDto = new UpdateUserProfileIn {
                Name = "display test",
                //CompanyIndustryId = 2,
                //CompanyRoleId = 777
            };

            //Act
            var response = await authHttpClient.PutAsJsonAsync("/api/v1/profile", updateDto);
            var dto = await response.Content.ReadFromJsonAsync<UpdateUserProfileOut>();

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Profile_Put_IndustryIdNotExist() {
            //Arrange
            var updateDto = new UpdateUserProfileIn {
                Name = "display test",
                //CompanyIndustryId = 7777,
                //CompanyRoleId = 777
            };

            //Act
            var response = await authHttpClient.PutAsJsonAsync("/api/v1/profile", updateDto);
            var dto = await response.Content.ReadFromJsonAsync<UpdateUserProfileOut>();

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Avatar_Get_NoContent_Success() {
            //Arrange

            //Act
            var response = await authHttpClient.GetAsync("/api/v1/profile/avatar");

            //Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task AvatarPNG_Patch_Success() {
            //Arrange
            var stub = new byte[] { 0x20 };
            var imageStream = new MemoryStream(stub);
            using var multipartFormContent = new MultipartFormDataContent();
            multipartFormContent.Add(new StreamContent(imageStream), name: "imageFile", fileName: "test.png");

            //Act
            var response = await authHttpClient.PatchAsync("/api/v1/profile/avatar", multipartFormContent);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("image/png", response.Content?.Headers?.ContentType?.MediaType);
            Assert.Equal(imageStream.Length, response.Content?.Headers?.ContentLength);
        }

        [Fact]
        public async Task AvatarJPG_Patch_Success() {
            //Arrange
            var stub = new byte[] { 0x20 };
            var imageStream = new MemoryStream(stub);
            using var multipartFormContent = new MultipartFormDataContent();
            multipartFormContent.Add(new StreamContent(imageStream), name: "imageFile", fileName: "test.jpg");

            //Act
            var response = await authHttpClient.PatchAsync("/api/v1/profile/avatar", multipartFormContent);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("image/jpg", response.Content?.Headers?.ContentType?.MediaType);
            Assert.Equal(imageStream.Length, response.Content?.Headers?.ContentLength);
        }

        [Fact]
        public async Task Avatar_BigFile_Patch_BadRequest() {
            //Arrange
            var stub = Enumerable.Repeat<byte>(0x20, 5 * 1024 * 1024).ToArray();
            var imageStream = new MemoryStream(stub);
            using var multipartFormContent = new MultipartFormDataContent();
            multipartFormContent.Add(new StreamContent(imageStream), name: "imageFile", fileName: "test.png");

            //Act
            var response = await authHttpClient.PatchAsync("/api/v1/profile/avatar", multipartFormContent);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Avatar_Delete_Success() {
            //Arrange
            var stub = new byte[] { 0x20 };
            var imageStream = new MemoryStream(stub);
            using var multipartFormContent = new MultipartFormDataContent();
            multipartFormContent.Add(new StreamContent(imageStream), name: "imageFile", fileName: "test.jpg");

            //Act
            var responseCreate = await authHttpClient.PatchAsync("/api/v1/profile/avatar", multipartFormContent);
            var responseDelete = await authHttpClient.DeleteAsync("/api/v1/profile/avatar");

            //Assert
            Assert.Equal(HttpStatusCode.OK, responseCreate.StatusCode);
            Assert.Equal(HttpStatusCode.OK, responseDelete.StatusCode);
        }
    }
}
