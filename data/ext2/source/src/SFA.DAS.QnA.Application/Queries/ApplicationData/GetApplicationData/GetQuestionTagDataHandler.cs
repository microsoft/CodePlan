using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace SFA.DAS.QnA.Application.Queries.ApplicationData.GetApplicationData
{
    public class GetQuestionTagDataHandler : IRequestHandler<GetQuestionTagDataRequest, HandlerResponse<string>>
    {
        private readonly QnaDataContext _dataContext;

        public GetQuestionTagDataHandler(QnaDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<HandlerResponse<string>> Handle(GetQuestionTagDataRequest request, CancellationToken cancellationToken)
        {
            using (var command = _dataContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = @"DECLARE @AppDataJson NVARCHAR(MAX) = (SELECT ApplicationData FROM [dbo].[Applications] WHERE Id = @ApplicationId)
                                                                SELECT value AS QuestionTagValue
                                                                FROM OPENJSON(@AppDataJson)
                                                                WHERE[key] = @QuestionTag";
                command.Parameters.Add(new SqlParameter("@ApplicationId", request.ApplicationId));
                command.Parameters.Add(new SqlParameter("@QuestionTag", request.QuestionTag));
                _dataContext.Database.OpenConnection();

                try
                {
                    var questionTagValue = command.ExecuteScalar();
                    return new HandlerResponse<string>(Convert.ToString(questionTagValue));
                }
                catch (Exception ex)
                {
                    // TODO: Log the ex.Message;
                    return new HandlerResponse<string>(success: false, message: "QuestionTag not exist.");
                }
            }
        }
    }
}
