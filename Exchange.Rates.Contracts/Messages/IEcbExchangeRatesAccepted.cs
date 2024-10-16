﻿using Exchange.Rates.Contracts.Messages.Base;
using Exchange.Rates.Contracts.Models;

namespace Exchange.Rates.Contracts.Messages;

public interface IEcbExchangeRatesAccepted : IBaseContract
{
  string Symbols { get; }

  EcbCurrencyExchange CurrencyExchange { get; }

  string Message { get; }
}
