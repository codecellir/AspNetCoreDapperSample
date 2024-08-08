using DapperCRUDSample.Entities;

namespace DapperCRUDSample.Repositories
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> EFListAsync();
        Task<Student> EFGetAsync(int studentId);
        Task EFDeleteAsync(int studentId);
        Task EFCreateAsync(Student student);
        Task EFUpdateAsync(Student student);

        Task<IEnumerable<Student>> DapperListAsync();
        Task<Student> DapperGetAsync(int studentId);
        Task DapperDeleteAsync(int studentId);
        Task DapperCreateAsync(Student student);
        Task DapperUpdateAsync(Student student);
    }
}
