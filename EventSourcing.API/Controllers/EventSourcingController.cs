using EventSourcing.API.Services;
using EventSourcing.Shared;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcing.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventSourcingController : ControllerBase
    {
        private readonly IEventSourcingService service;

        public EventSourcingController(IEventSourcingService service)
        {
            this.service = service;
        }

        [HttpPost]
        [Route("createaccount")]
        public ActionResult CreateAccount([FromBody] AccountDto accountDto)
        {
            service.CreateAccount(accountDto);

            return Ok();
        }

        [HttpPost]
        [Route("movefunds")]
        public ActionResult MoveFunds([FromBody] MoveFundsDto moveFundsDto)
        {
            service.MoveFunds(moveFundsDto);

            return Ok();
        }

        [HttpGet]
        [Route("getevents")]
        public string GetAllEvents()
        {
            return service.GetAllEvents();
        }
    }
}
