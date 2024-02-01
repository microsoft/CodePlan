using System;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Data;

namespace SFA.DAS.QnA.Application.UnitTests
{
    public class DataContextHelpers
    {
        public static QnaDataContext GetInMemoryDataContext()
        {
            var dbContextOptions = new DbContextOptionsBuilder<QnaDataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new QnaDataContext(dbContextOptions);
        }
    }
}