using Investor.Core.Entity.ApplicationData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static Azure.Core.HttpHeader;
using System.Net;
using Investor.Core.Entity.PostData;
using Investor.Core.Entity.EvaluationData;
using Investor.Core.Entity.ConnectionData;
using Investor.Core.Entity.ChatandUserConnection;

namespace Investor.Core
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {

        //-----------------------------------------------------------------------------------
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CommentPost> CommentPosts { get; set; }
        public virtual DbSet<ReactPost> ReactPosts { get; set; }

        //-----------------------------------------------------------------------------------
        public virtual DbSet<EvaluationUser> EvaluationUsers { get; set; }
        public virtual DbSet<Connection> Connections { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<UserConnection> UserConnections { get; set; }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public ApplicationDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=LAPTOP-K8QC50ME;Database=Our gp;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().ToTable("Users", "dbo");
            modelBuilder.Entity<ApplicationRole>().ToTable("Role", "dbo");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRole", "dbo");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim", "dbo");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin", "dbo");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "dbo");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "dbo");

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(b => b.Posts);

            modelBuilder.Entity<EvaluationUser>()
                .HasOne(p => p.User)
                .WithMany(b => b.EvaluationUsers);

            modelBuilder.Entity<Connection>()
                .HasOne(p => p.User1)
                .WithMany(b => b.connections);

            modelBuilder.Entity<CommentPost>().HasKey(p => p.CommentId);

            modelBuilder.Entity<ReactPost>().HasKey(p => p.ReactId);

            modelBuilder.Entity<EvaluationUser>().HasKey(p => p.EvaluationId);

            modelBuilder.Entity<CommentPost>()
                .HasOne(p => p.Post)
                .WithMany(b => b.Comments);

            modelBuilder.Entity<ReactPost>()
                .HasOne(p => p.Post)
                .WithMany(b => b.Reacts);

        }
    }
}
