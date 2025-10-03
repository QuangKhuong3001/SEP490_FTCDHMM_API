using AutoMapper;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;
namespace SEP490_FTCDHMM_API.Api.Mappings
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            //register
            CreateMap<APIDtos.AuthDTOs.RegisterDto, ApplicationDtos.AuthDTOs.RegisterDto>();

            //verifyEmail
            CreateMap<APIDtos.AuthDTOs.OtpVerifyDto, ApplicationDtos.AuthDTOs.OtpVerifyDto>();

            //resentOtp
            CreateMap<APIDtos.AuthDTOs.ResendOtpDto, ApplicationDtos.AuthDTOs.ResendOtpDto>();

            //login
            CreateMap<APIDtos.AuthDTOs.LoginDto, ApplicationDtos.AuthDTOs.LoginDto>();

            //changePassword
            CreateMap<APIDtos.AuthDTOs.ChangePasswordDto, ApplicationDtos.AuthDTOs.ChangePasswordDto>();

            //forgotPassword
            CreateMap<APIDtos.AuthDTOs.ForgotPasswordRequestDto, ApplicationDtos.AuthDTOs.ForgotPasswordRequestDto>();
            CreateMap<APIDtos.AuthDTOs.VerifyOtpForPasswordResetDto, ApplicationDtos.AuthDTOs.VerifyOtpForPasswordResetDto>();
            CreateMap<APIDtos.AuthDTOs.ResetPasswordWithTokenDto, ApplicationDtos.AuthDTOs.ResetPasswordWithTokenDto>();

            //gg
            CreateMap<APIDtos.GoogleAuthDtos.GoogleCodeLoginRequest, ApplicationDtos.GoogleAuthDtos.GoogleCodeLoginRequest>();
            CreateMap<APIDtos.GoogleAuthDtos.GoogleIdTokenLoginRequest, ApplicationDtos.GoogleAuthDtos.GoogleIdTokenLoginRequest>();


        }
    }
}