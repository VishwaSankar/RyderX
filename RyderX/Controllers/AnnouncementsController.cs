using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RyderX_Server.DTO.AnnouncementDTOs;
using RyderX_Server.Models;
using RyderX_Server.Repositories.Interfaces;

namespace RyderX_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementsController : ControllerBase
    {
        private readonly IAnnouncementRepository _announcementRepository;

        public AnnouncementsController(IAnnouncementRepository announcementRepository)
        {
            _announcementRepository = announcementRepository;
        }

        // GET: api/announcements/active
        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveAnnouncements()
        {
            try
            {
                var announcements = await _announcementRepository.GetActiveAsync();

                var result = announcements.Select(a => new AnnouncementDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Message = a.Message,
                    Audience = a.Audience,
                    CreatedAt = a.CreatedAt,
                    ExpiryDate = a.ExpiryDate,
                    IsActive = a.IsActive
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching active announcements", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // GET: api/announcements
        [HttpGet]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetAllAnnouncements()
        {
            try
            {
                var announcements = await _announcementRepository.GetAllAsync();

                var result = announcements.Select(a => new AnnouncementDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Message = a.Message,
                    Audience = a.Audience,
                    CreatedAt = a.CreatedAt,
                    ExpiryDate = a.ExpiryDate,
                    IsActive = a.IsActive
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching all announcements", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // POST: api/announcements
        [HttpPost]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var announcement = new Announcement
                {
                    Title = dto.Title,
                    Message = dto.Message,
                    Audience = dto.Audience,
                    CreatedAt = DateTime.UtcNow,
                    ExpiryDate = dto.ExpiryDate,
                    IsActive = true
                };

                await _announcementRepository.AddAsync(announcement);
                return Ok(new { Message = "Announcement created successfully", Id = announcement.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error creating announcement", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // PUT: api/announcements
        [HttpPut]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> UpdateAnnouncement([FromBody] UpdateAnnouncementDto dto)
        {
            try
            {
                var existing = await _announcementRepository.GetByIdAsync(dto.Id);
                if (existing == null) return NotFound(new { Message = "Announcement not found" });

                if (!string.IsNullOrEmpty(dto.Title)) existing.Title = dto.Title;
                if (!string.IsNullOrEmpty(dto.Message)) existing.Message = dto.Message;
                if (!string.IsNullOrEmpty(dto.Audience)) existing.Audience = dto.Audience;
                if (dto.ExpiryDate.HasValue) existing.ExpiryDate = dto.ExpiryDate;
                if (dto.IsActive.HasValue) existing.IsActive = dto.IsActive.Value;

                await _announcementRepository.UpdateAsync(existing);

                return Ok(new { Message = "Announcement updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error updating announcement", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // DELETE: api/announcements/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            try
            {
                var existing = await _announcementRepository.GetByIdAsync(id);
                if (existing == null) return NotFound(new { Message = "Announcement not found" });

                await _announcementRepository.DeleteAsync(id);
                return Ok(new { Message = "Announcement deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error deleting announcement", Details = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}
