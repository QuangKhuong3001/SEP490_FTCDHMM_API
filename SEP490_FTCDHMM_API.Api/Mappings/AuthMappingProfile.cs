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
            CreateMap<APIDtos.AuthDTOs.RegisterRequest, ApplicationDtos.AuthDTOs.RegisterDto>();

            //verifyEmail
            CreateMap<APIDtos.AuthDTOs.OtpVerifyRequest, ApplicationDtos.AuthDTOs.OtpVerifyRequest>();

            //resentOtp
            CreateMap<APIDtos.AuthDTOs.ResendOtpRequest, ApplicationDtos.AuthDTOs.ResendOtpRequest>();

            //login
            CreateMap<APIDtos.AuthDTOs.LoginRequest, ApplicationDtos.AuthDTOs.LoginRequest>();

            //changePassword
            CreateMap<APIDtos.AuthDTOs.ChangePasswordRequest, ApplicationDtos.AuthDTOs.ChangePasswordRequest>();

            //forgotPassword
            CreateMap<APIDtos.AuthDTOs.ForgotPasswordRequest, ApplicationDtos.AuthDTOs.ForgotPasswordRequest>();
            CreateMap<APIDtos.AuthDTOs.VerifyOtpForPasswordResetRequest, ApplicationDtos.AuthDTOs.VerifyOtpForPasswordResetRequest>();
            CreateMap<APIDtos.AuthDTOs.ResetPasswordWithTokenDto, ApplicationDtos.AuthDTOs.ResetPasswordWithTokenDto>();

            //gg
            CreateMap<APIDtos.GoogleAuthDtos.GoogleCodeLoginRequest, ApplicationDtos.GoogleAuthDtos.GoogleCodeLoginRequest>();
            CreateMap<APIDtos.GoogleAuthDtos.GoogleIdTokenLoginRequest, ApplicationDtos.GoogleAuthDtos.GoogleIdTokenLoginRequest>();


        }
    }
}