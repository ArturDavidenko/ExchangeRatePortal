using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestesExchangeRatesAPI.Helpers.XmlSamples
{
    public static class ValidFxRatesXml
    {
        public static string ValidEuRates() => @"
            <FxRates>
              <FxRate>
                <Dt>2024-01-15</Dt>
                <Tp>EU</Tp>

                <CcyAmt>
                  <Ccy>EUR</Ccy>
                  <Amt>1</Amt>
                </CcyAmt>

                <CcyAmt>
                  <Ccy>USD</Ccy>
                  <Amt>1.09</Amt>
                </CcyAmt>

                <CcyAmt>
                  <Ccy>GBP</Ccy>
                  <Amt>0.86</Amt>
                </CcyAmt>
              </FxRate>
            </FxRates>";
    }
}
