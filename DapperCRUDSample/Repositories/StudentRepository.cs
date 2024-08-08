using Dapper;
using DapperCRUDSample.Entities;
using DapperCRUDSample.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DapperCRUDSample.Repositories
{
    public class StudentRepository(AppDbContext dbContext) : IStudentRepository
    {
        #region Dapper
        public async Task DapperCreateAsync(Student student)
        {
            using var connection = new SqlConnection(dbContext.Database.GetConnectionString());

            string insertQuery = """
                INSERT INTO Student (FirstName,LastName,Age)
                VALUES (@FirstName,@LastName,@Age)
                """;

            await connection.ExecuteAsync(insertQuery, student);
        }

        public async Task DapperDeleteAsync(int studentId)
        {
            using var connection = new SqlConnection(dbContext.Database.GetConnectionString());

            string deleteQuery = $"""
                DELETE Student
                WHERE Id={studentId}
                """;

            var affectedRow = await connection.ExecuteAsync(deleteQuery);

            if (affectedRow == 0)
                throw new BadHttpRequestException("student is invalid");
        }

        public async Task<Student> DapperGetAsync(int studentId)
        {
            using var connection = new SqlConnection(dbContext.Database.GetConnectionString());

            string getQuery = $"""
                SELECT * FROM Student
                WHERE Id={studentId}
                """;

            var student = await connection.QueryFirstOrDefaultAsync<Student>(getQuery);

            return student is not null ? student : throw new BadHttpRequestException("studentId is invalid");
        }

        public async Task<IEnumerable<Student>> DapperListAsync()
        {
            using var connection = new SqlConnection(dbContext.Database.GetConnectionString());

            string getQuery = $"""
                SELECT * FROM Student
                """;

            return await connection.QueryAsync<Student>(getQuery);
        }

        public async Task DapperUpdateAsync(Student student)
        {
            using var connection = new SqlConnection(dbContext.Database.GetConnectionString());

            string updateQuery = $"""
                UPDATE Student
                SET FirstName=@FirstName,LastName=@LastName,Age=@Age
                WHERE Id=@Id
                """;

            var affectedRow = await connection.ExecuteAsync(updateQuery,student);

            if (affectedRow == 0)
                throw new BadHttpRequestException("student is invalid");
        }
        #endregion

        #region EFCore
        public async Task EFCreateAsync(Student student)
        {
            student.Id = 0;
            dbContext.Students.Add(student);
            await dbContext.SaveChangesAsync();
        }

        public async Task EFDeleteAsync(int studentId)
        {
            if (!await dbContext.Students.AnyAsync(d => d.Id == studentId))
            {
                throw new BadHttpRequestException("studentId is invalid");
            }
            await dbContext.Students
                .Where(d => d.Id == studentId)
                  .ExecuteDeleteAsync();
        }

        public async Task<Student> EFGetAsync(int studentId)
        {
            if (!await dbContext.Students.AnyAsync(d => d.Id == studentId))
            {
                throw new BadHttpRequestException("studentId is invalid");
            }

            return await dbContext.Students
                .AsNoTracking()
                .FirstAsync(d => d.Id == studentId);
        }

        public async Task<IEnumerable<Student>> EFListAsync()
        {
            return await dbContext.Students
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task EFUpdateAsync(Student student)
        {
            if (!await dbContext.Students.AnyAsync(d => d.Id == student.Id))
            {
                throw new BadHttpRequestException("student is invalid");
            }

            dbContext.Students.Update(student);

            await dbContext.SaveChangesAsync();
        }
        #endregion
    }
}
