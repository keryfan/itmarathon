using CSharpFunctionalExtensions;
using Epam.ItMarathon.ApiService.Application.UseCases.User.Commands;
using Epam.ItMarathon.ApiService.Application.UseCases.User.Queries;
using Epam.ItMarathon.ApiService.Domain.Abstract;
using Epam.ItMarathon.ApiService.Domain.Shared.ValidationErrors;
using FluentValidation.Results;
using MediatR;
using RoomAggregate = Epam.ItMarathon.ApiService.Domain.Aggregate.Room.Room;

namespace Epam.ItMarathon.ApiService.Application.UseCases.User.Handlers
{
    /// <summary>
    /// Handler for Users query.
    /// </summary>
    /// <param name="userRepository">Implementation of <see cref="IUserReadOnlyRepository"/> for operating with database.</param>
    public class DeleteUsersHandler(IRoomRepository roomRepository)
        : IRequestHandler<DeleteUsersQuery, Result<RoomAggregate, ValidationResult>>
    {
        ///<inheritdoc/>
        public async Task<Result<RoomAggregate, ValidationResult>> Handle(DeleteUsersQuery request,
            CancellationToken cancellationToken)
        {
            //Get room by UserCode
            var roomResult = await roomRepository.GetByUserCodeAsync(request.UserCode, cancellationToken);
            
            if (roomResult.IsFailure)
            {
                return roomResult;
            }

            //Delete user by Id in room's users - room.DeleteUser(userId)
            var room = roomResult.Value;
            var deleteResult = room.DeleteUser(request.UserId);
            
            if (deleteResult.IsFailure)
            {
                return deleteResult;
            }

            // Update room in repository
            var updateResult = await roomRepository.UpdateAsync(room, cancellationToken);

            if (updateResult.IsFailure)
            {
                return Result.Failure<RoomAggregate, ValidationResult>(new BadRequestError([
                    new ValidationFailure(string.Empty, updateResult.Error)
                ]));
            }

            // Get updated room
            var updatedRoomResult = await roomRepository.GetByUserCodeAsync(request.UserCode, cancellationToken);
            return updatedRoomResult;
        }
    }
}