namespace SpaceHelmet.Shared.Entities {
    public class EntityBase {
        public  string  EntityId { get; }
        public  string  UserId { get; }

        protected EntityBase() {
            EntityId = EntityIdentifier.CreateNew();
            UserId = EntityIdentifier.CreateNew();
        }

        protected EntityBase( ShUser forUser ) {
            EntityId = EntityIdentifier.CreateNew();
            UserId = forUser.EntityId;
        }

        protected EntityBase( string entityId, string userId ) {
            EntityId = EntityIdentifier.CreateIdOrThrow( entityId );
            UserId = EntityIdentifier.CreateIdOrThrow( userId );
        }
    }
}
