using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DC.Business.Application.Contracts.Dtos.Account;
using DC.Business.Application.Contracts.Interfaces.Account;
using DC.Business.Application.Services.Helpers;
using DC.Business.Application.Services.Pipeline;
using DC.Business.Domain.Entities.Organization;
using DC.Business.Domain.Repositories.Organization;
using DC.Core.Contracts.Application.Pipeline.Dtos;
using DC.Core.Contracts.Application.Pipeline.Dtos.Errors;
using DC.Core.Contracts.Infrastructure.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DC.Business.WebApi.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using DC.Business.Application.Contracts.Dtos.Organization.Listing;
using DC.Business.Application.Contracts.Interfaces.Organization.Listing;
using DC.Business.Application.Contracts.Interfaces.Services;
using DC.Business.Domain.ElasticEntities;
using DC.Business.Application.Contracts.Dtos.Image;

namespace DC.Business.Application.Services.Account
{
    public class GetTokenByEmailAndPasswordService : BusinessService<EmailAndPasswordInputDto, string>, IGetTokenByEmailAndPasswordService
    {
        private static readonly string[] PasswordHashingAlgorithms = { "md5", "sha1", "sha256" };
        private readonly IUserRepository userRepository;
        private readonly ITokenService tokenService;
        private readonly IListingRepository _listingRepository;
        private readonly AppSettings _appSettings;
        private IMapper _mapper;
        private readonly IListPropertyService _listSellHouseService;
        private readonly IImageService _imageService;


        public GetTokenByEmailAndPasswordService(IUserRepository userRepository,
            ITokenService tokenService,
            IOptions<AppSettings> appSettings,
            IListingRepository listingRepository,
            IMapper mapper,
            IListPropertyService listSellHouseService,
            IImageService imageService)
        {
            _listSellHouseService = listSellHouseService ?? throw new ArgumentNullException(nameof(listSellHouseService));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            this._appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings.Value));
            _listingRepository = listingRepository ?? throw new ArgumentNullException(nameof(listingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        }

        protected override async Task<OperationResultDto<string>> ExecuteAsync(EmailAndPasswordInputDto inputDto, CancellationToken cancellationToken = default)
        {
            // Guid newSession = Guid.NewGuid();

            List<ErrorDto> executionErrors = ValidateInput(inputDto);
            if (executionErrors.Any()) return BuildOperationResultDto(executionErrors);

            List<string> hashedPasswords = new List<string>();
            foreach (string algorithm in PasswordHashingAlgorithms)
                hashedPasswords.Add(AuthenticationHelper.GetHashedPasswordForDatabase(inputDto.Password, algorithm));

            User user = await userRepository.GetUserByEmailAndPasswordsAsync(inputDto.Email, hashedPasswords);
            if (user == null)
                return BuildOperationResultDto(new ErrorDto(ErrorCodes.INVALID_AUTHENTICATION_DATA, nameof(inputDto.Email), inputDto.Email));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
              {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, "Admin")  // TODO user.Role
              }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            //List<Claim> userClaims = new List<Claim>
            //{
            //    new Claim(DCClaimTypes.NameIdentifier, user.Id.ToString()),
            //    new Claim(DCClaimTypes.UniqueNameClaim, user.Email), // Change to email
            //    new Claim(DCClaimTypes.SessionId, newSession.ToString()),
            //    //new Claim(DCClaimTypes.SystemAdmin, AuthorizationHelper.IsSuperUser(user).ToString()),
            //    new Claim(DCClaimTypes.OriginalUsername, user.Email) // Change to email
            //    //new Claim(DCClaimTypes.OriginalSystem, tokenService.CurrentAudience)
            //};
            //Token

            // Validate Temp
            // BREAKING SOLID !!! Probably extract to rabbitmq!!!
            if (inputDto.TempId != null && inputDto.TempId != Guid.Empty)
            {
                var property = await _listingRepository.GetPropertyByTempId((Guid)inputDto.TempId);
                if(property != null)
                {
                    var sellHouseDto = _mapper.Map<SellHouseDto>(property);
                    sellHouseDto.UserId = user.Id;
                    CancellationToken cancellationTkn = default;
                    var result = await _listSellHouseService.ExecuteServiceAsync(sellHouseDto, cancellationTkn);

                    if(property.Images.Count > 0)
                    {
                        var propertyImages = new List<PropertyImageDto>();
                        foreach (var tempImp in property.Images)
                        {
                            await _imageService.AddPropertyImagePath((int)result.Data, tempImp.ImageName, tempImp.ImageUrl);
                            propertyImages.Add(new PropertyImageDto() { ImageUrl = tempImp.ImageUrl, ImageName = tempImp.ImageName });
                        }
                        Thread.Sleep(1500);// TOO FAST NEED GIVE SOME TIME FOR ELASTIC
                        await _imageService.AddPropertyImagePathToElastic((int)result.Data, propertyImages);

                        // DELETE Temp property
                        await _listingRepository.DeleteTempPropertyById(property.Id);
                    }

                };
            }
            return BuildOperationResultDto(token);
        }

        private List<ErrorDto> ValidateInput(EmailAndPasswordInputDto inputDto)
        {
            if (inputDto is null)
                return new List<ErrorDto> { new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(inputDto)) };

            List<ErrorDto> validationErrors = new List<ErrorDto>();

            if (string.IsNullOrEmpty(inputDto.Email))
                validationErrors.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(inputDto.Email)));

            if (string.IsNullOrEmpty(inputDto.Password))
                validationErrors.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(inputDto.Password)));

            return validationErrors;
        }
    }
}
