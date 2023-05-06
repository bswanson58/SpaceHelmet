// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

using System;

namespace SpaceHelmet.Client.Gravatar.Models {
    internal class GravatarResponse {
        public Entry[] Entry { get; set; }

        public GravatarResponse() {
            Entry = Array.Empty<Entry>();
        }
    }
}
