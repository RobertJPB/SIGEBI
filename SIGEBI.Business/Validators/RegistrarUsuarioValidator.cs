using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.Validators;
using SIGEBI.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGEBI.Business.Validators
{
    // validador: para reglas de afuera (bd, unicidad, etc)
    public class RegistrarUsuarioValidator : IRegistrarUsuarioValidator
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public RegistrarUsuarioValidator(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<List<string>> ValidarAsync(UsuarioDTO dto)
        {
            var errores = new List<string>();

            // Validar Nombre
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                errores.Add("El nombre es obligatorio.");
            else if (dto.Nombre.Length > 100)
                errores.Add("El nombre no puede superar los 100 caracteres.");

            // Validar Correo usando el Value Object
            try
            {
                var emailVo = new Email(dto.Correo);
                // la unicidad es de negocio, no de formato (por eso va aca)
                var existente = await _usuarioRepository.GetByCorreoAsync(emailVo.Value);
                if (existente != null)
                    errores.Add("Ya existe un usuario registrado con este correo electrónico.");
            }
            catch (ArgumentException ex)
            {
                errores.Add(ex.Message);
            }

            // Validar Contraseña usando el Value Object
            try
            {
                var contrasenaVo = new Contrasena(dto.Contrasena);
            }
            catch (ArgumentException ex)
            {
                errores.Add(ex.Message);
            }

            return errores;
        }

        public async Task<bool> EsValidoAsync(UsuarioDTO dto)
        {
            var errores = await ValidarAsync(dto);
            return errores.Count == 0;
        }
    }
}
