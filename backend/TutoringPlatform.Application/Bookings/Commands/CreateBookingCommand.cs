using TutoringPlatform.Application.Bookings.Dtos;

namespace TutoringPlatform.Application.Bookings.Commands;

public record CreateBookingCommand(
    CreateBookingDto Dto,
    Guid StudentUserId
);
