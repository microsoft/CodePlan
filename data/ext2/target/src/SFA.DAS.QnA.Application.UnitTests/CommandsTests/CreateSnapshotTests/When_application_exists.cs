using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.QnA.Application.Commands.CreateSnapshot;

namespace SFA.DAS.QnA.Application.UnitTests.CommandsTests.CreateSnapshotTests
{
    public class When_application_exists : CreateSnapshotTestBase
    {
        [Test]
#if (!DEBUG)
        [Ignore("Must be tested on local DEV machine as it uses local Azure storage")]
#endif
        public async Task Then_snapshot_is_created()
        {
            var snapshot = await Handler.Handle(new CreateSnapshotRequest(ApplicationId), new System.Threading.CancellationToken());

            Assert.IsTrue(snapshot.Success);
            Assert.AreNotEqual(ApplicationId, snapshot.Value.ApplicationId);
        }

        [Test]
#if (!DEBUG)
        [Ignore("Must be tested on local DEV machine as it uses local Azure storage")]
#endif
        public async Task Then_snapshot_has_copied_over_files_successfully()
        {
            var snapshot = await Handler.Handle(new CreateSnapshotRequest(ApplicationId), new System.Threading.CancellationToken());
            Assert.IsTrue(snapshot.Success);

            var section = DataContext.ApplicationSections.SingleOrDefault(sec => sec.ApplicationId == snapshot.Value.ApplicationId);
            var page = section?.QnAData.Pages.SingleOrDefault(p => p.PageId == PageId);
            var answer = page?.PageOfAnswers.SelectMany(pao => pao.Answers).SingleOrDefault(ans => ans.QuestionId == QuestionId);

            Assert.IsNotNull(answer);
            Assert.AreEqual(Filename, answer.Value);
            Assert.IsTrue(FileExists(section.ApplicationId, section.SequenceId, section.Id, page.PageId, answer.QuestionId, answer.Value, Container));
        }
    }
}
