using Nick.Mediator.Common.Grid.Processor;
using Nick.Mediator.Common.Grid.Response;

namespace Nick.Mediator.Application.Handlers.User.Queries;

public class GetUserResponse: BaseGridResponse<UserGridViewModel>
{
    
}
public class UserGridViewModel : IGridResponseItemModel
{
    public int Id { get; set; }
    
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FaxNumber { get; set; }
}