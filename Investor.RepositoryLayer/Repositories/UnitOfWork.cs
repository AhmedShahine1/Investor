using Investor.Core;
using Investor.Core.Entity.ApplicationData;
using Investor.Core.Entity.ChatAndNotification;
using Investor.Core.Entity.ConnectionData;
using Investor.Core.Entity.EvaluationData;
using Investor.Core.Entity.PostData;
using Investor.RepositoryLayer.Interfaces;

namespace Investor.RepositoryLayer.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IBaseRepository<ApplicationUser> Users { get; private set; }
    public IBaseRepository<ApplicationRole> Roles { get; private set; }

    public IBaseRepository<Post> Posts { get; private set; }
    public IBaseRepository<CommentPost> Comments { get; private set; }
    public IBaseRepository<ReactPost> Reacts { get; private set; }
    public IBaseRepository<Category> Catagories { get; private set; }

    public IBaseRepository<EvaluationUser> EvaluationUser {  get; private set; }

    public IBaseRepository<Connection> Connections { get; private set; }

    //public IBaseRepository<MessageChat> MessageChats { get; private set; }
    public IBaseRepository<UserConnection> UserConnections { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new BaseRepository<ApplicationUser>(_context);
        Roles = new BaseRepository<ApplicationRole>(_context);
        Posts = new BaseRepository<Post>(_context);
        Comments = new BaseRepository<CommentPost>(_context);
        Reacts = new BaseRepository<ReactPost>(_context);
        Catagories = new BaseRepository<Category>(_context);
        EvaluationUser = new BaseRepository<EvaluationUser>(_context);
        Connections = new BaseRepository<Connection>(_context);
        //MessageChats = new BaseRepository<MessageChat>(_context);
        UserConnections = new BaseRepository<UserConnection>(_context);
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}