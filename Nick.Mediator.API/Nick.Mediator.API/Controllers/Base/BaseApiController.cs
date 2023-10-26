using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nick.Mediator.Common.Controller;

namespace Nick.Mediator.API.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    private IMediator? _mediator;

        /// <summary>
        /// common Mediator object
        /// </summary>
        private IMediator? Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();


        /// <summary>
        /// Base method for all commands
        /// </summary>
        /// <param name="command"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        protected async Task<TResult> CommandAsync<TResult>(IRequest<TResult> command)
        {
            return await Mediator?.Send(command)!;
        }

        /// <summary>
        /// base method for all queries
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        protected async Task<TResult> QueryAsync<TResult>(IRequest<TResult> query)
        {
            return await Mediator?.Send(query)!;
        }

        /// <summary>
        /// base method for all events
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        protected async Task PublishAsync<TResult>(IRequest<TResult> query)
        {
            await Mediator?.Publish(query)!;
        }

        /// <summary>
        /// base method for all events
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        protected async Task PublishAsync(INotification notification)
        {
            await Mediator?.Publish(notification)!;
        }


        protected OkObjectResult ReturnObject<T>(T data, bool isSuccess = true, string message = null, string exception = null)
        {
            var responseModel = new ResponseModel<T>(data) { IsSuccess = isSuccess, Message = message, Exception = exception };
            return Ok(responseModel);
        }

        protected OkObjectResult ReturnSuccess(string message = null, string exception = null)
        {
            var responseModel = new ResponseModel { IsSuccess = true, Message = message, Exception = exception };
            return Ok(responseModel);
        }
        protected OkObjectResult ReturnFailure(string message = null, string exception = null)
        {
            var responseModel = new ResponseModel { IsSuccess = false, Message = message, Exception = exception };
            return Ok(responseModel);
        }
}