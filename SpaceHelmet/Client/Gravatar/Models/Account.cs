// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

using System;

namespace SpaceHelmet.Client.Gravatar.Models {
    public class Account {
        public string Domain { get; set; }
        public string Display { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Verified { get; set; }
        public string ShortName { get; set; }

        public Account() {
            Domain = String.Empty;
            Display = String.Empty;
            Url = String.Empty;
            Username = String.Empty;
            Verified = String.Empty;
            ShortName = String.Empty;
        }
    }
}
