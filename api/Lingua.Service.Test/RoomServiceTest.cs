using Lingua.Services;
using Lingua.Shared;
using Lingua.ZoomIntegration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;

namespace Lingua.Service.Test
{
    [TestClass]
    public class RoomServiceTest
    {
        [TestMethod]
        public void Available_ReturnAvailableRooms()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var host = new User { Firstname = "Host", Lastname = "Host", TargetLanguage = "English" };
            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            dateTimeMock.SetupGet(m => m.UtcNow).Returns(new DateTime(2020, 1, 1, 12, 0, 0));
            var rooms = new List<Room>
            {
                new Room {
                    HostUserId = host.Id,
                    StartDate = new DateTime(2020, 1, 1, 12, 30, 0),
                    EndDate = new DateTime(2020, 1, 1, 13, 0, 0),
                    DurationInMinutes = 30,
                    Language = "English",
                    MaxParticipants = 2,
                    Participants = new List<User>{  host }
                }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns((Expression<Func<Room, bool>> filter) => Task.FromResult(rooms.Where(filter.Compile())));

            //act

            var available = service.Available(null, user.Id).Result;

            //assert
            Assert.AreEqual(1, available.Count);

        }

        [TestMethod]
        public void Available_DoNotReturnRoom_IfRoomAlreadyStarted()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var host = new User { Firstname = "Host", Lastname = "Host", TargetLanguage = "English" };
            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            dateTimeMock.SetupGet(m => m.UtcNow).Returns(new DateTime(2020, 1, 1, 12, 0, 0));
            var rooms = new List<Room>
            {
                new Room {
                    HostUserId = host.Id,
                    StartDate = new DateTime(2020, 1, 1, 11, 20, 0),
                    EndDate = new DateTime(2020, 1, 1, 11, 50, 0),
                    DurationInMinutes = 30,
                    Language = "English",
                    MaxParticipants = 2,
                    Participants = new List<User>{  host }
                }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns((Expression<Func<Room, bool>> filter) => Task.FromResult(rooms.Where(filter.Compile())));

            //act

            var available = service.Available(null, user.Id).Result;

            //assert
            Assert.AreEqual(0, available.Count);
        }

        [TestMethod]
        public void Available_DoNotReturnRoom_IfRoomHasOtherTargetLanguage()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var host = new User { Firstname = "Host", Lastname = "Host", TargetLanguage = "English" };
            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            dateTimeMock.SetupGet(m => m.UtcNow).Returns(new DateTime(2020, 1, 1, 12, 0, 0));
            var rooms = new List<Room>
            {
                new Room {
                    HostUserId = host.Id,
                    StartDate = new DateTime(2020, 1, 1, 13, 20 ,0),
                    EndDate = new DateTime(2020, 1, 1, 13, 50 ,0),
                    DurationInMinutes = 30,
                    Language = "Russian",
                    MaxParticipants = 2,
                    Participants = new List<User>{  host }
                }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns((Expression<Func<Room, bool>> filter) => Task.FromResult(rooms.Where(filter.Compile())));

            //act

            var available = service.Available(null, user.Id).Result;

            //assert
            Assert.AreEqual(0, available.Count);
        }

        [TestMethod]
        public void Available_DoNotReturnRoom_IfRoomIsFull()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var host = new User { Firstname = "Host", Lastname = "Host", TargetLanguage = "English" };
            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            dateTimeMock.SetupGet(m => m.UtcNow).Returns(new DateTime(2020, 1, 1, 12, 0, 0));
            var rooms = new List<Room>
            {
                new Room {
                    HostUserId = host.Id,
                    StartDate = new DateTime(2020, 1, 1, 12, 20 ,0),
                    EndDate = new DateTime(2020, 1, 1, 12, 50 ,0),
                    DurationInMinutes = 30,
                    Language = "English",
                    MaxParticipants = 2,
                    Participants = new List<User>{  host , new User() }
                }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns((Expression<Func<Room, bool>> filter) => Task.FromResult(rooms.Where(filter.Compile())));

            //act

            var available = service.Available(null, user.Id).Result;

            //assert
            Assert.AreEqual(0, available.Count);
        }

        [TestMethod]
        public void Available_DoNotReturnRoom_IfRoomIsCreatedByUser()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var host = new User { Firstname = "Host", Lastname = "Host", TargetLanguage = "English" };
            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            dateTimeMock.SetupGet(m => m.UtcNow).Returns(new DateTime(2020, 1, 1, 12, 0, 0));
            var rooms = new List<Room>
            {
                new Room {
                    HostUserId = user.Id,
                    StartDate = new DateTime(2020, 1, 1, 12, 20 ,0),
                    EndDate = new DateTime(2020, 1, 1, 12, 50 ,0),
                    DurationInMinutes = 30,
                    Language = "English",
                    MaxParticipants = 2,
                    Participants = new List<User>{  user }
                }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns((Expression<Func<Room, bool>> filter) => Task.FromResult(rooms.Where(filter.Compile())));

            //act

            var available = service.Available(null, user.Id).Result;

            //assert
            Assert.AreEqual(0, available.Count);

        }





        [TestMethod]
        public void Upcoming_ReturnRoom_IfUserIsHost()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var host = new User { Firstname = "Host", Lastname = "Host", TargetLanguage = "English" };
            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            dateTimeMock.SetupGet(m => m.UtcNow).Returns(new DateTime(2020, 1, 1, 12, 0, 0));
            var rooms = new List<Room>
            {
                new Room {
                    HostUserId = user.Id,
                    StartDate = new DateTime(2020, 1, 1, 12, 30, 0),
                    EndDate = new DateTime(2020, 1, 1, 13, 0, 0),
                    DurationInMinutes = 30,
                    Language = "English",
                    MaxParticipants = 2,
                    Participants = new List<User>{  user }
                }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns((Expression<Func<Room, bool>> filter) => Task.FromResult(rooms.Where(filter.Compile())));

            //act

            var upcoming = service.Upcoming(user.Id).Result;

            //assert
            Assert.AreEqual(1, upcoming.Count);

        }

        [TestMethod]
        public void Available_DoNotReturnRoom_IfUserJoinedRoom()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var host = new User { Firstname = "Host", Lastname = "Host", TargetLanguage = "English" };
            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            dateTimeMock.SetupGet(m => m.UtcNow).Returns(new DateTime(2020, 1, 1, 12, 0, 0));
            var rooms = new List<Room>
            {
                new Room {
                    HostUserId = host.Id,
                    StartDate = new DateTime(2020, 1, 1, 12, 30, 0),
                    EndDate = new DateTime(2020, 1, 1, 13, 0, 0),
                    DurationInMinutes = 30,
                    Language = "English",
                    MaxParticipants = 2,
                    Participants = new List<User>{  host, user }
                }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns((Expression<Func<Room, bool>> filter) => Task.FromResult(rooms.Where(filter.Compile())));

            //act

            var upcoming = service.Upcoming(user.Id).Result;

            //assert
            Assert.AreEqual(1, upcoming.Count);
        }

        [TestMethod]
        public void Available_DoNotReturnRoom_IfUserIsNotParticipant()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var host = new User { Firstname = "Host", Lastname = "Host", TargetLanguage = "English" };
            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            dateTimeMock.SetupGet(m => m.UtcNow).Returns(new DateTime(2020, 1, 1, 12, 0, 0));
            var rooms = new List<Room>
            {
                new Room {
                    HostUserId = host.Id,
                    StartDate = new DateTime(2020, 1, 1, 12, 30, 0),
                    EndDate = new DateTime(2020, 1, 1, 13, 0, 0),
                    DurationInMinutes = 30,
                    Language = "English",
                    MaxParticipants = 2,
                    Participants = new List<User>{  host }
                }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns((Expression<Func<Room, bool>> filter) => Task.FromResult(rooms.Where(filter.Compile())));

            //act

            var upcoming = service.Upcoming(user.Id).Result;

            //assert
            Assert.AreEqual(0, upcoming.Count);
        }

        [TestMethod]
        public void Available_DoNotReturnRoom_IfNobodyJoinedAndRoomStarted()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var host = new User { Firstname = "Host", Lastname = "Host", TargetLanguage = "English" };
            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            dateTimeMock.SetupGet(m => m.UtcNow).Returns(new DateTime(2020, 1, 1, 12, 0, 0));
            var rooms = new List<Room>
            {
                new Room {
                    HostUserId = user.Id,
                    StartDate = new DateTime(2020, 1, 1, 11, 50, 0),
                    EndDate = new DateTime(2020, 1, 1, 12, 20, 0),
                    DurationInMinutes = 30,
                    Language = "English",
                    MaxParticipants = 2,
                    Participants = new List<User>{  user }
                }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns((Expression<Func<Room, bool>> filter) => Task.FromResult(rooms.Where(filter.Compile())));

            //act

            var upcoming = service.Upcoming(user.Id).Result;

            //assert
            Assert.AreEqual(0, upcoming.Count);
        }

        [TestMethod]
        public void Available_ReturnRoom_IfRoomStartedButItIsFull()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var host = new User { Firstname = "Host", Lastname = "Host", TargetLanguage = "English" };
            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            dateTimeMock.SetupGet(m => m.UtcNow).Returns(new DateTime(2020, 1, 1, 12, 0, 0));
            var rooms = new List<Room>
            {
                new Room {
                    HostUserId = host.Id,
                    StartDate = new DateTime(2020, 1, 1, 11, 50, 0),
                    EndDate = new DateTime(2020, 1, 1, 12, 20, 0),
                    DurationInMinutes = 30,
                    Language = "English",
                    MaxParticipants = 2,
                    Participants = new List<User>{  host, user }
                }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns((Expression<Func<Room, bool>> filter) => Task.FromResult(rooms.Where(filter.Compile())));

            //act

            var upcoming = service.Upcoming(user.Id).Result;

            //assert
            Assert.AreEqual(1, upcoming.Count);
        }




        [TestMethod]
        public void Create_ShouldCreateCorrectRoomBasedOnOptions()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            //act

            var options = new CreateRoomOptions
            {
                DurationInMinutes = 30,
                Language = "English",
                StartDate = new DateTime(2020, 1, 1, 12, 0, 0),
                Topic = "topic"
            };

            var room = service.Create(options, user.Id).Result;

            //assert
            var expected = new Room
            {
                HostUserId = user.Id,
                StartDate = new DateTime(2020, 1, 1, 12, 0, 0),
                EndDate = new DateTime(2020, 1, 1, 12, 30, 0),
                DurationInMinutes = 30,
                Language = "English",
                MaxParticipants = 2,
                Participants = new List<User> { user },
                Topic = "topic"
            };

            expected.Should().BeEquivalentTo(room, options =>
                options.Excluding(r => r.Created)
                        .Excluding(r => r.Updated)
                        .Excluding(r => r.Id)
            );
        }

        [TestMethod]
        public void Create_ShouldCallRepositoryAndReturnSameItem()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));
            Room savedRoom = null;
            roomRepoMock.Setup(r => r.Create(It.IsAny<Room>())).Callback((Room room) => savedRoom = Helper.DeepClone(room));

            //act

            var options = new CreateRoomOptions
            {
                DurationInMinutes = 30,
                Language = "English",
                StartDate = new DateTime(2020, 1, 1, 12, 0, 0),
                Topic = "topic"
            };

            var room = service.Create(options, user.Id).Result;

            //assert
            roomRepoMock.Verify(r => r.Create(It.IsAny<Room>()));
            savedRoom.Should().BeEquivalentTo(room);
        }

        [TestMethod]
        public void Create_ShouldReturnConflict_IfRoomEndingAfterStartDateAndStartingBeforeStartDateExist()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            var rooms = new List<Room>
            {
                new Room {
                    HostUserId = user.Id,
                    StartDate = new DateTime(2020, 1, 1, 11, 40, 0),
                    EndDate = new DateTime(2020, 1, 1, 12, 10, 0),
                    DurationInMinutes = 30,
                    Language = "English",
                    MaxParticipants = 2,
                    Participants = new List<User>{  user }
                }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns((Expression<Func<Room, bool>> filter) => Task.FromResult(rooms.Where(filter.Compile())));

            //act

            var options = new CreateRoomOptions
            {
                DurationInMinutes = 30,
                Language = "English",
                StartDate = new DateTime(2020, 1, 1, 12, 0, 0),
                Topic = "topic"
            };

            Func<Task<Room>> action = () => service.Create(options, user.Id);

            //assert
            Assert.ThrowsExceptionAsync<ValidationException>(action, ValidationExceptionType.Rooms_Create_Conflict.ToString()).Wait();
        }

        [TestMethod]
        public void Create_ShouldReturnConflict_IfRoomEndingAfterEndDateStartingAfterStartDateExist()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            var rooms = new List<Room>
            {
                new Room {
                    HostUserId = user.Id,
                    StartDate = new DateTime(2020, 1, 1, 12, 10, 0),
                    EndDate = new DateTime(2020, 1, 1, 12, 50, 0),
                    DurationInMinutes = 30,
                    Language = "English",
                    MaxParticipants = 2,
                    Participants = new List<User>{  user }
                }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns((Expression<Func<Room, bool>> filter) => Task.FromResult(rooms.Where(filter.Compile())));

            //act

            var options = new CreateRoomOptions
            {
                DurationInMinutes = 30,
                Language = "English",
                StartDate = new DateTime(2020, 1, 1, 12, 0, 0),
                Topic = "topic"
            };

            Func<Task<Room>> action = () => service.Create(options, user.Id);

            //assert
            Assert.ThrowsExceptionAsync<ValidationException>(action, ValidationExceptionType.Rooms_Create_Conflict.ToString()).Wait();
        }

        [TestMethod]
        public void Create_ShouldReturnConflict_IfRoomEndingAfterFinishDateAndStartingBeforeStartDateExist()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            var rooms = new List<Room>
            {
                new Room {
                    HostUserId = user.Id,
                    StartDate = new DateTime(2020, 1, 1, 11, 40, 0),
                    EndDate = new DateTime(2020, 1, 1, 12, 40, 0),
                    DurationInMinutes = 60,
                    Language = "English",
                    MaxParticipants = 2,
                    Participants = new List<User>{  user }
                }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns((Expression<Func<Room, bool>> filter) => Task.FromResult(rooms.Where(filter.Compile())));

            //act

            var options = new CreateRoomOptions
            {
                DurationInMinutes = 30,
                Language = "English",
                StartDate = new DateTime(2020, 1, 1, 12, 0, 0),
                Topic = "topic"
            };

            Func<Task<Room>> action = () => service.Create(options, user.Id);

            //assert
            Assert.ThrowsExceptionAsync<ValidationException>(action, ValidationExceptionType.Rooms_Create_Conflict.ToString()).Wait();
        }

        [TestMethod]
        public void Create_ShouldReturnConflict_IfRoomEndingBeforeFinishDateAndStartingAfterStartDateExist()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            var rooms = new List<Room>
            {
                new Room {
                    HostUserId = user.Id,
                    StartDate = new DateTime(2020, 1, 1, 12, 10, 0),
                    EndDate = new DateTime(2020, 1, 1, 12, 40, 0),
                    DurationInMinutes = 30,
                    Language = "English",
                    MaxParticipants = 2,
                    Participants = new List<User>{  user }
                }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns((Expression<Func<Room, bool>> filter) => Task.FromResult(rooms.Where(filter.Compile())));

            //act

            var options = new CreateRoomOptions
            {
                DurationInMinutes = 60,
                Language = "English",
                StartDate = new DateTime(2020, 1, 1, 12, 0, 0),
                Topic = "topic"
            };

            Func<Task<Room>> action = () => service.Create(options, user.Id);

            //assert
            Assert.ThrowsExceptionAsync<ValidationException>(action, ValidationExceptionType.Rooms_Create_Conflict.ToString()).Wait();
        }

        [TestMethod]
        public void Create_ShouldNotCallRepository_IfConflictFound()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            var rooms = new List<Room>
            {
                new Room {
                    HostUserId = user.Id,
                    StartDate = new DateTime(2020, 1, 1, 12, 10, 0),
                    EndDate = new DateTime(2020, 1, 1, 12, 40, 0),
                    DurationInMinutes = 30,
                    Language = "English",
                    MaxParticipants = 2,
                    Participants = new List<User>{  user }
                }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns((Expression<Func<Room, bool>> filter) => Task.FromResult(rooms.Where(filter.Compile())));

            //act

            var options = new CreateRoomOptions
            {
                DurationInMinutes = 60,
                Language = "English",
                StartDate = new DateTime(2020, 1, 1, 12, 0, 0),
                Topic = "topic"
            };

            Func<Task<Room>> action = () => service.Create(options, user.Id);

            //assert
            roomRepoMock.Verify(m => m.Create(It.IsAny<Room>()), Times.Never);
        }

        [TestMethod]
        public void Create_ShouldNotReturnConflict_IfExistingConflictingRoomIsUnattended()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));
            dateTimeMock.SetupGet(m => m.UtcNow).Returns(new DateTime(2020, 1, 1, 12, 0, 0));

            var rooms = new List<Room>
            {
                new Room {
                    HostUserId = user.Id,
                    StartDate = new DateTime(2020, 1, 1, 11, 50, 0),
                    EndDate = new DateTime(2020, 1, 1, 12, 20, 0),
                    DurationInMinutes = 30,
                    Language = "English",
                    MaxParticipants = 2,
                    Participants = new List<User>{  user }
                }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Room, bool>>>()))
                .Returns((Expression<Func<Room, bool>> filter) => Task.FromResult(rooms.Where(filter.Compile())));

            //act

            var options = new CreateRoomOptions
            {
                DurationInMinutes = 60,
                Language = "English",
                StartDate = new DateTime(2020, 1, 1, 12, 10, 0),
                Topic = "topic"
            };

            service.Create(options, user.Id).Wait();

            //assert
            //no exception
        }



        [TestMethod]
        public void Update_ShouldUpdatePropertiesCorrectlyBasedOnOptions()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            var existing = new Room
            {
                HostUserId = user.Id,
                StartDate = new DateTime(2020, 1, 1, 11, 50, 0),
                EndDate = new DateTime(2020, 1, 1, 12, 20, 0),
                DurationInMinutes = 30,
                Language = "English",
                MaxParticipants = 2,
                Participants = new List<User> { user }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Guid>()))
                .Returns(Task.FromResult(existing));

            //act

            var options = new UpdateRoomOptions
            {
                RoomId = existing.Id,
                DurationInMinutes = 60,
                StartDate = new DateTime(2020, 1, 1, 12, 0, 0),
                Topic = "topic"
            };

            var expected = Helper.DeepClone(existing);
            expected.DurationInMinutes = 60;
            expected.StartDate = new DateTime(2020, 1, 1, 12, 0, 0);
            expected.EndDate = new DateTime(2020, 1, 1, 13, 00, 0);
            expected.Topic = "topic";

            var updated = service.Update(options, user.Id).Result;

            //assert
            expected.Should().BeEquivalentTo(updated);
        }

        [TestMethod]
        public void Update_ShouldCallRepositoryAndReturnSameItem()
        {
            //arrange

            var zoomAuthMock = new Mock<IAuthClient>();
            var zoomMettingsMock = new Mock<IMeetingClient>();

            var roomRepoMock = new Mock<IRoomRepository>();
            var userRepoMock = new Mock<IUserRepository>();
            var dateTimeMock = new Mock<IDateTimeProvider>();

            var service = new RoomService(roomRepoMock.Object, userRepoMock.Object, zoomMettingsMock.Object, dateTimeMock.Object, Mock.Of<IEmailService>(), Mock.Of<ITemplateProvider>());

            var user = new User { Firstname = "John", Lastname = "Doe", TargetLanguage = "English" };
            userRepoMock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(Task.FromResult(user));

            var existing = new Room
            {
                HostUserId = user.Id,
                StartDate = new DateTime(2020, 1, 1, 11, 50, 0),
                EndDate = new DateTime(2020, 1, 1, 12, 20, 0),
                DurationInMinutes = 30,
                Language = "English",
                MaxParticipants = 2,
                Participants = new List<User> { user }
            };

            roomRepoMock.Setup(r => r.Get(It.IsAny<Guid>()))
                .Returns(Task.FromResult(existing));

            Room savedRoom = null;
            roomRepoMock.Setup(r => r.Update(It.IsAny<Room>())).Callback((Room room) => savedRoom = Helper.DeepClone(room));

            //act

            var options = new UpdateRoomOptions
            {
                RoomId = existing.Id,
                DurationInMinutes = 60,
                StartDate = new DateTime(2020, 1, 1, 12, 0, 0),
                Topic = "topic"
            };

            var room = service.Update(options, user.Id).Result;

            //assert
            roomRepoMock.Verify(r => r.Update(It.IsAny<Room>()));
            savedRoom.Should().BeEquivalentTo(room);
        }




    }

    public static class Helper
    {
        public static T DeepClone<T>(T item)
        {
            var serialized = JsonConvert.SerializeObject(item);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
