﻿using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Domain
{
    public class ResponseEntityDomainModelGeneratorModel : BaseGenericGeneratorModel
    {
        public override IEnumerable<string> Usings => new List<string>
        {
           nameof(System.ComponentModel.DataAnnotations),
        }.Union(base.Usings);

        public override AssetKind GeneratedClassKind => AssetKind.ResponseEntityDomainModel;

        public ResponseEntityDomainModelGeneratorModel(IViewModelBase viewModelBase) : base(viewModelBase)
        {
        }
    }
}