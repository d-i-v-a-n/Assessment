namespace Persistence.Configurations;

//internal sealed class PostConfiguration : IEntityTypeConfiguration<Post>
//{
//    public void Configure(EntityTypeBuilder<Post> builder)
//    {
//        builder.ToTable(nameof(Post));
//    }
//}

//internal sealed class UserPostLikeConfiguration : IEntityTypeConfiguration<UserPostLike>
//{
//    public void Configure(EntityTypeBuilder<UserPostLike> builder)
//    {
//        builder.ToTable(nameof(UserPostLike));

//        builder.HasKey(e => new { e.UserId, e.PostId }); // user can like a post only once
//    }
//}

//internal sealed class UserPostCommentConfiguration : IEntityTypeConfiguration<UserPostComment>
//{
//    public void Configure(EntityTypeBuilder<UserPostComment> builder)
//    {
//        builder.ToTable(nameof(UserPostComment));

//        builder.Property(e => e.Comment)
//            .HasMaxLength(1000)
//            .IsUnicode(false);
//    }
//}

//internal sealed class UserPostTagConfiguration : IEntityTypeConfiguration<UserPostTag>
//{
//    public void Configure(EntityTypeBuilder<UserPostTag> builder)
//    {
//        builder.ToTable(nameof(UserPostTag));

//        builder.Property(e => e.Tag)
//            .HasMaxLength(100);
//    }
//}