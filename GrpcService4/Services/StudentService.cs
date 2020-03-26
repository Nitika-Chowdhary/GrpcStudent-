using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcService4.Protos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static GrpcService4.Protos.StudentSvc;

namespace GrpcService4.Services
{
    public class StudentService : StudentSvcBase
    {
        private readonly StudentContext _context;

        public StudentService()
        {
            _context = new StudentContext();
        }

        public override Task<Empty> CreateStudent(StudentProfile request, ServerCallContext context)
        {
            StudentPoco poco = new StudentPoco
            {
                Id = Guid.Parse(request.Identifer),
                Name = request.Name,
                Grade = request.Grade,
                DoB = request.DoB.ToDateTime()
            };

            _context.Students.Add(poco);
            _context.SaveChanges();

            return Task.FromResult<Empty>(null);
        }

        public override Task<StudentProfile> ReadStudent(Id request, ServerCallContext context)
        {
            Guid requestedGuid = Guid.Parse(request.Identifier);
            StudentPoco studentPoco = _context.Students.Where(s => s.Id == requestedGuid).FirstOrDefault();
            StudentProfile profile = new StudentProfile()
            {
                Identifer = studentPoco.Id.ToString(),
                Name = studentPoco.Name,
                Grade = studentPoco.Grade,
                DoB = Timestamp.FromDateTime(studentPoco.DoB)
            };

            return new Task<StudentProfile>(() => profile);
        }

        public override Task<Empty> UpdateStudent(StudentProfile request, ServerCallContext context)
        {
            StudentPoco poco = new StudentPoco
            {
                Id = Guid.Parse(request.Identifer),
                Name = request.Name,
                Grade = request.Grade,
                DoB = request.DoB.ToDateTime()
            };

            _context.Students.Update(poco);
            _context.SaveChanges();

            return Task.FromResult<Empty>(null);
        }

        public override Task<Empty> DeleteStudent(Id request, ServerCallContext context)
        {
            Guid requestedGuid = Guid.Parse(request.Identifier);
            StudentPoco studentPoco = 
                _context.Students.Where(s => s.Id == requestedGuid).FirstOrDefault();

            _context.Entry(studentPoco).State 
                = EntityState.Deleted;
            _context.SaveChanges();

            return Task.FromResult<Empty>(null);
        }
    }

    public class StudentContext : DbContext
    {
        public DbSet<StudentPoco> Students { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(
                @"Data Source=CSHARPHUMBER\HUMBERBRIDGING;Initial Catalog=STUDENT_PORTAL_DB;Integrated Security=True;");
            base.OnConfiguring(optionsBuilder);
        }
    }



    public class StudentPoco
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Grade { get; set; }
        public DateTime DoB { get; set; }
    }

}
