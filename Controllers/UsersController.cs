using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using forfarmingcase.Entities;
using forfarmingcase.Repositories.Interfaces;
using forfarmingcase.Validators.FluentValidation;
using forfarmingcase.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace forfarmingcase.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly IUserRepository _repository;
    private readonly ILogger<UsersController> _logger;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _config;
    public UsersController(IUserRepository repository, ILogger<UsersController> logger, IMapper mapper, ITokenService tokenService, IConfiguration config)
    {
      _repository = repository;
      _logger = logger;
      _mapper = mapper;
      _tokenService = tokenService;
      _config = config;
    }
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [ProducesResponseType(typeof(List<User>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<List<UserResponseVM>>> Get()
    {
      var users = await _repository.GetAll();

      if (users.Count < 1)
      {
        _logger.LogError("Users not found");
        return NotFound(new { Error = "Users not found" });
      }

      List<UserResponseVM> userViewModel = _mapper.Map<List<UserResponseVM>>(users);
      return Ok(userViewModel);
    }

    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("{id}", Name = "GetById")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<UserResponseVM>> GetById(int id)
    {
      var user = await _repository.GetById(id);

      if(user == null)
      {
        _logger.LogError($"User not found with id: {id}");
        return NotFound(new { Error = "User not found" });
      }
     UserResponseVM userViewModel = _mapper.Map<UserResponseVM>(user);
      return Ok(userViewModel);
    }

    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<User>> Post([FromBody] UserCreateVM userCreateVM)
    {
      var validator = new UserCreateValidator();
      var result = validator.Validate(userCreateVM);
      if (!result.IsValid) return BadRequest(new {Error = result});
      User user = _mapper.Map<User>(userCreateVM);
      await _repository.Create(user);
      return CreatedAtRoute("GetById", new { id = user.Id }, user);
    }

    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<User>> Put([FromBody] UserUpdateVM userUpdateVM)
    {
      var validator = new UserUpdateValidator();
      var result = validator.Validate(userUpdateVM);
      if (!result.IsValid) return BadRequest(new { Error = result });
      User user = _mapper.Map<User>(userUpdateVM);
      var existingsUser = await _repository.GetById(user.Id);
      if(existingsUser == null)
      {
        _logger.LogError($"User not found with id: {user.Id}");
        return NotFound(new { Error = "User not found" });
      }
      await _repository.Update(user);
      return AcceptedAtRoute("GetById", new { id = user.Id }, user);
    }

    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
      var existingsUser = await _repository.GetById(id);
      if (existingsUser == null)
      {
        _logger.LogError($"User not found with id: {id}");
        return NotFound(new { Error = "User not found" });
      }
      await _repository.Delete(id);
      return NoContent();
    }

    [AllowAnonymous]
    [Route("login")]
    [HttpPost]
    public async  Task<ActionResult> Login([FromBody] LoginVM model)
    {
        var status = await _repository.Login(model);
         if (!status) return BadRequest();
        var generatedToken = _tokenService.BuildToken(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), _config["Jwt:Audience"].ToString(), model);
        if (generatedToken != null)
        {
          return Ok(new { Token = generatedToken });
        }
        else
        {
          return BadRequest();
        }
      }

    }

  }
