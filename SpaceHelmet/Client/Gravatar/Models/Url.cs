// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

using System;

namespace SpaceHelmet.Client.Gravatar.Models {
    public class Url {
        public string Value { get; set; }
        public string Title { get; set; }

        public Url() {
            Value = String.Empty;
            Title = String.Empty;
        }
    }
}

