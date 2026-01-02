using FluentValidation;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests
{
    public class TrainingGameRequestDTOValidator : AbstractValidator<TrainingGameRequestDTO>
    {
        public TrainingGameRequestDTOValidator()
        {
            
        }
    }
}
