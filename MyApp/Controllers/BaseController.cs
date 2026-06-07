using Core.Attribute;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using wallet.lib.BaseApi.ControllerConfig;

namespace ApiApp.Controllers
{
    [ApiController]
    [ApiResultFilter]
    [Route(template: Constant.Router.Controller)]
    public class BaseController : wallet.lib.BaseApi.ControllerConfig.ControllerBase
    {
        public BaseController(IMediator mediator) : base(mediator)
        {
        }
    }
}
