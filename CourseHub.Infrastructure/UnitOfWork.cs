using CourseHub.Core.Interfaces.Repositories;
using CourseHub.Core.Interfaces.Repositories.AssignmentRepos;
using CourseHub.Core.Interfaces.Repositories.CommonRepos;
using CourseHub.Core.Interfaces.Repositories.CourseRepos;
using CourseHub.Core.Interfaces.Repositories.PaymentRepos;
using CourseHub.Core.Interfaces.Repositories.SocialRepos;
using CourseHub.Core.Interfaces.Repositories.UserRepos;
using CourseHub.Infrastructure.AccessContext;
using CourseHub.Infrastructure.Repositories.CommonRepos;
using CourseHub.Infrastructure.Repositories.CourseRepos;
using CourseHub.Infrastructure.Repositories.PaymentRepos;
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

    public SectionRepository? _sectionRepo;
    public ISectionRepository SectionRepo { get => _sectionRepo ??= new SectionRepository(_context); }

    public LectureRepository _lectureRepo;
    public ILectureRepository LectureRepo { get => _lectureRepo ??= new LectureRepository(_context); }

    public ICourseCouponRepository CourseCouponRepo => throw new NotImplementedException();

    public ICourseReviewRepository CourseReviewRepo => throw new NotImplementedException();

    public EnrollmentRepository _enrollmentRepo;
    public IEnrollmentRepository EnrollmentRepo { get => _enrollmentRepo ??= new EnrollmentRepository(_context); }






    private NotificationRepository? _notificationRepo;
    public INotificationRepository NotificationRepo { get => _notificationRepo ??= new NotificationRepository(_context); }

    public ICommentRepository CommentRepo => throw new NotImplementedException();

    public IReactionRepository ReactionRepo => throw new NotImplementedException();






    private BillRepository? _billRepo;
    public IBillRepository BillRepo { get => _billRepo ??= new BillRepository(_context); }






    public IConversationRepository ConversationRepo => throw new NotImplementedException();

    public IChatMessageRepository ChatMessageRepo => throw new NotImplementedException();

    public ITagRepository TagRepo => throw new NotImplementedException();






    public IAssignmentRepository AssignmentRepo => throw new NotImplementedException();
}
