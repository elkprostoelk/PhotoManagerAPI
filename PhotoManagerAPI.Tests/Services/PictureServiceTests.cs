using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PhotoManagerAPI.Core.Configurations;
using PhotoManagerAPI.Core.DTO;
using PhotoManagerAPI.Core.Services;
using PhotoManagerAPI.DataAccess.Entities;
using PhotoManagerAPI.DataAccess.Repositories;
using PhotoManagerAPI.Web.AutoMapper;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace PhotoManagerAPI.Tests.Services
{
    [ExcludeFromCodeCoverage]
    public class PictureServiceTests : BaseServiceTests
    {
        private readonly PictureService _pictureService;
        private readonly Mock<IPictureUploaderService> _pictureUploaderServiceMock = new();

        public PictureServiceTests()
        {
            var loggerMock = new Mock<ILogger<PictureService>>();
            var userRepository = new UserRepository(DbContext);
            var pictureRepository = new Repository<Picture, Guid>(DbContext);
            var imageOptions = Options.Create(new ImageOptions
            {
                AllowedFileTypes = [".jpg", ".jpeg", ".png"],
                MaxFileSizeBytes = 5 * 1024 * 1024
            });
            var mapper = new MapperConfiguration(config => config.AddProfile<AutoMapperProfile>()).CreateMapper();
            _pictureService = new PictureService(
                pictureRepository,
                userRepository,
                Configuration,
                loggerMock.Object,
                imageOptions,
                _pictureUploaderServiceMock.Object,
                mapper);
        }

        [Fact]
        public async Task GetAsync_InvalidId_ReturnsNull()
        {
            // Act

            var picture = await _pictureService.GetAsync(new Guid());

            // Assert

            Assert.Null(picture);
        }

        [Theory]
        [InlineData("019157fb-a9a9-8edd-8391-7464a9e815d4")]
        [InlineData("019157fb-a9bc-2b4e-8641-79ac3b5aa499")]
        [InlineData("019157fb-a9bc-47ee-a1be-5150efd2a39d")]
        public async Task GetAsync_ValidId_ReturnsPicture(string id)
        {
            // Act

            var guidId = Guid.Parse(id);
            var picture = await _pictureService.GetAsync(guidId);

            // Assert

            Assert.NotNull(picture);
            Assert.Equal(picture.Id, guidId);
        }

        [Fact]
        public async Task AddAsync_NoValidUser_ReturnsUnsuccess()
        {
            // Act

            var result = await _pictureService.AddAsync(new NewPictureDto());

            // Assert

            Assert.False(result.IsSuccess);
            Assert.Contains("User was not found.", result.Errors);
        }

        [Theory]
        [InlineData(6 * 1024 * 1024, "image.jpeg", "File size is too large.")]
        [InlineData(4 * 1024 * 1024, "image.gif", "File type is invalid.")]
        public async Task AddAsync_FileInvalid_ReturnsUnsuccess(long fileSizeBytes, string fileName, string validationError)
        {
            // Arrange

            var pictureDto = new NewPictureDto
            {
                UserId = Guid.Parse("0190d1a3-4389-98ff-816b-a83e4376af7f"),
                FileSizeBytes = fileSizeBytes,
                FileName = fileName
            };

            // Act

            var result = await _pictureService.AddAsync(pictureDto);

            // Assert

            Assert.False(result.IsSuccess);
            Assert.Contains(validationError, result.Errors);
        }

        [Fact]
        public async Task AddAsync_ValidData_ReturnsSuccess()
        {
            // Arrange

            var pictureDto = new NewPictureDto
            {
                UserId = new Guid("0190d1a3-439c-d4e5-d362-32ca66912a88"),
                Title = "Test Adding Image",
                File = RandomNumberGenerator.GetBytes(1024),
                FileName = "test_image.jpg",
                Width = 1024,
                Height = 1024,
                FileSizeBytes = 1024
            };
            var fileName = $"{Guid.NewGuid()}.jpg";
            _pictureUploaderServiceMock.Setup(p => p.UploadAsync(It.Is<UploadPictureDto>(u =>
                u.UserId == pictureDto.UserId
                && u.FileName == pictureDto.FileName
                && u.File == pictureDto.File)))
                .ReturnsAsync(new ServiceResultDto<string>
                {
                    IsSuccess = true,
                    Container = fileName
                });

            // Act

            var result = await _pictureService.AddAsync(pictureDto);

            // Assert

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }
    }
}
