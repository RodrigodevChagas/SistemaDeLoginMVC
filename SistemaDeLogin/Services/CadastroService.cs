﻿using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using SistemaDeLogin.Data.Dtos;
using SistemaDeLogin.Data.Requests;
using SistemaDeLogin.Models;

namespace SistemaDeLogin.Services
{
    public class CadastroService
    {
        private IMapper _mapper;
        private UserManager<IdentityUser<int>> _userManager;

        public CadastroService(IMapper mapper, UserManager<IdentityUser<int>> userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        public Result CadastraUsuario(CreateUsuarioDto createUsuarioDto)
        {
            
            Usuarios usuario = _mapper.Map<Usuarios>(createUsuarioDto);

            IdentityUser<int> usuarioIdentity = _mapper.Map<IdentityUser<int>>(usuario);
            
            Task<IdentityResult> ResultadoIdentity = _userManager.CreateAsync(usuarioIdentity, createUsuarioDto.Password);

            if (ResultadoIdentity.Result.Succeeded) {

                var code =  _userManager.GenerateEmailConfirmationTokenAsync(usuarioIdentity).Result;
                return Result.Ok()
                    .WithSuccess(code);
            } 
            
            return Result.Fail("Falha ao cadastrar usuário");
        }

        public Result AtivaUsuario(AtivaContaRequest request)
        {
            var IdentityUser = _userManager
                .Users
                .FirstOrDefault(u => u.Id == request.Id);
            var IdentityResult = _userManager.ConfirmEmailAsync(IdentityUser, request.CodigoDeAtivacao).Result;

            if (IdentityResult.Succeeded) return Result.Ok();
            return Result.Fail("Falha ao ativar conta de usuario");
        }
    }
}
