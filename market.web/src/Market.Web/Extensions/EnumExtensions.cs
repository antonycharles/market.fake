using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Market.Web.Extensions
{
    public static class EnumExtensions
    {
        
    public static string GetDisplayName(this Enum enumValue)
    {
        // Pega o tipo do Enum e o campo correspondente
        FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

        // Busca o atributo DisplayAttribute associado ao campo
        DisplayAttribute[] attributes = (DisplayAttribute[])fi.GetCustomAttributes(
            typeof(DisplayAttribute), false);

        // Retorna o Name caso exista, senão retorna o próprio ToString do Enum
        if (attributes != null && attributes.Length > 0)
        {
            return attributes[0].Name;
        }

        return enumValue.ToString();
    }
    }
}