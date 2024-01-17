﻿using Blog.Server.Data;
using Blog.Server.Data.Models;
using Blog.Server.Exceptions;
using Blog.Server.Extensions;
using Blog.Server.Models.Requests;
using Blog.Server.Services.FileStorage;
using Microsoft.EntityFrameworkCore;

namespace Blog.Server.Services.PostService
{
    public class PostService : IPostService
    {
        protected readonly BlogDbContext dbContext;
        protected readonly IFileStorage fileStorage;
        protected readonly IHttpContextAccessor httpContextAccessor;

        private int? UserId => httpContextAccessor.HttpContext?.GetUserId();
        private bool IsAdmin => httpContextAccessor.HttpContext?.IsAdmin() ?? false;

        public PostService(BlogDbContext dbContext, IFileStorage fileStorage, IHttpContextAccessor httpContextAccessor)
        {
            this.dbContext = dbContext;
            this.fileStorage = fileStorage;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<PostModel> CreatePost(CreatePostRequestModel requestModel)
        {
            var userId = UserId ?? throw new UnauthorizedAccessException("User is not authenticated");

            var author = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new NotFoundException($"User with id {userId} not found");

            var post = new PostModel
            {
                Title = requestModel.Title,
                Content = requestModel.Content,
                AuthorId = userId,
                Author = author,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                HasComments = requestModel.HasComments,
                IsPublished = requestModel.IsPublished
            };

            post = (await dbContext.Posts.AddAsync(post)).Entity;
            await dbContext.SaveChangesAsync();

            if (requestModel.Tags != null)
            {
                foreach (var tag in requestModel.Tags)
                {
                    var tagModel = await dbContext.Tags.FirstOrDefaultAsync(t => t.Tag == tag);
                    if (tagModel is null)
                    {
                        tagModel = new TagModel
                        {
                            Tag = tag
                        };
                        tagModel = (await dbContext.Tags.AddAsync(tagModel)).Entity;
                    }
                    post.Tags.Add(tagModel);
                }
            }

            await dbContext.SaveChangesAsync();

            if (requestModel.Files != null)
            {
                foreach (var file in requestModel.Files)
                {
                    var fileModel = new FileModel
                    {
                        Name = file.FileName,
                        ContentType = file.ContentType,
                        Size = file.Length,
                        PostId = post.Id,
                    };
                    fileModel = (await dbContext.Files.AddAsync(fileModel)).Entity;
                    await dbContext.SaveChangesAsync();
                    await fileStorage.SaveFile(fileModel, file);
                }
            }

            await dbContext.SaveChangesAsync();

            return post;
        }

        public async Task<PostModel> DeletePost(int id)
        {
            var userId = UserId ?? throw new UnauthorizedAccessException("User is not authenticated");
            var post = dbContext.Posts
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Include(p => p.Files)
                .FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                throw new NotFoundException($"Post with id {id} not found");
            }
            if (post.AuthorId != userId && !IsAdmin)
            {
                throw new UnauthorizedAccessException("User is not authorized");
            }

            // delete files from storage
            foreach (var file in post.Files)
            {
                await fileStorage.DeleteFile(file);
            }

            // delete all comments
            var comments = dbContext.Comments.Where(c => c.PostId == post.Id);
            dbContext.Comments.RemoveRange(comments);

            // delete all tags where no other posts are using them
            var tags = dbContext.Tags
                .Include(t => t.Posts)
                .Where(t => t.Posts.Count == 1);
            dbContext.Tags.RemoveRange(tags);

            await dbContext.SaveChangesAsync();

            dbContext.Posts.Remove(post);
            dbContext.SaveChanges();

            return post;
        }

        public async Task<PostModel> GetPost(int id)
        {
            var post = await dbContext.Posts
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Include(p => p.Files)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post is null)
            {
                throw new NotFoundException($"Post with id {id} not found");
            }

            if (!post.IsPublished && !IsAdmin)
            {
                throw new NotFoundException($"Post with id {id} not found");
            }

            return post;
        }

        public async Task<IEnumerable<PostModel>> GetPosts(PageRequestModel pageRequest)
        {
            var query = dbContext.Posts
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Include(p => p.Files)
                .Where(p => p.IsPublished || IsAdmin)
                .OrderByDescending(p => p.CreatedAt)
                .AsQueryable();

            query = query
                .Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize);

            return await query.ToListAsync();
        }

        public async Task<PostModel> UpdatePost(int id, UpdatePostRequestModel requestModel)
        {
            var userId = UserId ?? throw new UnauthorizedAccessException("User is not authenticated");
            var post = await dbContext.Posts
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Include(p => p.Files)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                throw new NotFoundException();
            }

            if (post.AuthorId != userId && !IsAdmin)
            {
                throw new UnauthorizedAccessException("User is not authorized");
            }

            post.Title = requestModel.Title;
            post.Content = requestModel.Content;
            post.HasComments = requestModel.HasComments;
            post.IsPublished = requestModel.IsPublished;
            post.UpdatedAt = DateTime.UtcNow;


            if (requestModel.Tags != null)
            {
                post.Tags.Clear();
                foreach (var tag in requestModel.Tags)
                {
                    var tagModel = await dbContext.Tags.FirstOrDefaultAsync(t => t.Tag == tag);
                    if (tagModel is null)
                    {
                        tagModel = new TagModel
                        {
                            Tag = tag
                        };
                        tagModel = (await dbContext.Tags.AddAsync(tagModel)).Entity;
                    }
                    post.Tags.Add(tagModel);
                }
            }

            await dbContext.SaveChangesAsync();

            return post;
        }
    }
}