using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.QnA.Api.Types.Page;

namespace SFA.DAS.QnA.Application.Validators
{
    public class ValidatorFactory : IValidatorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidatorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public List<IValidator> Build(Question question)
        {
            var validators = new Dictionary<string, IValidator>();

            if (question?.Input != null)
            {
                var hasInputValidatorsSpecified = question.Input.Validations != null && question.Input.Validations.Count > 0;

                if (hasInputValidatorsSpecified)
                {
                    // Always load the input validators
                    // Note that if same validator is specified more than once, the latest one takes precedence
                    foreach (var inputValidation in question.Input.Validations.Where(v => v.Name != "ClientApiCall"))
                    {
                        var validatorName = $"{inputValidation.Name}Validator";
                        var validator = _serviceProvider.GetServices<IValidator>().FirstOrDefault(v => v.GetType().Name == validatorName);

                        if (validator != null)
                        {
                            validator.ValidationDefinition = inputValidation;
                            validators[validatorName] = validator;
                        }
                    }
                }

                var typeValidatorName = $"{question.Input.Type}TypeValidator";
                var typeValidator = _serviceProvider.GetServices<IValidator>().FirstOrDefault(v => v.GetType().Name == typeValidatorName);

                // If the type validator is found and doesn't have an overriding input validator, then add it
                if (typeValidator != null && !validators.ContainsKey($"{question.Input.Type}Validator"))
                {
                    validators[typeValidatorName] = typeValidator;
                }

                // If there are no validators for the question then add a default one (which happens to be NullValidator)
                if (validators.Count == 0)
                {
                    validators["NullValidator"] = new NullValidator();
                }
            }

            return validators.Values.ToList();
        }
    }
}