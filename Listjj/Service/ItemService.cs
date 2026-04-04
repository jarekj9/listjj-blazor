using Listjj.Abstract;
using Listjj.Infrastructure.DTOs;
using Listjj.Models;
using Ganss.Xss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Listjj.Service
{
    public class ItemService: IItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HtmlSanitizer _htmlSanitizer;

        public ItemService(IUnitOfWork unitOfWork, HtmlSanitizer htmlSanitizer)
        {
            _unitOfWork = unitOfWork;
            _htmlSanitizer = htmlSanitizer;
        }

        /// <summary>
        /// Add or update a list item with HTML sanitization.
        /// </summary>
        /// <param name="request">Item data to add or update</param>
        /// <returns>The created or updated ListItem</returns>
        public async Task<ListItem> AddOrUpdateItemAsync(ListItemAddOrUpdateRequest request)
        {
            // Sanitize HTML content
            request.Name = _htmlSanitizer.Sanitize(request.Name);
            request.Description = _htmlSanitizer.Sanitize(request.Description);
            request.Tags = request.Tags?.Select(tag => _htmlSanitizer.Sanitize(tag)).ToList();

            var existingItem = await _unitOfWork.ListItems.GetById(request.Id);

            if (existingItem != null)
            {
                // Update existing item
                existingItem.Name = request.Name;
                existingItem.Description = request.Description;
                existingItem.Tags = string.Join(",", request.Tags ?? new List<string>());
                existingItem.CategoryId = request.CategoryId;
                existingItem.Active = request.Active;
                existingItem.Modified = DateTime.UtcNow;
                _unitOfWork.ListItems.Update(existingItem);
                await _unitOfWork.Save();
                return existingItem;
            }
            else
            {
                // Create new item
                var newItem = new ListItem
                {
                    Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id,
                    Name = request.Name,
                    Description = request.Description,
                    Tags = string.Join(",", request.Tags ?? new List<string>()),
                    CategoryId = request.CategoryId,
                    Active = request.Active,
                    UserId = request.UserId,
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow
                };

                // Calculate sequence number based on existing items in category
                var allSequenceNumbers = (await _unitOfWork.ListItems.GetAllByCategoryId(request.CategoryId))
                    .Select(i => i.SequenceNumber)
                    .ToList();
                newItem.SequenceNumber = allSequenceNumbers.Count > 0 ? allSequenceNumbers.Max() + 1 : 1;

                _unitOfWork.ListItems.Add(newItem);
                await _unitOfWork.Save();
                return newItem;
            }
        }
    }

    public interface IItemService
    {
        Task<ListItem> AddOrUpdateItemAsync(ListItemAddOrUpdateRequest request);
    }
}
