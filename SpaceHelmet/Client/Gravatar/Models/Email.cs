// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

using System;

namespace SpaceHelmet.Client.Gravatar.Models {
    public class Email {
        public string Primary { get; set; }
        public string Value { get; set; }

        public Email() {
            Primary = String.Empty;
            Value = String.Empty;
        }
    }
}
