﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Documentation
{
  public interface IElement
  {
    T Visit<T>(IElementVisitor<T> visitor);
  }
}
