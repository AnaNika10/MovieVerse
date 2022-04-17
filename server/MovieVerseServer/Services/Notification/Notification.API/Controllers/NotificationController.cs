using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notification.API.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notification.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _repository;

        public NotificationController(INotificationRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Entities.Notification>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Entities.Notification>>> GetNotifications()
        {
            var notifications = await _repository.GetNotifications();
            return Ok(notifications);
        }
    }
}
