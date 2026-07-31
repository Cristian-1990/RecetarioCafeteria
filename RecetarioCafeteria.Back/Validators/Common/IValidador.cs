using RecetarioCafeteria.Back.Errors.Common;
using CSharpFunctionalExtensions;

namespace RecetarioCafeteria.Back.Validators.Common;

public interface IValidador<T>
{
    Result<T, DomainError> Validar(T entidad);
}
