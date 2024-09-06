using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable(nameof(Post));
    }
}

internal sealed class UserPostLikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.ToTable(nameof(PostLike));

        builder.HasKey(e => new { e.UserId, e.PostId }); // user can like a post only once
    }
}

internal sealed class UserPostCommentConfiguration : IEntityTypeConfiguration<PostComment>
{
    public void Configure(EntityTypeBuilder<PostComment> builder)
    {
        builder.ToTable(nameof(PostComment));

        builder.Property(e => e.Comment)
            .HasMaxLength(1000)
            .IsUnicode(false);
    }
}

internal sealed class UserPostTagConfiguration : IEntityTypeConfiguration<PostTag>
{
    public void Configure(EntityTypeBuilder<PostTag> builder)
    {
        builder.ToTable(nameof(PostTag));

        builder.Property(e => e.Tag)
            .HasMaxLength(100);
    }
}