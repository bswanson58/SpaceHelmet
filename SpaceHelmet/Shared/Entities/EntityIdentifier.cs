using System.Diagnostics;

namespace SpaceHelmet.Shared.Entities {
    [DebuggerDisplay("Id:{" + nameof( Value ) + "}")]
    public sealed class EntityIdentifier : IEquatable<EntityIdentifier> {
        public  string Value { get; }

        public  static implicit operator string( EntityIdentifier issueId ) => issueId.Value;

        private EntityIdentifier( string entityId ) {
            if(!IsValid( entityId ) ) {
                throw new ArgumentNullException( entityId );
            }

            Value = entityId;
        }

        private static bool IsValid( string value ) {
            return !String.IsNullOrWhiteSpace( value );
        }

        public static EntityIdentifier Default => new ( "default" );

        public static EntityIdentifier CreateIdOrThrow( string entityId ) {
            var retValue = new EntityIdentifier( entityId );

            if(!EntityIdentifier.IsValid( entityId )) {
                throw new ApplicationException( "entity ID could not be created" );
            }

            return retValue;
        }

        public static EntityIdentifier CreateNew() {
            return new EntityIdentifier( Guid.NewGuid().ToString());
        }

        // Equality:
        public override bool Equals( object? obj ) => Equals( obj as EntityIdentifier );

        public bool Equals( EntityIdentifier? right ) {
            if( right is null ) {
                return false;
            }

            // Optimization for a common success case.
            if( ReferenceEquals( this, right )) {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if( GetType() != right.GetType()) {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return Value == right.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator == ( EntityIdentifier ? lhs, EntityIdentifier ? rhs ) {
            if( lhs is null ) {
                if( rhs is null ) {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals( rhs );
        }

        public static bool operator !=( EntityIdentifier lhs, EntityIdentifier rhs ) => !( lhs == rhs );
    }
}
