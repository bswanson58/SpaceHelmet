// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

using System;

namespace SpaceHelmet.Client.Gravatar.Models {
    public class Entry {
        public string       Id { get; set; }
        public string       Hash { get; set; }
        public string       RequestHash { get; set; }
        public string       ProfileUrl { get; set; }
        public string       PreferredUsername { get; set; }
        public string       ThumbnailUrl { get; set; }
        public Photo[]      Photos { get; set; }
        public Name         Name { get; set; }
        public string       DisplayName { get; set; }
        public string       AboutMe { get; set; }
        public string       CurrentLocation { get; set; }
        public Email[]      Emails { get; set; }
        public Account[]    Accounts { get; set; }
        public Url[]        Urls { get; set; }

        public Entry() {
            Id = String.Empty;
            Hash = String.Empty;
            RequestHash = String.Empty;
            ProfileUrl = String.Empty;
            PreferredUsername = String.Empty;
            ThumbnailUrl = String.Empty;
            Photos = Array.Empty<Photo>();
            Name = new Name();
            DisplayName = String.Empty;
            AboutMe = String.Empty;
            CurrentLocation = String.Empty;
            Emails = Array.Empty<Email>();
            Accounts = Array.Empty<Account>();
            Urls = Array.Empty<Url>();
        }
    }
}