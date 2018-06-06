using Service_Repository_Layer.Entity;
using Service_Repository_Layer.Common.Response;
using Service_Repository_Layer.Repo.Contracts;
using Service_Repository_Layer.Service.Contracts;
using Service_Repository_Layer.Enums;
using Service_Repository_Layer.Common.DataTransferObjects;

using System;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;
using Newtonsoft.Json;
using System.Security;

namespace Service_Repository_Layer.Service
{
    public class UserService : IUserService
    {
        private readonly ILogRepository _logRepository;
        private readonly IUserRepository _userRepository;

        public UserService(
            ILogRepository logRepository,
            IUserRepository userRepository)
        {
            _logRepository = logRepository;
            _userRepository = userRepository;
        }

        public ItemResponse<UserDto> Authenticate(string username, string password)
        {
            ItemResponse<UserDto> serviceResponse = new ItemResponse<UserDto>(new ResponseModel(), null);

            try
            {
                var entity = _userRepository.Authenticate(username, password);

                var entityDto = Mapper.Map<User, UserDto>(entity);
                serviceResponse.Code = ResponseCode.Success;
                serviceResponse.Item = entityDto;
            }
            catch (ArgumentNullException)
            {
                serviceResponse.Code = ResponseCode.UsernameNotInContext;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "User with the username: " + username + " doesn't exists in the context.";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logRepository.Log("User Service - Authenticate",
                    ArchLevel.Repository, methodName,
                    string.Format("Given Username and Password doesn't match any record in the context. Username: {0}, Password: {1}", username, password));
            }
            catch (ArgumentException)
            {
                serviceResponse.Code = ResponseCode.EmptyNullParameters;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Username or Password can't be left null or empty.";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logRepository.Log("User Service - Authenticate",
                    ArchLevel.Repository, methodName,
                    string.Format("Given Username and Password contain Empty or Null parameters. Username: {0}, Password: {1}", username, password));
            }
            catch (UnauthorizedAccessException)
            {
                serviceResponse.Code = ResponseCode.WrongCredentials;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Provided password for user: " + username + " is incorrect.";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logRepository.Log("User Service - Authenticate",
                    ArchLevel.Repository, methodName,
                    string.Format("Given Password for Username: {0} is not correct.", username));
            }
            catch (MissingFieldException)
            {
                serviceResponse.Code = ResponseCode.EmailNotConfirmed;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Email is not confirmed yet.";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logRepository.Log("User Service - Authenticate",
                    ArchLevel.Repository, methodName,
                    string.Format("Username: {0} has not yet confirmed email but tried to authenticate.", username));
            }
            catch (SecurityException)
            {
                serviceResponse.Code = ResponseCode.UserDeleted;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Passive users cannot login. Please contact your system administrator !";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logRepository.Log("User Service - Reset Password",
                    ArchLevel.Repository, methodName,
                    string.Format("Username: {0} cannot reset password. (Passive user)", username));
            }
            catch (VerificationException)
            {
                serviceResponse.Code = ResponseCode.UserCannotLogin;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Standard user cannot login until role is changed by Admin/SuperAdmin. Please contact your system administrator !";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logRepository.Log("User Service - Authenticate",
                    ArchLevel.Repository, methodName,
                    string.Format("Username: {0} cannot login until user role is changed", username));
            }

            catch (Exception ex)
            {
                serviceResponse.Code = ResponseCode.Error;
                serviceResponse.LogLevel = LogLevel.Error;
                serviceResponse.Message = "Error occured while trying to remove the user from the context. " +
                    "See exception: " + ex.Message;

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logRepository.Log("User Service - Authenticate",
                    ArchLevel.Repository, methodName,
                    string.Format("Excepiton occured while fetching users from the context. See following exception message: {0}.", ex.Message));
            }

            return serviceResponse;
        }

        public ItemResponse<UserDto> Create(UserDto user)
        {
            ItemResponse<UserDto> serviceResponse = new ItemResponse<UserDto>(new ResponseModel(), null);

            try
            {
                var userEntity = Mapper.Map<UserDto, User>(user);

                try
                {
                    userEntity.EmailConfirmed = true;
                    var entity = _userRepository.Create(userEntity, user.Token);

                    var entityDto = Mapper.Map<User, UserDto>(entity);

                    serviceResponse.Code = ResponseCode.Success;
                    serviceResponse.Item = entityDto;
                }
                catch (ArgumentNullException)
                {
                    serviceResponse.Code = ResponseCode.NotFoundInContext;
                    serviceResponse.LogLevel = LogLevel.Information;
                    serviceResponse.Message = "Username '" + user.UserName + "' couldn't be found in the context.";

                    var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                    _logRepository.Log("User Service - Create",
                        ArchLevel.Repository, serializedObject, methodName,
                        string.Format("Username: {0} creation failed or couldn't be found in the context.", user.UserName));
                }
                catch (ApplicationException)
                {
                    serviceResponse.Code = ResponseCode.AlreadyExistsInContext;
                    serviceResponse.LogLevel = LogLevel.Information;
                    serviceResponse.Message = string.Format("Username or Email is already taken. Username: {0}, Email: {1}", user.UserName, user.Email);

                    var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                    _logRepository.Log("User Service - Create",
                        ArchLevel.Repository, serializedObject, methodName,
                        string.Format("Username: {0} exists already in the context", user.UserName));
                }
                catch (ArgumentException)
                {
                    serviceResponse.Code = ResponseCode.MissingInformation;
                    serviceResponse.LogLevel = LogLevel.Information;
                    serviceResponse.Message = "Password can't be null or empty.";

                    var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                    _logRepository.Log("User Service - Create",
                        ArchLevel.Repository, serializedObject, methodName,
                        string.Format("The user with Username: {0} tried to get registered with the following Password: {1}", user.UserName, user.Password));
                }
                catch (Exception ex)
                {
                    serviceResponse.Code = ResponseCode.Error;
                    serviceResponse.LogLevel = LogLevel.Error;
                    serviceResponse.Message = "Error occured while trying to remove the user from the context. " +
                        "See exception: " + ex.Message;

                    var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                    _logRepository.Log("User Service - Create",
                        ArchLevel.Repository, serializedObject, methodName,
                        string.Format("Excepiton occured while fetching users from the context. See following exception message: {0}.", ex.Message));
                }
            }
            catch (ArgumentNullException)
            {
                serviceResponse.Code = 0;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Mapping error occured.";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Create",
                    ArchLevel.Service, serializedObject, methodName,
                    "While adding a new User to the context, mapping the DTO to Entity Model created an ArgumentNullException. One or more child objects are null and shouldn't be.");
            }
            catch (Exception ex)
            {
                serviceResponse.Code = 0;
                serviceResponse.LogLevel = LogLevel.Error;
                serviceResponse.Message = "Error occured while trying to update the user from the context. " +
                    "See exception: " + ex.Message;

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Create",
                        ArchLevel.Service, serializedObject, methodName,
                        string.Format("While adding a new User to the context, an exception with the following message occured: {0}", ex.Message));
            }

            return serviceResponse;
        }

        public Response Delete(int id)
        {
            Response serviceResponse = new Response();

            try
            {
                _userRepository.Delete(id);
            }
            catch (ArgumentNullException)
            {
                serviceResponse.Code = ResponseCode.NotFoundInContext;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Couldn't find the user on the repository.";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(id, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Delete",
                    ArchLevel.Repository, serializedObject, methodName,
                    "The user that has been tried to be deleted doesn't exists in the context.");
            }
            catch (Exception ex)
            {
                serviceResponse.Code = ResponseCode.Error;
                serviceResponse.LogLevel = LogLevel.Error;
                serviceResponse.Message = "Error occured while trying to remove the user from the context. " +
                    "See exception: " + ex.Message;

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(id, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Delete",
                    ArchLevel.Repository, serializedObject, methodName,
                    string.Format("The user with ID: {0} couldn't be deleted from the context. See following exception message: {1}", id, ex.Message));
            }

            return serviceResponse;
        }

        public ItemResponse<string> GeneratePasswordResetToken(int id)
        {
            ItemResponse<string> serviceResponse = new ItemResponse<string>(new ResponseModel(), null);

            try
            {
                var token = _userRepository.GeneratePasswordResetToken(id);

                serviceResponse.Code = ResponseCode.Success;
                serviceResponse.Item = token;
            }
            catch (ArgumentNullException)
            {
                serviceResponse.Code = ResponseCode.NotFoundInContext;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Couldn't find the user on the repository.";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(id, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Generate Password Reset Token",
                    ArchLevel.Repository, serializedObject, methodName,
                    string.Format("User with ID: {0} doesn't exists in the context.", id));
            }
            catch (Exception ex)
            {
                serviceResponse.Code = ResponseCode.Error;
                serviceResponse.LogLevel = LogLevel.Error;
                serviceResponse.Message = "Error occured while trying to get the user from the context. " +
                    "See exception: " + ex.Message;

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(id, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Generate Password Reset Token",
                    ArchLevel.Repository, serializedObject, methodName,
                    string.Format("Exception occured while trying to get the User with ID: {0} for a password reset request. See following exception message: {1}.", id, ex.Message));
            }

            return serviceResponse;
        }

        public ListResponse<UserDto> GetAllUsers()
        {
            ListResponse<UserDto> serviceResponse = new ListResponse<UserDto>(new ResponseModel(), null);

            try
            {
                var entities = _userRepository.GetAll();                
                
                var entitiesDto = Mapper.Map<List<User>, List<UserDto>>(entities.ToList());
                serviceResponse.Code = ResponseCode.Success;
                serviceResponse.Items = entitiesDto;
            }
            catch (ArgumentNullException)
            {
                serviceResponse.Code = ResponseCode.NotFoundInContext;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Couldn't find any user on the repository.";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logRepository.Log("User Service - Get All",
                    ArchLevel.Repository, methodName,
                    "Context throw ArgumentNullException which in this case means that EF couldn't fetch data from the database.");
            }
            catch (Exception ex)
            {
                serviceResponse.Code = ResponseCode.Error;
                serviceResponse.LogLevel = LogLevel.Error;
                serviceResponse.Message = "Error occured while trying to get users from the context. " +
                    "See exception: " + ex.Message;

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                _logRepository.Log("User Service - Get All",
                    ArchLevel.Repository, methodName,
                    string.Format("Excepiton occured while fetching users from the context. See following exception message: {0}.", ex.Message));
            }

            return serviceResponse;
        }

        public ItemResponse<UserDto> GetByEmail(string email)
        {
            ItemResponse<UserDto> serviceResponse = new ItemResponse<UserDto>(new ResponseModel(), null);

            try
            {
                var entity = _userRepository.GetByEmail(email);

                var entityDto = Mapper.Map<User, UserDto>(entity);
                serviceResponse.Code = ResponseCode.Success;
                serviceResponse.Item = entityDto;
            }
            catch (ArgumentNullException)
            {
                serviceResponse.Code = ResponseCode.NotFoundInContext;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Couldn't find the user on the repository.";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(email, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Get By Email",
                    ArchLevel.Repository, serializedObject, methodName,
                    string.Format("User with Email: {0} doesn't exists in the context.", email));
            }
            catch (Exception ex)
            {
                serviceResponse.Code = ResponseCode.Error;
                serviceResponse.LogLevel = LogLevel.Error;
                serviceResponse.Message = "Error occured while trying to get the user from the context. " +
                    "See exception: " + ex.Message;

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(email, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Get By Email",
                    ArchLevel.Repository, serializedObject, methodName,
                    string.Format("Exception occured while trying to get the User with Email: {0}. See following exception message: {1}.", email, ex.Message));
            }

            return serviceResponse;
        }

        public ItemResponse<UserDto> GetUserById(int id)
        {
            ItemResponse<UserDto> serviceResponse = new ItemResponse<UserDto>(new ResponseModel(), null);

            try
            {
                var entity = _userRepository.GetById(id);

                var entityDto = Mapper.Map<User, UserDto>(entity);
                serviceResponse.Code = ResponseCode.Success;
                serviceResponse.Item = entityDto;
            }
            catch (ArgumentNullException)
            {
                serviceResponse.Code = ResponseCode.NotFoundInContext;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Couldn't find the user on the repository.";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(id, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Get By Id",
                    ArchLevel.Repository, serializedObject, methodName,
                    string.Format("User with ID: {0} doesn't exists in the context.", id));
            }
            catch (Exception ex)
            {
                serviceResponse.Code = ResponseCode.Error;
                serviceResponse.LogLevel = LogLevel.Error;
                serviceResponse.Message = "Error occured while trying to get the user from the context. " +
                    "See exception: " + ex.Message;

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(id, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Get By Id",
                    ArchLevel.Repository, serializedObject, methodName,
                    string.Format("Exception occured while trying to get the User with ID: {0}. See following exception message: {1}.", id, ex.Message));
            }

            return serviceResponse;
        }

        public ItemResponse<UserDto> GetUserByToken(string token)
        {
            ItemResponse<UserDto> serviceResponse = new ItemResponse<UserDto>(new ResponseModel(), null);

            try
            {
                var entity = _userRepository.GetUserByToken(token);

                var entityDto = Mapper.Map<User, UserDto>(entity);
                serviceResponse.Code = ResponseCode.Success;
                serviceResponse.Item = entityDto;
            }
            catch (ArgumentNullException)
            {
                serviceResponse.Code = ResponseCode.NotFoundInContext;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Couldn't find the user on the repository.";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(token, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - User By Token",
                    ArchLevel.Repository, serializedObject, methodName,
                    string.Format("User with Token: {0} doesn't exists in the context.", token));
            }
            catch (Exception ex)
            {
                serviceResponse.Code = ResponseCode.Error;
                serviceResponse.LogLevel = LogLevel.Error;
                serviceResponse.Message = "Error occured while trying to get the user from the context. " +
                    "See exception: " + ex.Message;

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(token, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - User By Token",
                    ArchLevel.Repository, serializedObject, methodName,
                    string.Format("Exception occured while trying to get the User with Token: {0}. See following exception message: {1}.", token, ex.Message));
            }

            return serviceResponse;
        }

        public ItemResponse<UserDto> IsEmailConfirmed(int id)
        {
            ItemResponse<UserDto> serviceResponse = new ItemResponse<UserDto>(new ResponseModel(), null);

            try
            {
                var entity = _userRepository.IsEmailConfirmed(id);

                var entityDto = Mapper.Map<User, UserDto>(entity);
                serviceResponse.Code = ResponseCode.Success;
                serviceResponse.Item = entityDto;
            }
            catch (ArgumentNullException)
            {
                serviceResponse.Code = ResponseCode.NotFoundInContext;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Couldn't find the user on the repository.";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(id, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Email Confirmed Check",
                    ArchLevel.Repository, serializedObject, methodName,
                    string.Format("User with ID: {0} doesn't exists in the context.", id));
            }
            catch (Exception ex)
            {
                serviceResponse.Code = ResponseCode.Error;
                serviceResponse.LogLevel = LogLevel.Error;
                serviceResponse.Message = "Error occured while trying to get the user from the context. " +
                    "See exception: " + ex.Message;

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(id, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Email Confirmed Check",
                    ArchLevel.Repository, serializedObject, methodName,
                    string.Format("Exception occured while trying to get the User with ID: {0}. See following exception message: {1}.", id, ex.Message));
            }

            return serviceResponse;
        }

        public ItemResponse<UserDto> Register(UserDto user)
        {
            ItemResponse<UserDto> serviceResponse = new ItemResponse<UserDto>(new ResponseModel(), null);

            try
            {
                var userEntity = Mapper.Map<UserDto, User>(user);

                try
                {
                    userEntity.EmailConfirmed = false;
                    var entity = _userRepository.Create(userEntity, user.Password);

                    var entityDto = Mapper.Map<User, UserDto>(entity);

                    serviceResponse.Code = ResponseCode.Success;
                    serviceResponse.Item = entityDto;
                }
                catch (ArgumentNullException)
                {
                    serviceResponse.Code = ResponseCode.NotFoundInContext;
                    serviceResponse.LogLevel = LogLevel.Information;
                    serviceResponse.Message = "Username '" + user.UserName + "' couldn't be found in the context.";

                    var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                    _logRepository.Log("User Service - Register",
                        ArchLevel.Repository, serializedObject, methodName,
                        string.Format("Username: {0} creation failed or couldn't be found in the context.", user.UserName));
                }
                catch (ApplicationException)
                {
                    serviceResponse.Code = ResponseCode.AlreadyExistsInContext;
                    serviceResponse.LogLevel = LogLevel.Information;
                    serviceResponse.Message = string.Format("Username or Email is already taken. Username: {0}, Email: {1}", user.UserName, user.Email);

                    var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                    _logRepository.Log("User Service - Register",
                        ArchLevel.Repository, serializedObject, methodName,
                        string.Format("Username: {0} exists already in the context", user.UserName));
                }
                catch (ArgumentException)
                {
                    serviceResponse.Code = ResponseCode.MissingInformation;
                    serviceResponse.LogLevel = LogLevel.Information;
                    serviceResponse.Message = "Password can't be null or empty.";

                    var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                    _logRepository.Log("User Service - Register",
                        ArchLevel.Repository, serializedObject, methodName,
                        string.Format("The user with Username: {0} tried to get registered with the following Password: {1}", user.UserName, user.Password));
                }
                catch (Exception ex)
                {
                    serviceResponse.Code = ResponseCode.Error;
                    serviceResponse.LogLevel = LogLevel.Error;
                    serviceResponse.Message = "Error occured while trying to remove the user from the context. " +
                        "See exception: " + ex.Message;

                    var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                    _logRepository.Log("User Service - Register",
                        ArchLevel.Repository, serializedObject, methodName,
                        string.Format("Excepiton occured while fetching users from the context. See following exception message: {0}.", ex.Message));
                }
            }
            catch (ArgumentNullException)
            {
                serviceResponse.Code = 0;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Mapping error occured.";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Register",
                    ArchLevel.Service, serializedObject, methodName,
                    "While adding a new User to the context, mapping the DTO to Entity Model created an ArgumentNullException. One or more child objects are null and shouldn't be.");
            }
            catch (Exception ex)
            {
                serviceResponse.Code = 0;
                serviceResponse.LogLevel = LogLevel.Error;
                serviceResponse.Message = "Error occured while trying to update the user from the context. " +
                    "See exception: " + ex.Message;

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Create",
                        ArchLevel.Service, serializedObject, methodName,
                        string.Format("While adding a new User to the context, an exception with the following message occured: {0}", ex.Message));
            }

            return serviceResponse;
        }

        public ItemResponse<UserDto> ResetPassword(int id, string password, string validationToken)
        {
            ItemResponse<UserDto> serviceResponse = new ItemResponse<UserDto>(new ResponseModel(), null);

            try
            {
                _userRepository.ResetPassword(id, password, validationToken);

                try
                {
                    var entity = _userRepository.GetById(id);

                    var entityDto = Mapper.Map<User, UserDto>(entity);
                    serviceResponse.Code = ResponseCode.Success;
                    serviceResponse.Item = entityDto;
                }
                catch (ArgumentNullException)
                {
                    serviceResponse.Code = ResponseCode.NotFoundInContext;
                    serviceResponse.LogLevel = LogLevel.Information;
                    serviceResponse.Message = "Couldn't find the user on the repository.";

                    var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    var serializedObject = JsonConvert.SerializeObject(id, Common.CommonSerializerSettings.GetSettings());
                    _logRepository.Log("User Service - Reset Password",
                        ArchLevel.Repository, serializedObject, methodName,
                        string.Format("User with ID: {0} couldn't be found in the context.", id));
                }
                catch (Exception ex)
                {
                    serviceResponse.Code = ResponseCode.Error;
                    serviceResponse.LogLevel = LogLevel.Error;
                    serviceResponse.Message = "Error occured while trying to get the user from the context. " +
                        "See exception: " + ex.Message;

                    var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    var serializedObject = JsonConvert.SerializeObject(id, Common.CommonSerializerSettings.GetSettings());
                    _logRepository.Log("User Service - Reset Password",
                        ArchLevel.Repository, serializedObject, methodName,
                        string.Format("Exception occured while trying to get the User with ID: {0}. See following exception message: {1}.", id, ex.Message));
                }
            }
            catch (ArgumentNullException)
            {
                serviceResponse.Code = 0;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Couldn't find the user on the repository.";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(id, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Reset Password",
                    ArchLevel.Repository, serializedObject, methodName,
                string.Format("User with ID: {0} couldn't be found in the context.", id));
            }
            catch (UnauthorizedAccessException)
            {
                serviceResponse.Code = ResponseCode.AlreadyExistsInContext;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "User ID '" + id + "' tried to reset password with a wrong validation token";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(id, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Reset Password",
                    ArchLevel.Repository, serializedObject, methodName,
                string.Format("User with ID: {0} already exist in the context.", id));
            }
            catch (Exception ex)
            {
                serviceResponse.Code = ResponseCode.Error;
                serviceResponse.LogLevel = LogLevel.Error;
                serviceResponse.Message = "Error occured while trying to update the user from the context. " +
                    "See exception: " + ex.Message;


                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(id, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Reset Password",
                    ArchLevel.Repository, serializedObject, methodName,
                    string.Format("Exception occured while trying to update the User with ID: {0}. See following exception message: {1}.", id, ex.Message));
            }

            return serviceResponse;
        }

        public ItemResponse<UserDto> UpdateUser(UserDto user, string password)
        {
            ItemResponse<UserDto> serviceResponse = new ItemResponse<UserDto>(new ResponseModel(), null);

            try
            {
                var userEntity = Mapper.Map<UserDto, User>(user);

                if (!string.IsNullOrEmpty(password))
                {
                    _userRepository.Update(userEntity, password);
                }
                else
                {
                    _userRepository.Update(userEntity);
                }

                try
                {
                    var entity = _userRepository.GetById(userEntity.Id);

                    var entityDto = Mapper.Map<User, UserDto>(entity);
                    serviceResponse.Code = ResponseCode.Success;
                    serviceResponse.Item = entityDto;
                }
                catch (ArgumentNullException)
                {
                    serviceResponse.Code = ResponseCode.NotFoundInContext;
                    serviceResponse.LogLevel = LogLevel.Information;
                    serviceResponse.Message = "Couldn't find the user on the repository.";

                    var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                    _logRepository.Log("User Service - Update",
                        ArchLevel.Repository, serializedObject, methodName,
                        string.Format("Username: {0} with ID: {1} couldn't be found in the context.", user.UserName, user.Id));
                }
                catch (Exception ex)
                {
                    serviceResponse.Code = ResponseCode.Error;
                    serviceResponse.LogLevel = LogLevel.Error;
                    serviceResponse.Message = "Error occured while trying to get the user from the context. " +
                        "See exception: " + ex.Message;

                    var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                    _logRepository.Log("User Service - Update",
                        ArchLevel.Repository, serializedObject, methodName,
                        string.Format("Exception occured while trying to get the User with ID: {0}. See following exception message: {1}.", user.Id, ex.Message));
                }
            }
            catch (ArgumentNullException)
            {
                serviceResponse.Code = 0;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Couldn't find the user on the repository.";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Update",
                    ArchLevel.Repository, serializedObject, methodName,
                string.Format("Username: {0} with ID: {1} couldn't be found in the context.", user.UserName, user.Id));
            }
            catch (ApplicationException)
            {
                serviceResponse.Code = ResponseCode.AlreadyExistsInContext;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Username '" + user.UserName + "' is already taken";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Update",
                    ArchLevel.Repository, serializedObject, methodName,
                string.Format("Username: {0} already exist in the context.", user.UserName));
            }
            catch (Exception ex)
            {
                serviceResponse.Code = ResponseCode.Error;
                serviceResponse.LogLevel = LogLevel.Error;
                serviceResponse.Message = "Error occured while trying to update the user from the context. " +
                    "See exception: " + ex.Message;


                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Update",
                    ArchLevel.Repository, serializedObject, methodName,
                    string.Format("Exception occured while trying to update the User with ID: {0}. See following exception message: {1}.", user.Id, ex.Message));
            }

            return serviceResponse;
        }

        public ItemResponse<UserDto> UpdateUser(UserDto user)
        {
            ItemResponse<UserDto> serviceResponse = new ItemResponse<UserDto>(new ResponseModel(), null);

            try
            {
                var userEntity = Mapper.Map<UserDto, User>(user);
                _userRepository.Update(userEntity);

                try
                {
                    var entity = _userRepository.GetById(userEntity.Id);

                    var entityDto = Mapper.Map<User, UserDto>(entity);
                    serviceResponse.Code = ResponseCode.Success;
                    serviceResponse.Item = entityDto;
                }
                catch (ArgumentNullException)
                {
                    serviceResponse.Code = ResponseCode.NotFoundInContext;
                    serviceResponse.LogLevel = LogLevel.Information;
                    serviceResponse.Message = "Couldn't find the user on the repository.";

                    var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                    _logRepository.Log("User Service - Update",
                        ArchLevel.Repository, serializedObject, methodName,
                        string.Format("Username: {0} with ID: {1} couldn't be found in the context.", user.UserName, user.Id));
                }
                catch (Exception ex)
                {
                    serviceResponse.Code = ResponseCode.Error;
                    serviceResponse.LogLevel = LogLevel.Error;
                    serviceResponse.Message = "Error occured while trying to get the user from the context. " +
                        "See exception: " + ex.Message;

                    var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                    _logRepository.Log("User Service - Update",
                        ArchLevel.Repository, serializedObject, methodName,
                        string.Format("Exception occured while trying to get the User with ID: {0}. See following exception message: {1}.", user.Id, ex.Message));
                }
            }
            catch (ArgumentNullException)
            {
                serviceResponse.Code = 0;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Couldn't find the user on the repository.";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Update",
                    ArchLevel.Repository, serializedObject, methodName,
                string.Format("Username: {0} with ID: {1} couldn't be found in the context.", user.UserName, user.Id));
            }
            catch (ApplicationException)
            {
                serviceResponse.Code = ResponseCode.AlreadyExistsInContext;
                serviceResponse.LogLevel = LogLevel.Information;
                serviceResponse.Message = "Username '" + user.UserName + "' is already taken";

                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Update",
                    ArchLevel.Repository, serializedObject, methodName,
                string.Format("Username: {0} already exist in the context.", user.UserName));
            }
            catch (Exception ex)
            {
                serviceResponse.Code = ResponseCode.Error;
                serviceResponse.LogLevel = LogLevel.Error;
                serviceResponse.Message = "Error occured while trying to update the user from the context. " +
                    "See exception: " + ex.Message;


                var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                var serializedObject = JsonConvert.SerializeObject(user, Common.CommonSerializerSettings.GetSettings());
                _logRepository.Log("User Service - Update",
                    ArchLevel.Repository, serializedObject, methodName,
                    string.Format("Exception occured while trying to update the User with ID: {0}. See following exception message: {1}.", user.Id, ex.Message));
            }

            return serviceResponse;
        }
    }
}
