
using MediatR;

namespace Nick.Mediator.Application.Handlers.User.Queries;

public class GetUserGird:IRequest<GetUserResponse>
{
    public GetUserRequest Request { get; }

    public GetUserGird(GetUserRequest request)
    {
        Request = request;
    }
    public class GetUserHandler:IRequestHandler<GetUserGird, GetUserResponse>
    {
        public GetUserHandler()
        {
            
        }
        public Task<GetUserResponse> Handle(GetUserGird request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}