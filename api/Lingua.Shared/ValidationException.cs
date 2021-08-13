using System;

namespace Lingua.Shared
{
    public class ValidationException : Exception
    {

        public ValidationException(ValidationExceptionType type, string message = null) : base(type.ToString())
        {
            Type = type;
        }

        public ValidationExceptionType Type { get; }

        public string UserFriendlyMessage
        {
            get
            {
                switch (Type)
                {
                    case ValidationExceptionType.Rooms_Create_Conflict: return "Oops. You can’t create a room for that time. Seems like you already have another one booked for the same time.";
                    case ValidationExceptionType.Rooms_Enter_AlreadyStarted: return "Oops. Seems like the room is no longer available. Try another one.";
                    case ValidationExceptionType.Rooms_Enter_Conflict: return "Oops. You can’t enter that room. Seems like you already have another room booked for the same time.";
                    case ValidationExceptionType.Rooms_Enter_AlreadyFull: return "Oops. Seems like the room is no longer available. Try another one.";
                }

                return Message;
            }
        }

        public static class Types
        {
            public static class Rooms_Enter_AlreadyStarted { }
            public static class Rooms_Enter_AlreadyFull { }
            public static class Rooms_Enter_Conflict { }
            public static class Rooms_Create_Conflict { }
        }
    }

    public enum ValidationExceptionType
    {
        Rooms_Enter_AlreadyStarted,
        Rooms_Enter_AlreadyFull,
        Rooms_Enter_Conflict,
        Rooms_Create_Conflict
    }

}
