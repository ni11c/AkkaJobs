using System;
using System.Numerics;

namespace Agridea.Banking
{
    /// <summary>
    /// http://www.raiffeisen.ch/web/iban2 
    /// Le compte IBAN est composé de 6 blocs 
    /// CHkk BBBB BCCC CCCC CCCC C
    /// Le dernier caractère de l'IBAN est une clé de controle. 
    /// Controle de la saisie : 
    /// 1.	Enlever les caractères indésirables (espaces, tirets)
    /// 2.	Déplacer les 4 premiers caractères à droite
    /// 3.	Substituer les lettres par des chiffres via une table de conversion (A=10, B=11, C=12 etc.)
    /// 4.	Diviser le nombre ainsi obtenu par 97.
    /// 5.	Si le reste n'est pas égal à 1 l'IBAN est incorrect : Modulo de 97 égal à 1
    /// </summary>
    public class IbanNumber
    {
        private string ibanNumber_;
        private static readonly int ACode = Convert.ToInt32('A');
        private static readonly int ZeroCode = Convert.ToInt32('0');
        private static readonly int NineCode = Convert.ToInt32('9');
        private static readonly int IbanLength = 21;
        private static readonly int CountOfLeftDigitsToMove = 4;
        private static readonly int PrimeDivisor = 97;
        private static readonly int ExpectedRemainder = 1;

        public IbanNumber(string ibanNumber)
        {
            ibanNumber_ = ibanNumber;
        }
        public bool IsValid()
        {
            var phase1 = ibanNumber_.Trim().Replace(" ", string.Empty).Replace("-", string.Empty).ToUpper();
            if (phase1.Length != IbanLength) return false;

            var phase2 = phase1.Substring(CountOfLeftDigitsToMove, phase1.Length - CountOfLeftDigitsToMove) + phase1.Substring(0, CountOfLeftDigitsToMove);

            var phase3 = string.Empty;
            foreach (char c in phase2)
            {
                var cCode = Convert.ToInt32(c);
                var cTransformed = cCode >= ZeroCode && cCode <= NineCode
                    ? c.ToString()
                    : (cCode - ACode + 10).ToString();
                phase3 += cTransformed;
            }

            BigInteger phase4;
            BigInteger.TryParse(phase3, out phase4);

            var phase5 = BigInteger.Remainder(phase4, PrimeDivisor);

            return phase5 == ExpectedRemainder;
        }
    }
}
