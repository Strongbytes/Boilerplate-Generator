﻿namespace BoilerplateGenerator.Models.Enums
{
    public enum AssetKind
    {
        Directory,
        ResponseDomainEntity,
        Controller,
        CreateRequestDomainEntity,
        UpdateRequestDomainEntity,
        GetAllQuery,
        GetPaginatedQuery,
        GetByIdQuery,
        CreateCommand,
        UpdateCommand,
        DeleteCommand,
        GetAllQueryHandler,
        GetPaginatedQueryHandler,
        GetByIdQueryHandler,
        CreateCommandHandler,
        UpdateCommandHandler,
        DeleteCommandHandler,
        ProfileMapper,
        UnitOfWorkInterface,
        UnitOfWorkImplementation,
        EntityRepositoryInterface,
        EntityRepositoryImplementation,
        DbContext,
    }
}
