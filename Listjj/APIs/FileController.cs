using AutoMapper;
using Listjj.Abstract;
using Listjj.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Listjj.APIs
{
    [Authorize(Roles = "Admin,User")]
    public class FileController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<FileController> logger;
        private readonly IMapper mapper;
        private readonly IFileService fileService;

        public FileController(IUnitOfWork unitOfWork, ILogger<FileController> logger, IMapper mapper, IFileService fileService)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
            this.fileService = fileService;
        }

        [Route("api/[controller]/get_file_simple_by_id")]
        [HttpGet]
        public async Task<JsonResult> GetFileWithoutBytesById(string id)
        {
            var fileId = Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
            var file = await fileService.GetFile(fileId);
            var fileVm = mapper.Map<FileSimpleViewModel>(file);
            return new JsonResult(fileVm);
        }

        [Route("api/[controller]/get_by_id")]
        [HttpGet]
        public async Task<JsonResult> GetFileById(string id)
        {
            var fileId = Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
            var file = await fileService.GetFile(fileId);
            var fileVm = mapper.Map<FileViewModel>(file);
            return new JsonResult(fileVm);
        }


        [Route("api/[controller]/add")]
        [HttpPost]
        public async Task<JsonResult> Add([FromBody] FileViewModel file)
        {
            var bytes = Convert.FromBase64String(file.B64Bytes);
            var result = await fileService.AddFile(file.ListItemId, bytes, file.Name);
            return new JsonResult(result);
        }

        [Route("api/[controller]/delete")]
        [HttpPost]
        public async Task<JsonResult> DeleteFile([FromBody] Guid id)
        {
            var result = await fileService.DelFile(id);
            return new JsonResult(result);
        }
    }
}
