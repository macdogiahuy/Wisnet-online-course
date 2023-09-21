using CourseHub.Core.Interfaces.Repositories;
using CourseHub.Core.Interfaces.Repositories.CommonRepos;
using CourseHub.Core.Interfaces.Repositories.CourseRepos;
using CourseHub.Core.Interfaces.Repositories.UserRepos;
using CourseHub.Infrastructure.AccessContext;
using CourseHub.Infrastructure.Repositories.CommonRepos;
using CourseHub.Infrastructure.Repositories.CourseRepos;
using CourseHub.Infrastructure.Repositories.UserRepos;

namespace CourseHub.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly Context _context;

    public UnitOfWork(Context context)
    {
        _context = context;
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }






    private UserRepository? _userRepo;
    public IUserRepository UserRepo { get => _userRepo ??= new UserRepository(_context); }



    private CategoryRepository? _categoryRepo;
    public ICategoryRepository CategoryRepo { get => _categoryRepo ??= new CategoryRepository(_context); }

    private InstructorRepository? _instructorRepo;
    public IInstructorRepository InstructorRepo { get => _instructorRepo ??= new InstructorRepository(_context); }

    private CourseRepository? _courseRepo;
    public ICourseRepository CourseRepo { get => _courseRepo ??= new CourseRepository(_context); }

    private NotificationRepository? _notificationRepo;
    public INotificationRepository NotificationRepo { get => _notificationRepo ??= new NotificationRepository(_context); }

    public SectionRepository? _sectionRepo;
    public ISectionRepository SectionRepo { get => _sectionRepo ??= new SectionRepository(_context); }

    public ICourseCouponRepository CourseCouponRepo => throw new NotImplementedException();

    public ICourseReviewRepository CourseReviewRepo => throw new NotImplementedException();

    public ILectureRepository LectureRepo => throw new NotImplementedException();





    public ICommentRepository CommentRepo => throw new NotImplementedException();

    public IReactionRepository ReactionRepo => throw new NotImplementedException();
}
