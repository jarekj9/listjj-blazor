using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Listjj.Models;
using System.Collections.Generic;

namespace Listjj.Abstract
{
    public interface IFileService
    {
        Task SaveAs(IJSRuntime js, string filename, byte[] data);
        Task<File> FindById(Guid id);
        Task<File> AddFile(Guid itemId, byte[] Bytes, string name);
        Task<List<(string, Guid)>> GetNamesAndIds(Guid itemId);
        Task<bool> DelFile(Guid fileId);
        Task<File> GetFile(Guid fileId);
    }
}
