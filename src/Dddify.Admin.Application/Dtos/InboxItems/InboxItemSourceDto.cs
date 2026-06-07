using Dddify.Admin.Domain.Aggregates.InboxItems;

namespace Dddify.Admin.Application.Dtos.InboxItems;

public record InboxItemSourceDto(string Type, Guid Id);
