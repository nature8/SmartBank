using Microsoft.AspNetCore.Mvc;
using SmartBank.Notification.DTOs;
using SmartBank.Notification.Services;

namespace SmartBank.Notification.Controllers
{
    [ApiController]
    [Route("api/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationAppService _service;

        public NotificationController(NotificationAppService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNotificationDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var result = await _service.GetByCustomerIdAsync(customerId);
            return Ok(result);
        }
    }
}