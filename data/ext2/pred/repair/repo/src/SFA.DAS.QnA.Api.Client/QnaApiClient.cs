using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.QnA.Api.Client
{
    public class QnaApiClient
    {
        private readonly HttpClient _httpClient;

        public QnaApiClient(HttpClient httpClient, QnaApiConfig apiConfig)
        {
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiConfig.GetBearerToken());
        }

        public async Task<StartApplicationResponse> StartApplication(StartApplicationRequest request)
        {
            return await HttpCall<StartApplicationResponse>(async () => await _httpClient.PostAsJsonAsync(new Uri("applications/start", UriKind.Relative), request));
        }

        public async Task<object> GetApplicationData(Guid applicationId)
        {
            return await HttpCall<object>(async () => await _httpClient.GetAsync(new Uri($"applications/{applicationId}/applicationData", UriKind.Relative)));
        }

        public async Task<Sequence> GetCurrentSequence(Guid applicationId)
        {
            return await HttpCall<Sequence>(async () => await _httpClient.GetAsync(new Uri($"applications/{applicationId}/sequences/current", UriKind.Relative)));
        }

        public async Task<List<Section>> GetSequence(Guid applicationId, Guid sequenceId)
        {
            return await HttpCall<List<Section>>(async () => await _httpClient.GetAsync(new Uri($"applications/{applicationId}/sequences/{sequenceId}", UriKind.Relative)));
        }

        public async Task<List<Section>> GetSequenceBySequenceNo(Guid applicationId, int sequenceNo)
        {
            return await HttpCall<List<Section>>(async () => await _httpClient.GetAsync(new Uri($"applications/{applicationId}/sequences/{sequenceNo}", UriKind.Relative)));
        }

        public async Task<List<Section>> GetSections(Guid applicationId, Guid sequenceId)
        {
            return await HttpCall<List<Section>>(async () => await _httpClient.GetAsync(new Uri($"applications/{applicationId}/sequences/{sequenceId}/sections", UriKind.Relative)));
        }

        public async Task<Section> GetSection(Guid applicationId, Guid sectionId)
        {
            return await HttpCall<Section>(async () => await _httpClient.GetAsync(new Uri($"applications/{applicationId}/sections/{sectionId}", UriKind.Relative)));
        }

        public async Task<Section> GetSectionBySectionNo(Guid applicationId, int sequenceNo, int sectionNo)
        {
            return await HttpCall<Section>(async () => await _httpClient.GetAsync(new Uri($"applications/{applicationId}/sequences/{sequenceNo}/sections/{sectionNo}", UriKind.Relative)));
        }

        //        public async Task<Section> DownloadPageFiles(Guid applicationId, Guid sectionId, string pageId)
        //        {
        //            var response = await _httpClient.GetAsync(new Uri($"{applicationId}/sections/{sectionId}/pages/{pageId}/download", UriKind.Relative));
        //            if (response.IsSuccessStatusCode)
        //            {
        //                response.Content.Headers.
        //            }
        //        }

        //        public async Task<List<Workflow>> GetWorkflows()
        //        {
        //            using (var response = await _httpClient.GetAsync(new Uri("workflows", UriKind.Relative)))
        //            {
        //                return await response.Content.ReadAsAsync<List<Workflow>>();
        //            }
        //        }

        private async Task<T> HttpCall<T>(Func<Task<HttpResponseMessage>> httpClientAction)
        {
            var httpResponse = await httpClientAction();

            if (httpResponse.IsSuccessStatusCode)
            {
                return await httpResponse.Content.ReadAsAsync<T>();
            }

            throw new HttpRequestException($"Error sending {httpResponse.RequestMessage.Method} to {httpResponse.RequestMessage.RequestUri}. Returned: {await httpResponse.Content.ReadAsStringAsync()}");
        }
    }
}