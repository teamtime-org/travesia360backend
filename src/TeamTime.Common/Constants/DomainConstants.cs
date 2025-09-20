namespace TeamTime.Common.Constants;

public static class UserRoles
{
    public const string Administrator = "ADMINISTRADOR";
    public const string Coordinator = "COORDINADOR";
    public const string Collaborator = "COLABORADOR";
}

public static class ProjectStatus
{
    public const string Active = "ACTIVE";
    public const string OnHold = "ON_HOLD";
    public const string Completed = "COMPLETED";
    public const string Cancelled = "CANCELLED";
}

public static class Priority
{
    public const string Low = "LOW";
    public const string Medium = "MEDIUM";
    public const string High = "HIGH";
    public const string Urgent = "URGENT";
}

public static class TaskStatus
{
    public const string Todo = "TODO";
    public const string InProgress = "IN_PROGRESS";
    public const string Review = "REVIEW";
    public const string Done = "DONE";
}

public static class Limits
{
    public const int MaxHoursPerDay = 24;
    public const decimal MinHoursPerEntry = 0.25m; // 15 minutes
    public const int MaxDescriptionLength = 1000;
    public const int MaxNameLength = 200;
    public const int PageSizeDefault = 10;
    public const int PageSizeMax = 100;
}

public static class ErrorMessages
{
    public const string Unauthorized = "Token de acceso requerido";
    public const string Forbidden = "Permisos insuficientes para esta operación";
    public const string InvalidToken = "Token inválido o expirado";
    public const string UserNotFound = "Usuario no encontrado";
    public const string InvalidCredentials = "Credenciales inválidas";
    public const string EmailAlreadyExists = "El email ya está registrado";
    public const string ResourceNotFound = "Recurso no encontrado";
    public const string ValidationError = "Error de validación en los datos enviados";
    public const string InternalError = "Error interno del servidor";
    public const string DuplicateResource = "El recurso ya existe";
    public const string InvalidDateRange = "El rango de fechas es inválido";
    public const string HoursExceedLimit = "No se pueden registrar más de 24 horas por día";
    public const string AlreadyAssigned = "El usuario ya está asignado a este proyecto";
    public const string NotAssigned = "El usuario no está asignado a este proyecto";
    public const string PeriodNotActive = "El período de tiempo no está activo para captura";
    public const string CannotModifyApproved = "No se puede modificar un registro ya aprobado";
}

public static class SuccessMessages
{
    public const string UserCreated = "Usuario creado exitosamente";
    public const string UserUpdated = "Usuario actualizado exitosamente";
    public const string LoginSuccess = "Autenticación exitosa";
    public const string LogoutSuccess = "Sesión cerrada exitosamente";
    public const string ResourceCreated = "Recurso creado exitosamente";
    public const string ResourceUpdated = "Recurso actualizado exitosamente";
    public const string ResourceDeleted = "Recurso eliminado exitosamente";
    public const string AssignmentCreated = "Asignación creada exitosamente";
    public const string AssignmentRemoved = "Asignación removida exitosamente";
    public const string TimeEntryApproved = "Registro de horas aprobado exitosamente";
    public const string TimeEntryRejected = "Registro de horas rechazado exitosamente";
}