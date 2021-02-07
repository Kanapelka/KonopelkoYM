using System.Linq;
using System.Threading.Tasks;
using Athena.Core.Models;
using Athena.Core.Result;
using Athena.Infrastructure;
using Athena.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Athena.Core.Services
{
    public class CommentService : BaseService
    {
        public CommentService(IInfrastructureOptionsProvider optionsProvider) : base(optionsProvider)
        {
        }


        public async Task<Result<CommentModel>> AddCommentAsync(Comment comment)
        {
            await using var context = Context;

            if (!context.Users.Any(u => u.UserId == comment.AuthorId)) {
                return new Result<CommentModel>{ ResultType = ResultType.NotFound,Message = "User not found"};
            }

            if (!context.Tickets.Any(t => t.TicketId == comment.TicketId)) {
                return new Result<CommentModel>{ ResultType = ResultType.NotFound,Message = "Ticket not found"};
            }

            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();

            CommentModel addedComment = context.Comments
                .Include(c => c.Author)
                .Where(c => c.TicketId == comment.TicketId)
                .Select(c => new CommentModel
                {
                    CommentId = c.CommentId,
                    TicketId = c.TicketId,
                    AuthorId = c.AuthorId,
                    AuthorFirstName = c.Author.FirstName,
                    AuthorLastName = c.Author.LastName,
                    CreatedDate = c.CreatedDate,
                    Message = c.Message,
                }).FirstOrDefault();

            return new Result<CommentModel>{ ResultType = ResultType.Created, Payload =  addedComment };
        }
    }
}