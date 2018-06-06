using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using React_ASP.Net_Core_Boilerplate.Helpers;
using React_ASP.Net_Core_Boilerplate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Service_Repository_Layer.Common.Response;
using Service_Repository_Layer.Enums;
using Service_Repository_Layer.Service.Contracts;
using Service_Repository_Layer.Common.DataTransferObjects;

namespace React_ASP.Net_Core_Boilerplate.Controllers
{    
    public class HomeController : Controller
    {
        private IUserService _userService;
        private readonly AppSettings _appSettings;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));
        private readonly RenderView _renderView;
        private static bool _logout = false;

        public HomeController(
                IUserService userService,
                IOptions<AppSettings> appSettings,
                RenderView renderView)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
            _renderView = renderView;
        }

        public IActionResult Index()
        {
            var result = HttpContext.Session.GetString("user");
            
            if (result != null)
            {
                var userObject = JsonConvert.DeserializeObject<UserViewModel>(result);
                var response = _userService.GetUserByToken(userObject.Token);
                
                if (response.Code == ResponseCode.Success && !_logout)
                {
                    _logout = false;
                    return RedirectToAction("Home");
                }
                else
                {
                    HttpContext.Session.Clear();
                    return View("Index");
                }
            }

            HttpContext.Session.Clear();
            return View("Index");
        }

        public IActionResult Home()
        {
            var result = HttpContext.Session.GetString("user");
            if (!string.IsNullOrEmpty(result))
            {
                var userObject = JsonConvert.DeserializeObject<UserViewModel>(result);
                var response = _userService.GetUserByToken(userObject.Token);

                if (response.Code == ResponseCode.Success && !_logout)
                {
                    _logout = false;
                    return View(userObject);
                }
                else
                    return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var serviceResponse = _userService.Authenticate(model.Username, model.Password);

                if (serviceResponse.Code == ResponseCode.Success)
                {
                    var user = serviceResponse.Item;

                    if (user == null)
                    {
                        ModelState.AddModelError("error_messages", "Username or password is incorrect");
                        return BadRequest(ModelState);
                    }

                    ViewData["ReturnUrl"] = "/";
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, user.Id.ToString()),                           
                        }),
                        Expires = DateTime.Now.AddMinutes(_appSettings.SessionTimeout),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    var userObject = new UserViewModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Token = tokenString                        
                    };
                    HttpContext.Session.SetString("user", JsonConvert.SerializeObject(userObject));

                    user.Token = tokenString;
                    _userService.UpdateUser(user);
                    _logout = false;
                    return PartialView("Home", userObject);
                }
                else if (serviceResponse.Code == ResponseCode.Success)
                {
                    ModelState.AddModelError(string.Empty, serviceResponse.Message);
                    return View("Index", model);
                }
                else if (serviceResponse.Code == ResponseCode.WrongCredentials)
                {
                    ModelState.AddModelError(string.Empty, serviceResponse.Message);
                    return View("Index", model);
                }
                else if (serviceResponse.Code == ResponseCode.UsernameNotInContext)
                {
                    ModelState.AddModelError(string.Empty, serviceResponse.Message);
                    return View("Index", model);
                }
                else if (serviceResponse.Code == ResponseCode.EmptyNullParameters)
                {
                    ModelState.AddModelError(string.Empty, serviceResponse.Message);
                    return View("Index", model);
                }
                else if (serviceResponse.Code == ResponseCode.EmailNotConfirmed)
                {
                    ModelState.AddModelError(string.Empty, serviceResponse.Message);
                    return View("Index", model);
                }
                else if (serviceResponse.Code == ResponseCode.UserCannotLogin)
                {
                    ModelState.AddModelError(string.Empty, serviceResponse.Message);
                    return View("Index", model);
                }
                else if (serviceResponse.Code == ResponseCode.UserDeleted)
                {
                    ModelState.AddModelError(string.Empty, serviceResponse.Message);
                    return View("Index", model);
                }                
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View("Index", model);
                }
            }

            return View("Index", model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Email))
            {
                var regex = new Regex(@"@test\.com$");

                var isValidEmail = regex.IsMatch(model.Email);

                if (!isValidEmail)
                    ModelState.AddModelError("Email", "Email must be in the tenant test.com");
            }

            if (ModelState.IsValid)
            {
                var user = new UserDto
                {
                    UserName = model.Username,
                    Password = model.Password,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email
                };
                var result = _userService.Register(user);
                if (result.Code == ResponseCode.Success)
                {
                    var code = _userService.GeneratePasswordResetToken(result.Item.Id);

                    ViewData["ReturnUrl"] = "/Login";
                    var domain = "http://" + Request.Host;
                    var actionName = "/Home/ConfirmEmail";
                    var href = domain + actionName + string.Format("?email={0}&code={1}", result.Item.Email, code.Item);
                    var pageModel = new ConfirmEmailViewModel
                    {
                        Href = href,
                        Name = result.Item.LastName + ", " + result.Item.FirstName,
                        Email = result.Item.Email
                    };
                    var view = await _renderView.RenderToStringAsync("ConfirmEmailTemplate", pageModel);

                    try
                    {
                        //_emailSender.SendEmail(model.Email, view, "Confirm Email");
                    }
                    catch (Exception ex)
                    {
                        // We do nothing. We don't let the user know that the email he provided doesn't exists.
                    }

                    return RedirectToAction("Index");
                }
                else if (result.Code == ResponseCode.AlreadyExistsInContext)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    return View(model);
                }
            }

            return View(model);
        } 
        
        [HttpPost]
        public IActionResult Logout([FromBody]string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var userLoggedIn = _userService.GetUserByToken(token);

                if (userLoggedIn.Item != null)
                {
                    userLoggedIn.Item.Token = null;
                    _userService.UpdateUser(userLoggedIn.Item);

                    HttpContext.Session.Clear();
                    _logout = true;                    
                }
            }

            HttpContext.Session.Clear();
            _logout = true;
            Response response = new Response();
            return Ok(response);
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
        
    }
}
