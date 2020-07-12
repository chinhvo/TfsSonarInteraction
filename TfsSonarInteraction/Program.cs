using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net.Http;
using SonarQube.Client;
using SonarQube.Client.Models;
using System.Security;
using System.Threading;
using SonarQube.Client.Helpers;
using SonarQube.Client.Api.V7_20;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace PRInteraction
{
    internal class Program
    {
        public static IConfiguration Configuration { get; private set; }
        public static AppSettings AppSettings { get; private set; }
        public static HttpClient Client { get; private set; }
        public static TestLogger Logger { get; private set; }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            AppSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();

            Console.WriteLine("Start get Sonar Scan issues and add comment to pull rquest");

            if (args.Length == 0)
            {
                System.Console.WriteLine("Please enter pull request id.");
                AppSettings.PullRequestId = Convert.ToInt32(Console.ReadLine());
                System.Console.WriteLine("Please enter pull project key.");
                AppSettings.ProjectKey = Console.ReadLine();
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    AppSettings.PullRequestId = Convert.ToInt32(args[0]);
                    AppSettings.ProjectKey = Convert.ToString(args[1]);
                }
            }

            Client = new HttpClient() { BaseAddress = new Uri(AppSettings.SonarServer) };

            Logger = new TestLogger();

            Task.Run(async () => { await Task.WhenAll(CallPostPullRequestCommentFromSonar(AppSettings.PullRequestId, AppSettings.ProjectKey)); }).GetAwaiter().GetResult();

            System.Console.WriteLine("End process.");
        }

        private static async Task CallPostPullRequestCommentFromSonar(int pullRequestId, string sonarKey)
        {
            var lastFilesChange = await GetGitChanges(pullRequestId);
            var sonarIssues = await GetSonarQubeIssues(sonarKey);
            await PostCommentToPR(pullRequestId, lastFilesChange, sonarIssues.ToList());
        }

        private static async Task PostCommentToPR(int pullRequestId, List<GitItem> gitsChange, List<SonarQubeIssue> issues)
        {
            var connection = GetConnection();
            var client = connection.GetClient<GitHttpClient>();
            var pullRequestCommentThread = await client.GetThreadsAsync(
               AppSettings.RepoId,
               pullRequestId
               );
            foreach (var item in issues)
            {
                if (item.Line.GetValueOrDefault() == 0)
                {
                    continue;
                }
                if (!gitsChange.Any(x => x.Path.Contains(item.FilePath)))
                {
                    continue;
                }
                if (ExistedComment(pullRequestCommentThread, item))
                {
                    continue;
                }
                var comment = new Comment()
                {
                    Content = item.Message,
                    CommentType = CommentType.CodeChange
                };
                var thread = new GitPullRequestCommentThread()
                {
                    Comments = new List<Comment>() { comment },
                    PublishedDate = DateTime.Now,
                    PullRequestThreadContext = new GitPullRequestCommentThreadContext()
                    {
                        IterationContext = new CommentIterationContext()
                        {
                            FirstComparingIteration = AppSettings.FirstComparingIteration,
                            SecondComparingIteration = AppSettings.SecondComparingIteration
                        }
                    },
                    Status = CommentThreadStatus.Active,
                    ThreadContext = new CommentThreadContext()
                    {
                        FilePath = item.FilePath,
                        RightFileStart = new CommentPosition()
                        {
                            Line = item.Line.GetValueOrDefault(),
                            Offset = item.TextRange.StartOffset.GetValueOrDefault()
                        },
                        RightFileEnd = new CommentPosition()
                        {
                            Line = item.Line.GetValueOrDefault(),
                            Offset = item.TextRange.EndOffset.GetValueOrDefault()
                        },
                    },
                };
                await client.CreateThreadAsync(thread, AppSettings.RepoId, pullRequestId, item.Hash);
            }
        }

        private static bool ExistedComment(List<GitPullRequestCommentThread> threads, SonarQubeIssue issue)
        {
            var matchFilePath = threads.Any(p => p.ThreadContext != null && p.ThreadContext.FilePath.Contains(issue.FilePath));
            var matchCommentPosition = threads.Any(
                p => p.ThreadContext != null &&
                p.ThreadContext.RightFileStart != null &&
                p.ThreadContext.RightFileStart.Line == issue.Line &&
                p.ThreadContext.RightFileStart.Offset == issue.TextRange.StartOffset);
            return matchFilePath && matchCommentPosition;
        }


        private static VssConnection GetConnection()
        {
            var collectionUri = new Uri(string.Format("{0}/{1}", AppSettings.ServerUrl, AppSettings.Collection));
            string pat = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", AppSettings.PAT)));

            var credentials = new VssCredentials(new Microsoft.VisualStudio.Services.Common.WindowsCredential(CredentialCache.DefaultNetworkCredentials));
            var connection = new VssConnection(collectionUri, credentials);


            return connection;
        }
      

        private static async Task<List<GitItem>> GetGitChanges(int pullRequestId)
        {
            var connection = GetConnection();
            var client = connection.GetClient<GitHttpClient>();
            var pr = await client.GetPullRequestCommitsAsync(
                AppSettings.RepoId,
                pullRequestId
                );


            var lastCommited = pr.FirstOrDefault();
            var lastItemsChange = await client.GetCommitAsync(
                lastCommited.CommitId,
                AppSettings.RepoId,
                AppSettings.MaxResult
                );
            var lastFilesChange = lastItemsChange.Changes.Where(p => p.Item.IsFolder != true).Select(p => p.Item).ToList();
            return lastFilesChange;
        }


        private static async Task<SonarQubeIssue[]> GetSonarQubeIssues(string sonarKey)
        {
            var service = new SonarQubeService(new HttpClientHandler(), "user-agent-string", Logger);
            await service.ConnectAsync(
                            new ConnectionInformation(new Uri(AppSettings.SonarServer), AppSettings.SonarUserName, AppSettings.SonarUserPassword.ToSecureString()),
                            CancellationToken.None);

            var getIssueWrapper = new IssuesRequestWrapper()
            {
                ProjectKey = sonarKey,
                Logger = Logger,
                HttpMethod = HttpMethod.Get
            };
            var issues = await getIssueWrapper.InvokeAsync(Client, CancellationToken.None);
            return issues;
        }

    }
}
