using Lingua.Shared;
using Lingua.ZoomIntegration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lingua.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMeetingClient _zoomMeetingService;
        private readonly IDateTimeProvider _dateTime;
        private readonly IEmailService _emailService;
        private readonly ITemplateProvider _templateProvider;
        private readonly ILogger<RoomService> _logger;

        public RoomService(IRoomRepository roomRepository,
                            IUserRepository userRepository,
                            IMeetingClient zoomMeetingService,
                            IDateTimeProvider dateTime,
                            IEmailService emailService,
                            ITemplateProvider templateProvider,
                            ILogger<RoomService> logger = null)
        {
            _roomRepository = roomRepository;
            _userRepository = userRepository;
            _zoomMeetingService = zoomMeetingService;
            _dateTime = dateTime;
            _emailService = emailService;
            _templateProvider = templateProvider;
            _logger = logger;
        }

        public async Task<List<Room>> Available(SearchRoomOptions options, Guid userId)
        {
            var user = await _userRepository.Get(userId);
            var rooms = (await _roomRepository.Get(r =>
                                    r.StartDate > _dateTime.UtcNow
                                    && r.Language == user.TargetLanguage
                                    && !r.Participants.Any(p => p.Id == userId)))
                .Where(r => r.Participants.Count < r.MaxParticipants);

            if (options != null)
            {
                rooms = ApplySearchFilter(options, rooms, user.Timezone);
            }

            return rooms.ToList();
        }

        private static IEnumerable<Room> ApplySearchFilter(SearchRoomOptions options, IEnumerable<Room> rooms, string timezone)
        {
            if (options?.Levels?.Any() == true)
            {
                rooms = rooms.Where(r => options.Levels.Contains(r.Host.LanguageLevel));
            }

            if (options?.Days?.Any() == true)
            {
                rooms = rooms.Where(r => options.Days.Contains(Utilities.ConvertToTimezone(r.StartDate, timezone).DayOfWeek));
            }

            if ((bool)options?.TimeFrom.HasValue && (bool)options?.TimeTo.HasValue)
            {
                //store room.till date
                rooms = rooms.Where(r => r.StartDate.TimeOfDay > options.TimeFrom.Value.TimeOfDay
                                        && r.StartDate.TimeOfDay < options.TimeTo.Value.TimeOfDay);
            }

            return rooms;
        }

        public async Task<List<Room>> Upcoming(Guid userId)
        {
            var user = await _userRepository.Get(userId);
            var rooms = await _roomRepository.Get(r =>
                            r.Participants.Any(p => p.Id == userId)
                            && (
                                (r.EndDate > _dateTime.UtcNow && r.Participants.Count > 1)
                                || r.StartDate > _dateTime.UtcNow
                               )
                            );

            return rooms.ToList();
        }

        public async Task<List<Room>> Past(Guid userId)
        {
            var user = await _userRepository.Get(userId);
            var rooms = await _roomRepository.Get(r =>
                            r.Participants.Any(p => p.Id == userId)
                            && !(
                                (r.EndDate > _dateTime.UtcNow && r.Participants.Count > 1)
                                || r.StartDate > _dateTime.UtcNow
                               )
                            );

            return rooms.ToList();
        }

        public async Task<Room> Create(CreateRoomOptions options, Guid userId)
        {
            var user = await _userRepository.Get(userId);
            var start = options.StartDate;
            var end = options.StartDate.AddMinutes(options.DurationInMinutes);
            var conflicts = await GetConflicts(userId, start, end);

            if (conflicts.Any())
            {
                throw new ValidationException(ValidationExceptionType.Rooms_Create_Conflict);
            }

            var room = new Room
            {
                HostUserId = userId,
                Language = options.Language,
                StartDate = start,
                EndDate = end,
                DurationInMinutes = options.DurationInMinutes,
                Topic = options.Topic,
                MaxParticipants = 2
            };

            room.Participants.Add(user);

            await _roomRepository.Create(room);

            return room;
        }

        private async Task<IEnumerable<Room>> GetConflicts(Guid userId, DateTime start, DateTime end)
        {
            return (await _roomRepository.Get(r =>
                                r.StartDate < end
                                && start < r.EndDate
                                && r.Participants.Any(p => p.Id == userId)))
                            .Where(r => r.StartDate > _dateTime.UtcNow || r.Participants.Count == r.MaxParticipants);
        }

        public async Task<Room> Update(UpdateRoomOptions options, Guid userId)
        {
            var room = await _roomRepository.Get(options.RoomId);
            room.Topic = options.Topic;
            if (options.StartDate.HasValue)
            {
                room.StartDate = options.StartDate.Value;
                room.EndDate = options.StartDate.Value.AddMinutes(room.DurationInMinutes);
            }

            if (options.DurationInMinutes.HasValue)
            {
                room.DurationInMinutes = options.DurationInMinutes.Value;
                room.EndDate = room.StartDate.AddMinutes(options.DurationInMinutes.Value);
            }

            await _roomRepository.Update(room);

            SendUpdateEmail(room, userId, "Room has been updated.");

            return room;
        }

        private async void SendUpdateEmail(Room room, Guid userId, string message)
        {
            var recipients = room.Participants.Where(u => u.Id != userId);

            foreach (var recipient in recipients)
            {
                try
                {
                    _logger?.LogInformation($"Before room:{room?.Id} update message is sent to {recipient?.Email}");

                    var body = await _templateProvider.GetRoomUpdateEmail(message, room, recipient);

                    await _emailService.SendAsync(
                        "Room update",
                        body,
                        true,
                        recipient.Email
                        ).ConfigureAwait(false);

                    _logger?.LogInformation($"After room:{room?.Id} update message is sent to {recipient?.Email}");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"Room:{room?.Id} update message is failed to sent to {recipient?.Email}");
                }
            }
        }

        public async Task<Room> Remove(Guid roomId, Guid userId)
        {
            var user = await _userRepository.Get(userId);
            await _roomRepository.Remove(roomId);
            var room = await _roomRepository.Get(roomId);

            SendUpdateEmail(room, userId, $"<b>{user.Fullname} deleted the room you have previously entered.</b> You can go ahead and create your own room for that time.");

            return room;
        }

        public async Task<Room> Enter(Guid roomId, Guid userId)
        {
            var user = await _userRepository.Get(userId);
            var room = await _roomRepository.Get(roomId);

            if (room.StartDate < _dateTime.UtcNow)
            {
                throw new ValidationException(ValidationExceptionType.Rooms_Enter_AlreadyStarted);
            }
            if (room.Participants.Any(p => p.Id == userId))
            {
                return room;
            }
            if (room.Participants.Count == room.MaxParticipants)
            {
                throw new ValidationException(ValidationExceptionType.Rooms_Enter_AlreadyFull);
            }

            var start = room.StartDate;
            var end = room.StartDate.AddMinutes(room.DurationInMinutes);

            var conflicts = await GetConflicts(userId, start, end);

            if (conflicts.Any())
            {
                throw new ValidationException(ValidationExceptionType.Rooms_Enter_Conflict);
            }

            room.Participants.Add(user);
            await _roomRepository.Update(room);

            SendUpdateEmail(room, userId, $"<b>{user.Fullname} entered the room.</b>");

            return room;
        }

        public async Task<Room> Leave(Guid roomId, Guid userId)
        {
            var user = await _userRepository.Get(userId);

            var room = await _roomRepository.Get(roomId);
            room.Participants.RemoveAll(p => p.Id == user.Id);
            await _roomRepository.Update(room);

            SendUpdateEmail(room, userId, $"<b>{user.Fullname} left the room.</b>");

            return room;
        }

        public async Task<Room> Join(Guid roomId, Guid userId)
        {
            var room = await _roomRepository.Get(roomId);
            if (room.JoinUrl != null)
            {
                return room;
            }

            var user = await _userRepository.Get(userId);
            var accessTokens = user.ZoomProperties?.AccessTokens;

            var request = new CreateMeetingRequest
            {
                Topic = room.Topic,
                Duration = room.DurationInMinutes,
                StartTime = room.StartDate,
                Type = MeetingType.Scheduled,
                Timezone = "UTC"
            };

            var response = await _zoomMeetingService.CreateMeeting(accessTokens, request);
            room.JoinUrl = response.Response.JoinUrl;
            await _roomRepository.Update(room);

            if (response.NewTokens != null)
            {
                await _userRepository.Update(user);
            }

            return room;

        }
    }
}
