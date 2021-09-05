namespace Lingua.Shared
{
    public class RoomParticipant : User
    {
        public RoomParticipant()
        {
        }

        public RoomParticipant(User user)
        {
            Id = user.Id;
            Lastname = user.Lastname;
            Firstname = user.Firstname;
            Email = user.Email;
            Country = user.Country;
            DateOfBirth = user.DateOfBirth;
            TargetLanguage = user.TargetLanguage;
            LanguageLevel = user.LanguageLevel;
            AvatarUrl = user.AvatarUrl;
            Timezone = user.Timezone;
        }
    }
}
