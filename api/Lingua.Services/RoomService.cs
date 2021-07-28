using Lingua.Shared;
using Lingua.ZoomIntegration;
using Lingua.ZoomIntegration.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace Lingua.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITokenProvider _tokenProvider;
        private readonly IMeetingService _zoomMeetingService;
        private readonly IDateTimeProvider _dateTime;

        public RoomService(IRoomRepository roomRepository,
                            IUserRepository userRepository,
                            ITokenProvider tokenProvider,
                            IMeetingService zoomMeetingService,
                            IDateTimeProvider dateTime)
        {
            _roomRepository = roomRepository;
            _userRepository = userRepository;
            _tokenProvider = tokenProvider;
            _zoomMeetingService = zoomMeetingService;
            _dateTime = dateTime;
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
                rooms = ApplySearchFilter(options, rooms);
            }

            return rooms.ToList();
        }

        private static IEnumerable<Room> ApplySearchFilter(SearchRoomOptions options, IEnumerable<Room> rooms)
        {
            if (options?.Levels?.Any() == true)
            {
                rooms = rooms.Where(r => options.Levels.Contains(r.Host.LanguageLevel));
            }

            if (options?.Days?.Any() == true)
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(options.Timezone));
                rooms = rooms.Where(r => options.Days.Contains(TimeZoneInfo.ConvertTimeFromUtc(r.StartDate, tz).DayOfWeek));
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

        public async Task<Room> Create(CreateRoomOptions options, Guid userId)
        {
            var user = await _userRepository.Get(userId);
            var start = options.StartDate;
            var end = options.StartDate.AddMinutes(options.DurationInMinutes);

            var conflicts = (await _roomRepository.Get(r =>
                                r.StartDate < end
                                && start < r.EndDate
                                && r.Participants.Any(p => p.Id == userId)))
                            .Where(r => r.StartDate > _dateTime.UtcNow || r.Participants.Count == r.MaxParticipants);

            if (conflicts.Any())
            {
                throw new Exception("You have conflicting rooms for this time frame");
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
            //await _emailService.SendAsync("Test", "Test", room.Participants.Select(p => p.Email).ToArray());

            return room;
        }

        public async Task<Room> Remove(Guid roomId, Guid userId)
        {
            await _roomRepository.Remove(roomId);
            var room = await _roomRepository.Get(roomId);
            //await _emailService.SendAsync("Test", "Test", room.Participants.Select(p => p.Email).ToArray());

            return room;
        }

        public async Task<Room> Enter(Guid roomId, Guid userId)
        {
            var user = await _userRepository.Get(userId);
            var room = await _roomRepository.Get(roomId);

            if (room.StartDate < _dateTime.UtcNow)
            {
                throw new Exception("This room is already started.");
            }
            if (room.Participants.Any(p => p.Id == userId))
            {
                throw new Exception("You have already entered this room.");
            }
            if (room.Participants.Count == room.MaxParticipants)
            {
                throw new Exception("This room is already full.");
            }

            var start = room.StartDate;
            var end = room.StartDate.AddMinutes(room.DurationInMinutes);

            var conflicts = await _roomRepository.Get(r =>
                                r.StartDate < end
                                && start < r.EndDate
                                && r.Participants.Any(p => p.Id == userId));

            if (conflicts.Any())
            {
                throw new Exception("You have conflicting rooms for this time frame");
            }

            if (room.Participants.Count == room.MaxParticipants)
            {
                throw new Exception("Room is already full");
            }

            room.Participants.Add(user);
            await _roomRepository.Update(room);
            //await _emailService.SendAsync("Test", "Test", room.Participants.Select(p => p.Email).ToArray());

            return room;
        }

        public async Task<Room> Leave(Guid roomId, Guid userId)
        {
            var user = await _userRepository.Get(userId);

            var room = await _roomRepository.Get(roomId);
            room.Participants.RemoveAll(p => p.Id == user.Id);
            await _roomRepository.Update(room);
            //await _emailService.SendAsync("Test", "Test", room.Participants.Select(p => p.Email).ToArray());

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

            if (accessTokens == null)
            {
                throw new Exception("User does not have zoom access");
            }

            var updated = await _tokenProvider.UseToken(accessTokens, async (accessTokens) =>
            {

                var request = new CreateMeetingRequest
                {
                    Topic = room.Topic,
                    Duration = room.DurationInMinutes,
                    StartTime = room.StartDate,
                    Type = MeetingType.Scheduled,
                    Timezone = "UTC"
                };

                var meeting = await _zoomMeetingService.CreateMeeting(accessTokens.AccessToken, request);
                room.JoinUrl = meeting.JoinUrl;
                await _roomRepository.Update(room);
            });

            if (updated)
            {
                await _userRepository.Update(user);
            }

            return room;

        }
    }
}
