using ASP.Migrations;
using Microsoft.AspNetCore.Mvc;

namespace ASP.Models.Home.Signup.MailTemplates
{
    public class ReserveMailModel
    {
        public string? User { get; set; }
        public string? Room { get; set; }
        public string? Location { get; set; }
        public string? Date { get; set; }
        public string? Price { get; set; }
        public string? GetSubject()
        {
            return "Резервация";
        }
        public string GetBody()
        {
            return $"<h2>Здравствуйте {User}. Сообщаем Вам о резервации в {Location}, комнату {Room}, цена {Price}₴.</h2>" +
            $"<p style='color: orange'>Дата резервации {Date}</p>";
        }
    }
}
