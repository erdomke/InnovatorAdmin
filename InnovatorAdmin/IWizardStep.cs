using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aras.Tools.InnovatorAdmin
{
  public interface IWizardStep
  {
    void Configure(IWizard wizard);
    void GoNext();
  }
}
