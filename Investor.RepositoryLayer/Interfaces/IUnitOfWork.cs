using Investor.Core.Entity.ApplicationData;
using Investor.Core.Entity.ChatandUserConnection;
using Investor.Core.Entity.ConnectionData;
using Investor.Core.Entity.EvaluationData;
using Investor.Core.Entity.PostData;
using Microsoft.EntityFrameworkCore;

namespace Investor.RepositoryLayer.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IBaseRepository<ApplicationUser> Users { get; }
    IBaseRepository<ApplicationRole> Roles { get; }

    IBaseRepository<Post> Posts { get; }
    IBaseRepository<CommentPost> Comments { get; }
    IBaseRepository<ReactPost> Reacts { get; }
    IBaseRepository<Category> Catagories { get; }

    IBaseRepository<EvaluationUser> EvaluationUser { get; }

    IBaseRepository<Connection> Connections { get; }
    IBaseRepository<Chat> Chats { get; }
    IBaseRepository<UserConnection> UserConnections { get; }
    //-----------------------------------------------------------------------------------
    int SaveChanges();

    Task<int> SaveChangesAsync();
}