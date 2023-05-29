using Listjj.Infrastructure.ViewModels;
using Listjj_frontend.Services;
using Listjj_frontend.Services.Abstract;
using Moq;

namespace ListjjTests
{
    public class Tests
    {
        private Mock<IApiClient> apiClientMock;

        [SetUp]
        public void Setup()
        {
            var id = Guid.Parse("f7667c3ef64949c8b140011e6d3a5d69");
            apiClientMock = new Mock<IApiClient>();
            apiClientMock.Setup(x => x.Get<FileSimpleViewModel>($"https://localhost:5001/api/file/get_file_simple_by_id?id={id}")).Returns(
                Task.FromResult((new FileSimpleViewModel(), new HttpResponseMessage()))
            );

        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public async Task Test_FileService_GetFileWithoutBytes()
        {
            var id = Guid.Parse("f7667c3ef64949c8b140011e6d3a5d69");
            var fileService = new FileService(apiClientMock.Object);
            await fileService.GetFileWithoutBytes(id);
            apiClientMock.Verify(x => x.Get<FileSimpleViewModel>($"https://localhost:5001/api/file/get_file_simple_by_id?id={id}"), Times.Once);
        }
    }
}