using PublishingCompany.Camunda.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublishingCompany.Camunda.Validators.User
{
    public class UserValidator : IUserValidator
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool UserExists(string email)
        {
            if (_unitOfWork.Users.GetUserByEmail(email) != null)
            {
                return true;
            }
            return false;
        }
    }
}
