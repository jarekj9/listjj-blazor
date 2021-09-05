using System;
using System.Threading.Tasks;
using Listjj.Abstract;
using Microsoft.JSInterop;
using Listjj.Models;
using Listjj.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Microsoft.EntityFrameworkCore;



namespace Listjj.Service
{
    public class FileService : IFileService
    {
        private readonly AppDbContext  _appDbContext;
        public FileService (AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task SaveAs(IJSRuntime js, string filename, byte[] data)
        {
            await js.InvokeAsync<object>(
                "saveAsFile",
                filename,
                Convert.ToBase64String(data));
        }

        public async Task<File> FindById(Guid id)
        {
            return (await _appDbContext.Files.FindAsync(id));
        }
        public async Task<File> AddFile(Guid itemId, byte[] Bytes, string name)
        {  
            var file = new File {
                Bytes = Bytes,
                ListItemId = itemId,
                Name = name,
                Size = Bytes.Length
            };
            await _appDbContext.Files.AddAsync(file);
            await _appDbContext.SaveChangesAsync();
            return file;
        }

        public async Task<List<(string, Guid)>> GetNamesAndIds(Guid itemId)
        {  
             var files = await _appDbContext.Files.Where(x => x.ListItemId == itemId).ToListAsync();
            return files.Select(f => (f.Name, f.Id)).ToList();
        }
        public async Task<bool> DelFile(Guid fileId)
        {
            var file = await _appDbContext.Files.FindAsync(fileId);
            _appDbContext.Files.Remove(file);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<File> GetFile(Guid fileId)
        {
            var file = await _appDbContext.Files.FindAsync(fileId);
            return file;
        }
    }
}