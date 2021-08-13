using System;

namespace Lingua.API.ViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string TargetLanguage { get; set; }
        public string LanguageLevel { get; set; }
        public string AvatarUrl { get; set; }
    }
}
