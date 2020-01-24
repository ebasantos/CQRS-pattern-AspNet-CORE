using CQRSPattern.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CQRSPattern.Commands.UOW
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly DbContext _context;

        public UnitOfWork(DbContext context) 
        {
            _context = context;
        }


        public int Commit() => _context.SaveChanges();

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
