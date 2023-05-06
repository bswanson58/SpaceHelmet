using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SpaceHelmet.Shared.Constants;

namespace SpaceHelmet.Server.Database.Support {
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole> {
        public void Configure( EntityTypeBuilder<IdentityRole> builder ) {
            builder.HasData(
                new IdentityRole {
                    Name = ClaimValues.ClaimRoleUser,
                    NormalizedName = ClaimValues.ClaimRoleUser.ToUpper()
                },
                new IdentityRole {
                    Name = ClaimValues.ClaimRoleAdmin,
                    NormalizedName = ClaimValues.ClaimRoleAdmin.ToUpper()
                }
            );
        }
    }
}
