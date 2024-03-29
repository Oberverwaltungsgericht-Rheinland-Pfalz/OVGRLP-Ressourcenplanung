﻿// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Text;

namespace Rema.Infrastructure.Email
{
  public class EmailSettings
  {
    public Boolean useIISAccount { get; set; }
    public string EmailUsername { get; set; }
    public string EmailPassword { get; set; }
    public string Domain { get; set; }
    public Boolean EnableSSL { get; set; }
    public int Port { get; set; }
    public string EmailSenderAddress { get; set; }
    public Boolean SendEmails { get; set; }
  }
}
