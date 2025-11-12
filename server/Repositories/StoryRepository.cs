using DragonGame.Data;
using DragonGame.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonGame.Repositories
{
    /// <summary>
    /// Repository for managing Story entities and their related Acts and Choices.
    /// Implements CRUD operations (Create, Read, Update, Delete).
    /// Uses nested eager loading to fetch Story - Acts - Choices inone database query, to avoid multiple rounds to the database
    /// </summary>
    public class StoryRepository : IStoryRepository
    {
        private readonly AppDbContext _context;
        /// <summary>
        /// Initializes a new instance of StoryRepository with db context.
        /// </summary>
        /// <param name="context">Entity framework database context</param>
        public StoryRepository(AppDbContext context) => _context = context;

        /// <summary>
        /// Gets all stories with their acts and choices.
        /// </summary>
        /// <returns>List of stories with acts and choices</returns>
        public async Task<List<Story>> GetAllAsync() => await _context.Stories.Include(s => s.Acts).ThenInclude(a => a.Choices).ToListAsync();
        public async Task<Story> GetByIdAsync(int id)
        {
            var story = await _context.Stories
                .Include(s => s.Acts) // load acts for each story
                .ThenInclude(a => a.Choices) // load choices for each act
                .FirstOrDefaultAsync(s => s.StoryId == id);



            if (story == null)
                throw new KeyNotFoundException($"Story with ID {id} not found.");

            return story;
        }

        /// <summary>
        /// Creates a new story.
        /// </summary>
        /// <param name="story">The story to create</param>
        /// <returns>The created story with generated ID</returns>
        public async Task<Story> CreateAsync(Story story)
        {
            _context.Stories.Add(story);
            await _context.SaveChangesAsync();
            return story;
        }
        /// <summary>
        /// Updates an existing story.
        /// </summary>
        /// <param name="story">Story to update</param>
        public async Task UpdateAsync(Story story)
        {
            _context.Stories.Update(story);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Deletes a story by id.
        /// </summary>
        /// <param name="id">Story id to delete</param>
        public async Task DeleteAsync(int id)
        {
            var story = await _context.Stories.FindAsync(id);
            if (story != null)
            {
                _context.Stories.Remove(story);
                await _context.SaveChangesAsync();
            }
        }
    }
}