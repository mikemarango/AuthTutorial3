﻿using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.AuthHandlers
{
  public class ImageOwnership : IAuthorizationRequirement
  {
    public ImageOwnership()
    {

    }
  }
}
