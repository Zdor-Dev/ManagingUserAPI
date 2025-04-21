using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private static readonly List<User> users = new();
        private readonly IValidator<User> _validator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IValidator<User> validator, ILogger<UsersController> logger)
        {
            _validator = validator;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            if (!users.Any())
            {
                _logger.LogWarning("Querying a list of users, but the database is empty.");
                return NotFound("There are no users.");
            }

            _logger.LogInformation($"A list of all users is requested. Quantity: {users.Count}");
            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUser(Guid id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                _logger.LogWarning($"The user with ID {id} was not found.");
                return NotFound("User not found.");
            }

            return Ok(user);
        }
        [HttpPost]
        public ActionResult<User> AddUser(User user)
        {
            try
            {
                var validationResult = _validator.Validate(user);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning($"Validation Error: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
                    return BadRequest(validationResult.Errors);
                }

                if (users.Any(u => u.Id == user.Id || u.Email == user.Email))
                {
                    _logger.LogWarning($"Attempting to create a duplicate user: {user.Id} - {user.Email}");
                    return BadRequest("A user with this ID or Email already exists!");
                }

                users.Add(user);
                _logger.LogInformation($"User Created: {user.Id} - {user.Email}");
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when creating a user.");
                return StatusCode(500, "There was an internal server error.");
            }
        }

        [HttpPut("{id}")]
        public ActionResult UpdateUser(Guid id, User updatedUser)
        {
            try
            {
                var user = users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    _logger.LogWarning($"Attempted to update a non-existent user: {id}");
                    return NotFound("User not found.");
                }

                var validationResult = _validator.Validate(updatedUser);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning($"Validation Error: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
                    return BadRequest(validationResult.Errors);
                }

                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                _logger.LogInformation($"User Updated: {user.Id} - {user.Email}");
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when updating a user.");
                return StatusCode(500, "There was an internal server error.");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser(Guid id)
        {
            try
            {
                var duplicatedUsers = users.Where(u => u.Id == id).ToList();
                if (!duplicatedUsers.Any())
                {
                    _logger.LogWarning($"Attempting to delete a non-existent user: {id}");
                    return NotFound("User not found.");
                }

                foreach (var user in duplicatedUsers)
                {
                    users.Remove(user);
                }

                _logger.LogInformation($"User Deleted: {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when deleting a user.");
                return StatusCode(500, "There was an internal server error.");
            }
        }
    }






}
