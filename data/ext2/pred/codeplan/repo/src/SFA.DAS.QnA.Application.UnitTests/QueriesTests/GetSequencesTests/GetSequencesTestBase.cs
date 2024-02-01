using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Queries.Sequences.GetSequences;
using SFA.DAS.QnA.Data;
using SFA.DAS.QnA.Data.Entities;

namespace SFA.DAS.QnA.Application.UnitTests.QueriesTests.GetSequencesTests
{
    [TestFixture]
    public class GetSequencesTestBase
    {
        protected GetSequencesHandler Handler;
        protected Guid ApplicationId;

        [SetUp]
        public async Task SetUp()
        {
            var dbContextOptions = new DbContextOptionsBuilder<QnaDataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new QnaDataContext(dbContextOptions);

            ApplicationId = Guid.NewGuid();

            context.Applications.Add(new Data.Entities.Application { Id = ApplicationId });

            context.ApplicationSequences.AddRange(new[]
            {
                new ApplicationSequence {ApplicationId = ApplicationId, IsActive = false, SequenceNo = 1},
                new ApplicationSequence {ApplicationId = ApplicationId, IsActive = true, SequenceNo = 2},
                new ApplicationSequence {ApplicationId = Guid.NewGuid(), IsActive = true, SequenceNo = 1}
            });

            await context.SaveChangesAsync();

            var mapper = new Mapper(new MapperConfiguration(config => { config.AddMaps(AppDomain.CurrentDomain.GetAssemblies()); }));

            Handler = new GetSequencesHandler(context, mapper);
        }
    }
}