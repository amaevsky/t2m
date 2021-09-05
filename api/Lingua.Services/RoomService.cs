using AutoMapper;
using Lingua.Shared;
using Lingua.ZoomIntegration;
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
        private readonly IEnumerable<IRoomUpdatesObserver> _updatesObservers;

        public RoomService(IRoomRepository roomRepository,
                            IUserRepository userRepository,
                            IMeetingClient zoomMeetingService,
                            IDateTimeProvider dateTime,
                            IEnumerable<IRoomUpdatesObserver> updatesObservers = null)
        {
            _roomRepository = roomRepository;
            _userRepository = userRepository;
            _zoomMeetingService = zoomMeetingService;
            _dateTime = dateTime;
            _updatesObservers = updatesObservers ?? Enumerable.Empty<IRoomUpdatesObserver>();
        }

        public async Task<List<Room>> Available(SearchRoomOptions options, Guid userId)
        {
            var user = await _userRepository.Get(userId);
            var rooms = (await _roomRepository.Get(r =>
                                    r.StartDate > _dateTime.UtcNow
                                    && r.Language == user.TargetLanguage
                                    && !r.Requests.OfType<EnterRoomRequest>().Any()
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
                            && !r.Requests.OfType<EnterRoomRequest>().Any(r => r.From.Id == userId && r.Status == RoomRequestStatus.Requested)
                            && (
                                (r.EndDate > _dateTime.UtcNow && r.Participants.Count > 1)
                                || r.StartDate > _dateTime.UtcNow
                               )
                            );

            return rooms.ToList();
        }

        public async Task<List<Room>> GetRequests(Guid userId)
        {
            var user = await _userRepository.Get(userId);
            var rooms = await _roomRepository.Get(r =>
                            r.Requests.Any(r => (r.To.Id == userId || r.From.Id == userId) && r.Status == RoomRequestStatus.Requested)
                            && r.StartDate > _dateTime.UtcNow);

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

            return rooms.OrderByDescending(r => r.StartDate).ToList();
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

            room.Participants.Add(new RoomParticipant(user));

            if (options.Participants?.Any() == true)
            {
                var usrs = await _userRepository.Get(u => options.Participants.Contains(u.Id));
                var requests = usrs.Select(usr => new EnterRoomRequest { From = user, To = usr, Status = RoomRequestStatus.Requested });

                room.Requests.AddRange(requests);
            }

            await _roomRepository.Create(room);
            Notify(RoomUpdateType.Created, user, room);

            return room;
        }

        private void Notify(RoomUpdateType updateType, User user, Room room)
        {
            Task.Run(async () =>
            {
                try
                {
                    var tasks = _updatesObservers.Select(o => o.OnUpdate(updateType, room, user));
                    await Task.WhenAll(tasks.ToArray());
                }
                catch (Exception ex)
                {
                    //logger
                }
            });
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
            var user = await _userRepository.Get(userId);
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
            Notify(RoomUpdateType.Updated, user, room);

            return room;
        }

        public async Task<Room> Remove(Guid roomId, Guid userId)
        {
            var user = await _userRepository.Get(userId);
            await _roomRepository.Remove(roomId);
            var room = await _roomRepository.Get(roomId);

            Notify(RoomUpdateType.Removed, user, room);

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

            room.Participants.Add(new RoomParticipant(user));
            room.Updated = _dateTime.UtcNow;
            await _roomRepository.Update(room);

            Notify(RoomUpdateType.Entered, user, room);

            return room;
        }

        public async Task<Room> AcceptRequest(Guid roomId, Guid requestId, Guid userId)
        {
            var user = await _userRepository.Get(userId);
            var room = await _roomRepository.Get(roomId);

            if (room.StartDate < _dateTime.UtcNow)
            {
                throw new ValidationException(ValidationExceptionType.Rooms_Enter_AlreadyStarted);
            }

            var start = room.StartDate;
            var end = room.StartDate.AddMinutes(room.DurationInMinutes);

            var conflicts = await GetConflicts(userId, start, end);

            if (conflicts.Any())
            {
                throw new ValidationException(ValidationExceptionType.Rooms_Enter_Conflict);
            }

            var request = room.Requests.First(r => r.Id == requestId);
            request.Status = RoomRequestStatus.Accepted;
            request.Updated = _dateTime.UtcNow;

            if (request is EnterRoomRequest)
            {
                room.Participants.Add(new RoomParticipant(request.To));
                room.Updated = _dateTime.UtcNow;

                Notify(RoomUpdateType.Accepted, user, room);
            }

            await _roomRepository.Update(room);

            return room;
        }

        public async Task<Room> DeclineRequest(Guid roomId, Guid requestId, Guid userId)
        {
            var user = await _userRepository.Get(userId);
            var room = await _roomRepository.Get(roomId);

            var request = room.Requests.First(r => r.Id == requestId);
            request.Status = RoomRequestStatus.Declined;
            request.Updated = _dateTime.UtcNow;

            await _roomRepository.Update(room);

            if (request is EnterRoomRequest)
            {
                Notify(RoomUpdateType.Declined, user, room);
            }

            return room;
        }

        public async Task<Room> Leave(Guid roomId, Guid userId)
        {
            var user = await _userRepository.Get(userId);

            var room = await _roomRepository.Get(roomId);
            room.Participants.RemoveAll(p => p.Id == user.Id);
            await _roomRepository.Update(room);

            Notify(RoomUpdateType.Left, user, room);

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

            Notify(RoomUpdateType.Joined, user, room);
            return room;
        }

        public async Task<List<Room>> Last()
        {
            var all = await _roomRepository.Get();
            return all.Where(r => r.Participants.Count() == r.MaxParticipants)
                        .OrderByDescending(r => r.Updated)
                        .Take(8)
                        .ToList();
        }
    }
}
